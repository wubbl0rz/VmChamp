using System.CommandLine;
using Spectre.Console;
using VmChamp;

public class VMCleanCommand : Command
{
    private readonly AppConfig _appConfig;

    public VMCleanCommand(AppConfig appConfig) : base("vmclean", "delete all vms without images")
    {
        _appConfig = appConfig;
    
        this.AddAlias("vpurge");

        var allVmDirectories = Directory.GetDirectories(appConfig.DataDir);
    
        this.SetHandler(() =>
        {
            if (allVmDirectories.Length == 0)
            {
                AnsiConsole.MarkupLine("[green]👀 No VMs found to delete.[/]");
            }
      
            AnsiConsole.MarkupLine($"[red]Going to delete all VMs ({allVmDirectories.Length}) without IMAGES[/]");
    
            if (AnsiConsole.Ask<string>("Continue? (y/N)", "N").ToLower() != "y")
            {
                return;
            }
    
            using var libvirtConnection = LibvirtConnection.CreateForSession();
    
            foreach (var vmDir in allVmDirectories)
            {
                var vmName = Path.GetFileName(vmDir);
                var vmId = Interop.virDomainLookupByName(libvirtConnection.NativePtr, vmName);
        
                Interop.DestroyVm(vmId, vmName, vmDir);
            }
  
        });
    }
}