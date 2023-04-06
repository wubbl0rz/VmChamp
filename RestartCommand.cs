using System.CommandLine;
using System.CommandLine.Completions;
using Spectre.Console;
using VmChamp;

public class RestartCommand : Command
{
  private readonly AppConfig _appConfig;

  public RestartCommand(AppConfig appConfig) : base("restart", "force restarts a vm")
  {
    _appConfig = appConfig;

    Argument<string> nameArgument = new("name",
      () => appConfig.DefaultVmName,
      "Name of the VM to restart.");

    this.AddAlias("reboot");
    this.AddAlias("reset");
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
      AnsiConsole.MarkupLine($"[yellow]🔄 Restart VM: {vmName}[/]");

      Interop.virDomainReset(vmId);
    }, nameArgument);
  }
}