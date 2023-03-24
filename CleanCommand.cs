using System.CommandLine;
using Spectre.Console;
using VmChamp;

public class CleanCommand : Command
{
  private readonly AppConfig _appConfig;

  public CleanCommand(AppConfig appConfig) : base("clean", "delete all vms and images")
  {
    _appConfig = appConfig;

    var allVmDirectories = Directory.GetDirectories(appConfig.DataDir);
    
    this.SetHandler(() =>
    {
      if (allVmDirectories.Length == 0)
      {
        AnsiConsole.MarkupLine("[green]👀 No VMs found to delete.[/]");
      }
      
      AnsiConsole.MarkupLine($"[red]Going to delete all VMs ({allVmDirectories.Length}) and IMAGES[/]");
    
      if (AnsiConsole.Ask<string>("Continue? (y/N)", "N").ToLower() != "y")
      {
        return;
      }
    
      using var libvirtConnection = LibvirtConnection.Create("qemu:///session");
    
      foreach (var vmDir in allVmDirectories)
      {
        var vmName = Path.GetFileName(vmDir);
        var vmId = Interop.virDomainLookupByName(libvirtConnection.NativePtr, vmName);
        
        Interop.DestroyVm(vmId, vmName, vmDir);
      }
  
      Directory.Delete(appConfig.CacheDir, true);
    });
  }
}