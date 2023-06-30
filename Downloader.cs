
namespace VmChamp;

using System.Net;
using Spectre.Console;

public class Downloader
{
  private readonly DirectoryInfo _cacheDirectory;
  private readonly ChecksumHelper _checksumHelper;

  public Downloader(DirectoryInfo cacheDirectory)
  {
    _cacheDirectory = cacheDirectory;
    _checksumHelper = new ChecksumHelper();

  }

  public async Task<FileInfo> DownloadAsync(DistroInfo distroInfo, bool force = false)
  {
    var targetFile = new FileInfo(Path.Combine(_cacheDirectory.FullName, distroInfo.ImageName + ".qcow2"));

    if (targetFile.Exists && !force)
    {
      AnsiConsole.WriteLine($"Using existing image: {distroInfo.ImageName}");
      return targetFile;
    }

    var uri = new Uri(new Uri(distroInfo.Url), distroInfo.ImageName);
    var checksumFileUri = new Uri(new Uri(distroInfo.Url), distroInfo.ChecksumFile);
    var checksumFile = new FileInfo(Path.Combine(_cacheDirectory.FullName, distroInfo.ImageName + "." + distroInfo.ChecksumType));

    
    
#pragma warning disable SYSLIB0014
    var webClient = new WebClient();
#pragma warning restore SYSLIB0014

    //Download checksum file
    if (distroInfo.ChecksumFile != "/dev/null") //Set /dev/null as checksum file if distro has no (working) checksum file
    {
      AnsiConsole.WriteLine($"Download: {checksumFileUri}");

      await AnsiConsole.Progress()
        .Columns(new SpinnerColumn(), new PercentageColumn(), new RemainingTimeColumn())
        .StartAsync(async ctx =>
        {
          var task = ctx.AddTask($"progress");

          webClient.DownloadProgressChanged += (_, eventArgs) => { task.Value = eventArgs.ProgressPercentage; };

          await webClient.DownloadFileTaskAsync(checksumFileUri, checksumFile.FullName);
        });
    }

    //Download Image File
    AnsiConsole.WriteLine($"Download: {uri}");

    await AnsiConsole.Progress()
      .Columns(new SpinnerColumn(), new PercentageColumn(), new RemainingTimeColumn())
      .StartAsync(async ctx =>
      {
        var task = ctx.AddTask($"progress");

        webClient.DownloadProgressChanged += (_, eventArgs) => { task.Value = eventArgs.ProgressPercentage; };

        await webClient.DownloadFileTaskAsync(uri, targetFile.FullName);
      });

    int checksumcheck = _checksumHelper.ChecksumCheck(distroInfo, targetFile, checksumFile); //Doing checksum checking and calculating

    switch (checksumcheck)
    {
      case 1:
      AnsiConsole.WriteLine("The checksum is good!");
        break;

      case 0:
        AnsiConsole.WriteLine("The checksum is wrong, exiting!");
        Environment.Exit(0);
        break;

      case 2:
        AnsiConsole.WriteLine("Warning: The checksum not verified!");
        break;


    }


    return targetFile;
  }
}
