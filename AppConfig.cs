namespace VmChamp;

public class AppConfig
{
  public string AppDir { get; private set; }
  public string CacheDir { get; private set; }
  public string DataDir { get; private set; }
  public string DefaultVmName { get; set; } = "testvm";
  public string DefaultVmDistro { get; set; } = "Debian11";
  public string DefaultUser { get; set; } = "user";
  public string DefaultMemorySize { get; set; } = "256MiB";
  public string DefaultDiskSize { get; set; } = "4GiB";
  public int DefaultCpuCount { get; set; } = 1;

  public AppConfig(string appName = "VmChamp", string sessionName = "default")
  {
    var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    this.AppDir = Path.Combine(homeDir, appName);
    this.CacheDir = Path.Combine(this.AppDir, sessionName, "cache");
    this.DataDir = Path.Combine(this.AppDir, sessionName, "vms");
  }
}