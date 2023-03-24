using System.CommandLine;
using Spectre.Console;
using VmChamp;

public class ListCommand : Command
{
  private readonly AppConfig _appConfig;

  public ListCommand(AppConfig appConfig) : base("list", "list all existing vms")
  {
    _appConfig = appConfig;

    var allVmDirectories = Directory.GetDirectories(appConfig.DataDir);
    
    this.AddAlias("ls");
    
    this.SetHandler(() =>
    {
      using var libvirtConnection = LibvirtConnection.Create("qemu:///session");
      
      var table = new Table();
      table.AddColumn("VM");
      table.AddColumn("IP");

      foreach (var vmDir in allVmDirectories)
      {
        var vmName = Path.GetFileName(vmDir);
        var vmId = Interop.virDomainLookupByName(libvirtConnection.NativePtr, vmName);
        
        table.AddRow(vmName, Interop.GetFirstIpById(vmId) ?? "none");
      }
  
      AnsiConsole.Write(table);
    });
  }
}