namespace VmChamp;

public class DistroInfo
{
  public static IEnumerable<DistroInfo> Distros { get; set; } = new List<DistroInfo>()
  {
    new()
    {
      Name = "Debian11",
      Family = "Debian",
      ImageName = "debian-11-genericcloud-amd64.qcow2",
      Url = "https://cloud.debian.org/images/cloud/bullseye/latest/",
      Aliases = new[] { "Bullseye" }
    },
    new()
    {
      Name = "Debian10",
      Family = "Debian",
      ImageName = "debian-10-genericcloud-amd64.qcow2",
      Url = "https://cloud.debian.org/images/cloud/buster/latest/",
      Aliases = new[] { "Buster" }
    },
    new()
    {
      Name = "Debian9",
      Family = "Debian",
      ImageName = "debian-10-genericcloud-amd64.qcow2",
      Url = "https://cloud.debian.org/images/cloud/stretch/latest/",
      Aliases = new[] { "Stretch" }
    }
  };

  public required string Family { get; set; }
  public required string Name { get; set; }
  public required string ImageName { get; set; }
  public required string Url { get; set; }
  public required string[] Aliases { get; set; }
}