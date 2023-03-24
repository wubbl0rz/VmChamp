using System.CommandLine;
using Spectre.Console;
using VmChamp;

public class OsCommand : Command
{
  private readonly AppConfig _appConfig;

  public OsCommand(AppConfig appConfig) : base("os", "get a list of all available os images")
  {
    _appConfig = appConfig;
    
    this.AddAlias("images");
    
    this.SetHandler(() =>
    {
      var table = new Table();
      table.AddColumn("OS");
      table.AddColumn("Alias");
      table.AddColumn("Image");
      table.AddColumn("Url");
  
      foreach (var distro in DistroInfo.Distros)
      {
        table.AddRow(distro.Name, 
          string.Join(", ", distro.Aliases), 
          distro.ImageName,
          distro.Url);
      }

      AnsiConsole.Write(table);
    });
  }
}