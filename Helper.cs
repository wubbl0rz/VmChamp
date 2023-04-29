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
    startInfo.Arguments = $"-Y -o StrictHostKeyChecking=off {user}@{ip}";

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

    var bridgeFileExists = File.Exists("/etc/qemu/bridge.conf");

    if (defaultBridgeHasIp is true && bridgeFileExists)
      return true;

    var script = "";

    if (defaultBridge == null || !bridgeFileExists)
    {
      AnsiConsole.MarkupLine("[red]Default network bridge: virbr0 missing configuration. " +
                             "Some distributions do not setup a bridge automatically.[/]");

      var hasFreeNetwork = NetworkInterface.GetAllNetworkInterfaces()
        .SelectMany(i => i.GetIPProperties().UnicastAddresses)
        .Any(i => i.ToString()?.Contains("192.168.122.") is true);

      var thirdOctet = hasFreeNetwork ? "122" : "22";

      var bridgeName = $"virbr0";
      var bridgeIp = $"192.168.{thirdOctet}.1";
      var bridgeMask = "255.255.255.0";
      var bridgeDhcpStart = $"192.168.{thirdOctet}.128";
      var bridgeDhcpEnd = $"192.168.{thirdOctet}.254";

      AnsiConsole.WriteLine("👉 The following default network can be created: ");
      AnsiConsole.WriteLine($"Name: {bridgeName}");
      AnsiConsole.WriteLine($"IP: {bridgeIp}");
      AnsiConsole.WriteLine($"Mask: {bridgeMask}");
      AnsiConsole.WriteLine();

      if (AnsiConsole
            .Ask<string>("Do you want to create it now and allow VMs to use it? (y/N)", "N")
            .ToLower() != "y")
      {
        return false;
      }

      var bridgeXml = Interop.GenVirBridgeXml(bridgeName, bridgeIp, bridgeMask, bridgeDhcpStart, bridgeDhcpEnd);

      script = $"""
        if ! test -f /etc/qemu/bridge.conf; then
          mkdir -p /etc/qemu &> /dev/null
          echo \"allow virbr0\" > /etc/qemu/bridge.conf
          chmod u+s /usr/lib/qemu/qemu-bridge-helper
        fi

        if ! virsh --connect qemu:///system net-start default &> /dev/null; then
          virsh net-undefine default &> /dev/null
          virsh --connect qemu:///system net-define <(echo \"{bridgeXml}\") &> /dev/null
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