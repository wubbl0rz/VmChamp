using System.Diagnostics;
using System.IO.Pipes;
using System.Net.NetworkInformation;
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

  public static bool CheckDefaultNetwork()
  {
    var defaultBridge= NetworkInterface.GetAllNetworkInterfaces()
      .FirstOrDefault(iface => iface.Name == "virbr0");

    var defaultBridgeHasIp  = defaultBridge?.GetIPProperties()
      .UnicastAddresses
      .Select(info => info.Address)
      .Any();

    if (defaultBridgeHasIp is true)
      return true;

    var script = "";

    if (defaultBridge == null)
    {
      AnsiConsole.MarkupLine("[red]Default network bridge: virbr0 not found. " +
                             "Normally libvirt should have created a bridge after installation.[/]");

      if (AnsiConsole.Ask<string>("Do you want to create one now? (y/N)", "N").ToLower() != "y")
      {
        return false;
      }

      AnsiConsole.WriteLine("👉 The following default network and bridge is going to be created: ");
      AnsiConsole.WriteLine(Interop.DefaultNetworkDefiniton);

      script = $"""
        if ! virsh --connect qemu:///system net-start default &> /dev/null; then
          virsh net-undefine default &> /dev/null
          virsh --connect qemu:///system net-define <(echo \"{Interop.DefaultNetworkDefiniton}\") &> /dev/null
          virsh --connect qemu:///system net-destroy default &> /dev/null
        fi
        virsh --connect qemu:///system net-start default &> /dev/null
        virsh --connect qemu:///system net-autostart default &> /dev/null
        """;
    }
    else
    {
      script = $"""
        IP=$(virsh net-dumpxml default | grep 'ip address' | cut -d \' -f 2)
        MASK=$(virsh net-dumpxml default | grep 'ip address' | cut -d \' -f 4)
        ip a add $IP/$MASK dev virbr0
        ip link set dev virbr0 up
        """;

      AnsiConsole.MarkupLine("[red]Default network has no IP address or is down.[/]");

      if (AnsiConsole.Ask<string>("Do you want to start it now? (y/N)", "N").ToLower() != "y")
      {
        return false;
      }
    }

    var startInfo = new ProcessStartInfo("sudo")
    {
      Arguments = $"bash -c \"{script}\"",
    };

    Process.Start(startInfo)?.WaitForExit();

    return true;
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