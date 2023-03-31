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
      ImageName = "debian-9-genericcloud-amd64.qcow2",
      Url = "https://cloud.debian.org/images/cloud/stretch/latest/",
      Aliases = new[] { "Stretch" }
    },
    new()
    {
      Name = "Ubuntu1804",
      Family = "Ubuntu",
      ImageName = "bionic-server-cloudimg-amd64.img",
      Url = "https://cloud-images.ubuntu.com/bionic/current/",
      Aliases = new[] { "Bionic Beaver", "Bionic" }
    },
    new()
    {
      Name = "Ubuntu2004",
      Family = "Ubuntu",
      ImageName = "focal-server-cloudimg-amd64.img",
      Url = "https://cloud-images.ubuntu.com/focal/current/",
      Aliases = new[] { "Focal Fossal", "Focal" }
    },
    new()
    {
      Name = "Ubuntu2204",
      Family = "Ubuntu",
      ImageName = "focal-server-cloudimg-amd64.img",
      Url = "https://cloud-images.ubuntu.com/jammy/current/",
      Aliases = new[] { "Jammy Jellyfish", "Jammy" }
    },
    new()
    {
      Name = "Arch",
      Family = "Arch",
      ImageName = "Arch-Linux-x86_64-cloudimg.qcow2",
      Url = "https://geo.mirror.pkgbuild.com/images/latest/",
      Aliases = Array.Empty<string>()
    },
    new()
    {
      Name = "Fedora36",
      Family = "Fedora",
      ImageName = "Fedora-Cloud-Base-36-1.5.x86_64.qcow2",
      Url = "https://download.fedoraproject.org/pub/fedora/linux/releases/37/Cloud/x86_64/images/",
      Aliases = Array.Empty<string>()
    },
    new()
    {
      Name = "Fedora37",
      Family = "Fedora",
      ImageName = "Fedora-Cloud-Base-37-1.7.x86_64.qcow2",
      Url = "https://download.fedoraproject.org/pub/fedora/linux/releases/37/Cloud/x86_64/images/",
      Aliases = Array.Empty<string>()
    },
    new()
    {
      Name = "CentOS7",
      Family = "RHEL",
      ImageName = "CentOS-7-x86_64-GenericCloud.qcow2",
      Url = "https://cloud.centos.org/centos/7/images/",
      Aliases = Array.Empty<string>()
    },
    new()
    {
      Name = "Alma8",
      Family = "RHEL",
      ImageName = "AlmaLinux-8-GenericCloud-latest.x86_64.qcow2",
      Url = "https://repo.almalinux.org/almalinux/8/cloud/x86_64/images/",
      Aliases = Array.Empty<string>()
    },
    new()
    {
      Name = "Alma9",
      Family = "RHEL",
      ImageName = "AlmaLinux-9-GenericCloud-latest.x86_64.qcow2",
      Url = "https://repo.almalinux.org/almalinux/9/cloud/x86_64/images/",
      Aliases = Array.Empty<string>()
    }
  };

  public static DistroInfo? CreateLocal(FileInfo localImage)
  {
    if (!localImage.Exists)
    {
      return null;
    }

    var localImageDirectory = localImage.Directory;

    return new DistroInfo()
    {
      Name = "local",
      Aliases = Array.Empty<string>(),
      Family = "local",
      Url = "file://" + localImageDirectory?.FullName,
      ImageName = localImage.Name
    };
  }

  public required string Family { get; set; }
  public required string Name { get; set; }
  public required string ImageName { get; set; }
  public required string Url { get; set; }
  public required string[] Aliases { get; set; }
}