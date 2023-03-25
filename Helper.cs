using System.Diagnostics;
using Spectre.Console;

namespace VmChamp;

public class Helper
{
  public static Process? ConnectViaSsh(string user, string ip)
  {
    var startInfo = new ProcessStartInfo("ssh");
    startInfo.Arguments = $"-o StrictHostKeyChecking=off {user}@{ip}";

    return Process.Start(startInfo);
  }

  public static void DeleteExistingDirectory(string directoryPath)
  {
    if (Directory.Exists(directoryPath))
    {
      Directory.Delete(directoryPath, true);
    }
  }

  public static void ResizeImage(string file, double size)
  {
    var startInfo = new ProcessStartInfo("qemu-img")
    {
      Arguments = $"resize {file} {size}",
      RedirectStandardOutput = true
    };

    Process.Start(startInfo)?.WaitForExit();
  }

  public static string?[] GetAllVmInDirectory(string path) =>
    Directory.GetDirectories(path)
      .Select(Path.GetFileName)
      .ToArray();
}