namespace VmChamp;

public class DistroInfo
{
  public static IEnumerable<DistroInfo> Distros { get; set; } = new List<DistroInfo>()
  {
    new()
    {
      Name = "Debian12",
      Family = "Debian",
      ImageName = "debian-12-genericcloud-amd64-daily.qcow2",
      Url = "https://cloud.debian.org/images/cloud/bookworm/daily/latest/",
      Aliases = new[] { "Bookworm" },
      ChecksumFile = "SHA512SUMS",
      ChecksumType = "sha512"
    },
    new()
    {
      Name = "Debian11",
      Family = "Debian",
      ImageName = "debian-11-genericcloud-amd64.qcow2",
      Url = "https://cloud.debian.org/images/cloud/bullseye/latest/",
      Aliases = new[] { "Bullseye" },
      ChecksumFile = "SHA512SUMS",
      ChecksumType = "sha512"
    },
    new()
    {
      Name = "Debian10",
      Family = "Debian",
      ImageName = "debian-10-genericcloud-amd64.qcow2",
      Url = "https://cloud.debian.org/images/cloud/buster/latest/",
      Aliases = new[] { "Buster" },
      ChecksumFile = "SHA512SUMS",
      ChecksumType = "sha512"
    },
    new()
    {
      Name = "Ubuntu1804",
      Family = "Ubuntu",
      ImageName = "bionic-server-cloudimg-amd64.img",
      Url = "https://cloud-images.ubuntu.com/bionic/current/",
      Aliases = new[] { "Bionic Beaver", "Bionic" },
      ChecksumFile = "SHA256SUMS",
      ChecksumType = "sha256"
    },
    new()
    {
      Name = "Ubuntu2004",
      Family = "Ubuntu",
      ImageName = "focal-server-cloudimg-amd64.img",
      Url = "https://cloud-images.ubuntu.com/focal/current/",
      Aliases = new[] { "Focal Fossal", "Focal" },
      ChecksumFile = "SHA256SUMS",
      ChecksumType = "sha256"
    },
    new()
    {
      Name = "Ubuntu2204",
      Family = "Ubuntu",
      ImageName = "jammy-server-cloudimg-amd64.img",
      Url = "https://cloud-images.ubuntu.com/jammy/current/",
      Aliases = new[] { "Jammy Jellyfish", "Jammy" },
      ChecksumFile = "SHA256SUMS",
      ChecksumType = "sha256"
    },
    new()
    {
      Name = "Ubuntu2304",
      Family = "Ubuntu",
      ImageName = "lunar-server-cloudimg-amd64.img",
      Url = "https://cloud-images.ubuntu.com/lunar/current/",
      Aliases = new[] { "Lunar Lobster", "Lunar" },
      ChecksumFile = "SHA256SUMS",
      ChecksumType = "sha256"
    },
    new()
    {
      Name = "Arch",
      Family = "Arch",
      ImageName = "Arch-Linux-x86_64-cloudimg.qcow2",
      Url = "https://geo.mirror.pkgbuild.com/images/latest/",
      Aliases = Array.Empty<string>(),
      ChecksumFile = "Arch-Linux-x86_64-cloudimg.qcow2.SHA256",
      ChecksumType = "sha256"
    },
    new()
    {
      Name = "Fedora36",
      Family = "Fedora",
      ImageName = "Fedora-Cloud-Base-36-1.5.x86_64.qcow2",
      Url = "https://download.fedoraproject.org/pub/fedora/linux/releases/36/Cloud/x86_64/images/",
      Aliases = Array.Empty<string>(),
      ChecksumFile = "Fedora-Cloud-36-1.5-x86_64-CHECKSUM",
      ChecksumType = "sha256"
    },
    new()
    {
      Name = "Fedora37",
      Family = "Fedora",
      ImageName = "Fedora-Cloud-Base-37-1.7.x86_64.qcow2",
      Url = "https://download.fedoraproject.org/pub/fedora/linux/releases/37/Cloud/x86_64/images/",
      Aliases = Array.Empty<string>(),
      ChecksumFile = "Fedora-Cloud-37-1.7-x86_64-CHECKSUM",
      ChecksumType = "sha256"
    },
    new()
    {
      Name = "CentOS7",
      Family = "RHEL",
      ImageName = "CentOS-7-x86_64-GenericCloud.qcow2",
      Url = "https://cloud.centos.org/centos/7/images/",
      Aliases = Array.Empty<string>(),
      ChecksumFile = "sha256sum.txt",
      ChecksumType = "sha256"
    },
    new()
    {
      Name = "Rocky8",
      Family = "RHEL",
      ImageName = "Rocky-8-GenericCloud.latest.x86_64.qcow2",
      Url = "https://download.rockylinux.org/pub/rocky/8/images/x86_64/",
      Aliases = Array.Empty<string>(),
      ChecksumFile = "CHECKSUM",
      ChecksumType = "sha256"
    },
    new()
    {
      Name = "Rocky9",
      Family = "RHEL",
      ImageName = "Rocky-9-GenericCloud.latest.x86_64.qcow2",
      Url = "https://download.rockylinux.org/pub/rocky/9/images/x86_64/",
      Aliases = Array.Empty<string>(),
      ChecksumFile = "CHECKSUM",
      ChecksumType = "sha256"
    },
    new()
    {
      Name = "Alma8",
      Family = "RHEL",
      ImageName = "AlmaLinux-8-GenericCloud-latest.x86_64.qcow2",
      Url = "https://repo.almalinux.org/almalinux/8/cloud/x86_64/images/",
      Aliases = Array.Empty<string>(),
      ChecksumFile = "CHECKSUM",
      ChecksumType = "sha256"
    },
    new()
    {
      Name = "Alma9",
      Family = "RHEL",
      ImageName = "AlmaLinux-9-GenericCloud-latest.x86_64.qcow2",
      Url = "https://repo.almalinux.org/almalinux/9/cloud/x86_64/images/",
      Aliases = Array.Empty<string>(),
      ChecksumFile = "CHECKSUM",
      ChecksumType = "sha256"
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
      Family = "local",
      ImageName = localImage.Name,
      Url = "file://" + localImageDirectory?.FullName,
      Aliases = Array.Empty<string>(),
      ChecksumFile = "",
      ChecksumType = ""
    };
  }

  public required string Family { get; set; }
  public required string Name { get; set; }
  public required string ImageName { get; set; }
  public required string Url { get; set; }
  public required string[] Aliases { get; set; }
  public required string ChecksumFile { get; set; }
  public required string ChecksumType { get; set; }
}
