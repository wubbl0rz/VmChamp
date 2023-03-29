using System.CommandLine;
using System.Net.NetworkInformation;
using ByteSizeLib;
using Spectre.Console;
using VmChamp;

public class ListCommand : Command
{
  private readonly AppConfig _appConfig;

  public ListCommand(AppConfig appConfig) : base("list", "list all existing vms")
  {
    _appConfig = appConfig;

    var allVmDirectories = Directory.GetDirectories(appConfig.DataDir);
    
    this.AddAlias("ps");
    this.AddAlias("ls");
    
    this.SetHandler(() =>
    {
      using var libvirtConnection = LibvirtConnection.Create("qemu:///session");
      
      var table = new Table();
      table.AddColumn("VM");
      table.AddColumn("IP");
      table.AddColumn("State");
      table.AddColumn("Memory");
      table.AddColumn("Disk");
      table.AddColumn("vCPUs");

      foreach (var vmDir in allVmDirectories)
      {
        var vmName = Path.GetFileName(vmDir);
        var vmId = Interop.virDomainLookupByName(libvirtConnection.NativePtr, vmName);

        var pinger = new Ping();
        
        unsafe
        {
          var vmIpAddress = Interop.GetFirstIpById(vmId);
          var stateInfo = new VirDomainInfo();
          var blockInfo = new VirDomainBlockInfo();
          Interop.virDomainGetInfo(vmId, &stateInfo);
          Interop.virDomainGetBlockInfo(vmId, "vda", &blockInfo);

          var pingAlive = "[grey]";

          if (vmIpAddress is not null)
          {
            var pingReply = pinger.Send(vmIpAddress, 5);
            pingAlive = pingReply.Status != IPStatus.Success ? "[red]" : "[green]";
          }

          var state = (VirDomainState)stateInfo.state;

          var color = state switch
          {
            VirDomainState.Running => "[green]",
            VirDomainState.Paused => "[yellow]",
            _ => "[red]"
          };

          var curMemSize = ByteSize.FromKibiBytes(stateInfo.memory);
          var curDiskSize = ByteSize.FromBytes(blockInfo.allocation);
          var maxDiskSize = ByteSize.FromBytes(blockInfo.capacity);

          table.AddRow(vmName,
            $"{pingAlive}{(vmIpAddress ?? "none")}[/]",
            $"{color}{state}[/]",
            $"{curMemSize:MiB}",
            $"{curDiskSize:GiB}/{maxDiskSize:GiB}",
            stateInfo.nrVirtCpu.ToString());
        }
      }
  
      AnsiConsole.Write(table);
    });
  }
}