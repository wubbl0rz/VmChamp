using System.Text;
using DiscUtils.Iso9660;

public class IsoImager
{
  private IEnumerable<string> FindSshKeys()
  {
    var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    var keys = new List<string>();

    foreach (var file in Directory.EnumerateFiles(Path.Combine(homeDir, ".ssh")))
    {
      if (file.EndsWith(".pub"))
      {
        keys.Add(File.ReadAllText(file));
      }
    }

    return keys;
  }

  public FileInfo CreateImage(string hostname, DirectoryInfo outputDirectory)
  {
    var keys = "[" + string.Join(",", this.FindSshKeys().Select(key => $"\"{key.Trim()}\"")) + "]";

    var userData = $"""
      Content-Type: multipart/mixed; boundary="==BOUNDARY=="
      MIME-Version: 1.0
      --==BOUNDARY==
      Content-Type: text/cloud-config; charset="us-ascii"

      #cloud-config

      preserve_hostname: False
      hostname: {hostname}
      users:
          - default
          - name: user
            groups: ['sudo']
            shell: /bin/bash
            sudo: ALL=(ALL) NOPASSWD:ALL
            ssh-authorized-keys:
              {keys}
      output:
        all: ">> /var/log/cloud-init.log"
      ssh_genkeytypes: ['ed25519', 'rsa']
      ssh_authorized_keys:
        {keys}
      runcmd:
        - systemctl stop networking && systemctl start networking
        - systemctl disable cloud-init.service

      --==BOUNDARY==--
      """;

    var metaData = $"""
      instance-id: {hostname}
      local-hostname: {hostname}
      """;

    var builder = new CDBuilder();
    builder.UseJoliet = true;
    builder.VolumeIdentifier = "cidata";
    builder.AddFile(@"user-data", Encoding.UTF8.GetBytes(userData));
    builder.AddFile(@"meta-data", Encoding.UTF8.GetBytes(metaData));

    var outputFile = Path.Combine(outputDirectory.FullName, "cloudInit.iso");
    
    builder.Build(outputFile);
    
    return new FileInfo(outputFile);
  }
}