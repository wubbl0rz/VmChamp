using System.Diagnostics;
using Spectre.Console;

namespace VmChamp;

public class Helper
{
  public static Process? ConnectViaSsh(string user, string ip)
  {
    ProcessStartInfo s = new ProcessStartInfo("ssh");
    s.Arguments = $"-o StrictHostKeyChecking=off {user}@{ip}";

    return Process.Start(s);
  }

  public static void DeleteExistingDirectory(string directoryPath)
  {
    if (Directory.Exists(directoryPath))
    {
      Directory.Delete(directoryPath, true);
    }
  }

  public static string?[] GetAllVmInDirectory(string path) =>
    Directory.GetDirectories(path)
      .Select(Path.GetFileName)
      .ToArray();
}