using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Completions;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Spectre.Console;
using VmChamp;

var appConfig = new AppConfig();
Directory.CreateDirectory(appConfig.CacheDir);
Directory.CreateDirectory(appConfig.DataDir);

var downloader = new Downloader(new DirectoryInfo(appConfig.CacheDir));

var rootCommand = new RootCommand();
rootCommand.AddCommand(new RunCommand(appConfig, downloader));
rootCommand.AddCommand(new CleanCommand(appConfig));
rootCommand.AddCommand(new VMCleanCommand(appConfig));
rootCommand.AddCommand(new RemoveCommand(appConfig));
rootCommand.AddCommand(new RestartCommand(appConfig));
rootCommand.AddCommand(new SshCommand(appConfig));
rootCommand.AddCommand(new ListCommand(appConfig));
rootCommand.AddCommand(new OsCommand(appConfig));
rootCommand.AddCommand(new KekwCommand());

var completionOption = new Option<string?>("--completion", "generate shell completion. (zsh or bash)");
rootCommand.AddOption(completionOption);

rootCommand.SetHandler((complete) =>
{
  if (complete == "zsh")
  {
    var zsh = """
    _VmChamp_zsh_complete()
    {
      local completions=("$(VmChamp '[complete]' "$words")");
      _values = "${(ps:\n:)completions}"
    }

    compdef _VmChamp_zsh_complete VmChamp
    compdef _VmChamp_zsh_complete vmchamp
    """;

    AnsiConsole.WriteLine(zsh);
  }
}, completionOption);

var builder = new CommandLineBuilder(rootCommand).UseDefaults();
var app = builder.Build();

if (args.Length == 0)
{
  return await app.InvokeAsync("--help");
}

if (args.Length > 1 && args[0] == "[complete]")
{
  var parseResult = app.Parse(args[1]);

  var lastToken = parseResult.Tokens.LastOrDefault();
  
  var completions = parseResult.GetCompletions()
    .Where(completion => completion.Label is not "-?" and not "-h" and not "/?" and not "/h")
    .ToArray();

  if (completions.Length == 0)
  {
    Console.WriteLine(lastToken?.Value);
  }

  foreach (var completion in completions)
  {
    Console.WriteLine(completion.Label);
  }

  return 0;
}

if (!Interop.IsLibvirtInstalled())
{
  AnsiConsole.MarkupLine("[red]Libvirt not found.[/]");
  return 1;
}

return await app.InvokeAsync(args);