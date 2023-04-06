using System.CommandLine;
using System.CommandLine.Completions;
using Spectre.Console;
using VmChamp;

public class RemoveCommand : Command
{
  private readonly AppConfig _appConfig;

  public RemoveCommand(AppConfig appConfig) : base("remove", "removes a vm")
  {
    _appConfig = appConfig;
    
    Argument<string> nameArgument = new("name",
      () => appConfig.DefaultVmName,
      "Name of the VM to remove.");

    this.AddAlias("rm");
    this.AddArgument(nameArgument);
    
    nameArgument.AddCompletions((ctx) => 
      Helper.GetAllVmInDirectory(_appConfig.DataDir)
        .Select(vmName => new CompletionItem(vmName ?? "")));
    
    nameArgument.AddCompletions((ctx) => 
      Helper.GetAllVmInDirectory(_appConfig.DataDir)
        .Select(vmName => new CompletionItem(vmName ?? "")));

    this.SetHandler((vmName) =>
    {
      using var libvirtConnection = LibvirtConnection.CreateForSession();
      
      var vmId = Interop.virDomainLookupByName(libvirtConnection.NativePtr, vmName);
      var vmDir = Path.Combine(_appConfig.DataDir, vmName);
      
      Interop.DestroyVm(vmId, vmName, vmDir);
      Helper.DeleteExistingDirectory(vmDir);
    }, nameArgument);
  }
}