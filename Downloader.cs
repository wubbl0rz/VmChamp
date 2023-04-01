namespace VmChamp;

using System.Net;
using Spectre.Console;

public class Downloader
{
  private readonly DirectoryInfo _cacheDirectory;

  public Downloader(DirectoryInfo cacheDirectory)
  {
    _cacheDirectory = cacheDirectory;
  }

  public async Task<FileInfo> DownloadAsync(DistroInfo distroInfo, bool force = false)
  {
    var targetFile = new FileInfo(Path.Combine(_cacheDirectory.FullName, distroInfo.ImageName));

    if (targetFile.Exists && !force)
    {
      AnsiConsole.WriteLine($"Using existing image: {distroInfo.ImageName}");
      return targetFile;
    }

    var uri = new Uri(new Uri(distroInfo.Url), distroInfo.ImageName);

    AnsiConsole.WriteLine($"Download: {uri}");

#pragma warning disable SYSLIB0014
    var webClient = new WebClient();
#pragma warning restore SYSLIB0014

    await AnsiConsole.Progress()
      .Columns(new SpinnerColumn(), new PercentageColumn(), new RemainingTimeColumn())
      .StartAsync(async ctx =>
      {
        var task = ctx.AddTask($"progress");

        webClient.DownloadProgressChanged += (_, eventArgs) => { task.Value = eventArgs.ProgressPercentage; };

        await webClient.DownloadFileTaskAsync(uri, targetFile.FullName);
      });

    return targetFile;
  }
}
