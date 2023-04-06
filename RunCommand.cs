using System.CommandLine;
using System.CommandLine.Completions;
using System.Diagnostics;
using ByteSizeLib;
using Spectre.Console;

namespace VmChamp;

public class RunCommand : Command
{
  private readonly AppConfig _appConfig;
  private readonly Downloader _downloader;
  private readonly IsoImager _imager;

  public RunCommand(AppConfig appConfig, Downloader downloader) : base("run", "start a new VM")
  {
    _appConfig = appConfig;
    _downloader = downloader;
    _imager = new IsoImager();

    var customCmdOption = new Option<string?>("--cmd",
      "Execute custom shell command after VM start.");

    var memOption = new Option<string>("--mem",
      () => appConfig.DefaultMemorySize,
      "How much memory the VM can use.");

    var diskOption = new Option<string>("--disk",
      () => appConfig.DefaultDiskSize,
      "Disk space available for new VM.");

    var osOption = new Option<string>("--os",
      () => appConfig.DefaultVmDistro,
      "The operating system image to use.");

    var localImageOption = new Option<string?>("--local-image",
      "Provide your own local image in .qcow2 format.");

    var backgroundOption = new Option<bool>("--bg",
      () => false,
      "Create in Background.");

    var cpuOption = new Option<int>("--cpu",
      () => appConfig.DefaultCpuCount,
      "Number of virtual CPUs.");

    var nameArgument = new Argument<string>("name",
      () => appConfig.DefaultVmName,
      "Name of the new VM");

    osOption.AddCompletions((ctx) =>
      DistroInfo.Distros.Select(distro => new CompletionItem(distro.Name)));

    osOption.AddCompletions((ctx) =>
      DistroInfo.Distros.Select(distro => new CompletionItem(distro.Name.ToLower())));

    memOption.AddCompletions((ctx) =>
      new[]
      {
        new CompletionItem("256MiB"),
        new CompletionItem("384MiB"),
        new CompletionItem("512MiB"),
        new CompletionItem("1024MiB"),
        new CompletionItem("2048MiB"),
      });

    diskOption.AddCompletions((ctx) =>
      new[]
      {
        new CompletionItem("4GiB"),
        new CompletionItem("8GiB"),
        new CompletionItem("16GiB"),
        new CompletionItem("32GiB"),
      });

    cpuOption.AddCompletions((ctx) =>
      new[]
      {
        new CompletionItem("1"),
        new CompletionItem("2"),
        new CompletionItem("3"),
        new CompletionItem("4"),
        new CompletionItem("5"),
        new CompletionItem("6"),
      });

    this.AddAlias("start");
    this.AddArgument(nameArgument);
    this.AddOption(osOption);
    this.AddOption(memOption);
    this.AddOption(diskOption);
    this.AddOption(cpuOption);
    this.AddOption(backgroundOption);
    this.AddOption(localImageOption);
    this.AddOption(customCmdOption);

    this.SetHandler(async (os,
        background,
        vmName,
        mem,
        disk,
        cpuCount,
        localImagePath,
        customCmd) =>
      {
        if (!Helper.CheckDefaultNetwork())
          return;

        var distro = localImagePath is null ? 
          this.GetDistro(os) : 
          DistroInfo.CreateLocal(new FileInfo(localImagePath));

        var diskSize = ByteSize.Parse(disk);
        var memSize = ByteSize.Parse(mem);

        if (distro == null)
        {
          AnsiConsole.WriteLine($"OS: {os} not found.");
          return;
        }

        AnsiConsole.MarkupLine($"️👉 Creating VM: {vmName}");
        AnsiConsole.MarkupLine($"💻 Using OS: {distro.Name}");
        AnsiConsole.MarkupLine($"📔 Memory size: {memSize:MiB}");
        AnsiConsole.MarkupLine($"💽 Disk size: {diskSize:GiB}");

        var vmDir = Path.Combine(appConfig.DataDir, vmName);

        if (Directory.Exists(vmDir))
        {
          AnsiConsole.MarkupLine($"[yellow]VM: {vmName} already exists. Do you want to delete and recreate it now?[/]");

          if (AnsiConsole.Ask<string>("Continue? (y/N)", "N").ToLower() != "y")
          {
            return;
          }

          this.RemoveVm(vmName, vmDir);
        }

        Directory.CreateDirectory(vmDir);

        var sourceOsImage = await this._downloader.DownloadAsync(distro);
        var targetOsImage = Path.Combine(vmDir, sourceOsImage.Name);
        sourceOsImage.CopyTo(targetOsImage);

        Helper.ResizeImage(targetOsImage, diskSize.Bytes);

        var initImage = _imager.CreateImage(vmName, new DirectoryInfo(vmDir), customCmd ?? "");

        await this.CreateVm(vmName, new FileInfo(targetOsImage), initImage, background, memSize.Bytes, cpuCount);
      },
      osOption,
      backgroundOption,
      nameArgument,
      memOption,
      diskOption,
      cpuOption,
      localImageOption,
      customCmdOption);
  }

  private DistroInfo? GetDistro(string name) =>
    DistroInfo.Distros
      .FirstOrDefault(distro => distro.Name.ToLower() == name.ToLower());

  private async Task CreateVm(string vmName,
    FileInfo osImage,
    FileInfo initImage,
    bool background,
    double memSize,
    int cpuCount)
  {
    using var libvirtConnection = LibvirtConnection.CreateForSession();

    var xml = this.GenXml(vmName, osImage.FullName, initImage.FullName, memSize, cpuCount);

    var vmId = Interop.virDomainCreateXML(libvirtConnection.NativePtr, xml, 0);

    if (vmId == nint.Zero)
    {
      AnsiConsole.MarkupLine($"[red]Error while creating: {vmName}[/]");
      return;
    }
    
    string? vmIp = null;

    if (background)
    {
      return;
    }
  
    await AnsiConsole.Progress()
      .Columns(new SpinnerColumn(), new TaskDescriptionColumn())
      .StartAsync(async ctx =>
      {
        ctx.AddTask($"Waiting for network...");

        var src = new CancellationTokenSource(TimeSpan.FromSeconds(90));
      
        do
        {
          await Task.Delay(1000);
          vmIp = Interop.GetFirstIpById(vmId);
        } while (vmIp == null && !src.IsCancellationRequested);
      });
    
    if (vmIp == null)
    {
      AnsiConsole.WriteLine("🤔 No network found. Maybe your VM takes too long to boot or network is not ready?");
      return;
    }

    AnsiConsole.WriteLine("🚀 Your VM is ready.");
    AnsiConsole.WriteLine($"IP: {vmIp}");
    AnsiConsole.WriteLine($"Connect with 'VmChamp ssh {_appConfig.DefaultUser}@{vmIp}'");

    Helper.ConnectViaSsh(_appConfig.DefaultUser, vmIp)?.WaitForExit();
  }

  private void RemoveVm(string vmName, string vmDir)
  {
    using var libvirtConnection = LibvirtConnection.CreateForSession();

    var vmId = Interop.virDomainLookupByName(libvirtConnection.NativePtr, vmName);

    Interop.DestroyVm(vmId, vmName, vmDir);
  }

  private string GenXml(string vmName, string diskImage, string initImage, double memSizeInBytes, int cpuCount)
  {
    var guid = Guid.NewGuid();
    var xml = $"""
      <domain type="kvm">
        <name>{vmName}</name>
        <uuid>{guid}</uuid>
        <metadata>
          <libosinfo:libosinfo xmlns:libosinfo="http://libosinfo.org/xmlns/libvirt/domain/1.0">
            <libosinfo:os id="http://libosinfo.org/linux/2020"/>
          </libosinfo:libosinfo>
        </metadata>
        <memory unit="B">{memSizeInBytes}</memory>
        <vcpu placement="static">{cpuCount}</vcpu>
        <os>
          <type arch="x86_64" machine="pc-q35-7.2">hvm</type>
          <boot dev="hd"/>
        </os>
        <features>
          <acpi/>
          <apic/>
          <vmport state="off"/>
        </features>
        <cpu mode="host-model" check="partial"/>
        <on_poweroff>destroy</on_poweroff>
        <on_reboot>restart</on_reboot>
        <on_crash>destroy</on_crash>
        <devices>
          <emulator>/usr/bin/qemu-system-x86_64</emulator>
          <disk type="file" device="disk">
            <driver name="qemu" type="qcow2"/>
            <source file="{diskImage}"/>
            <target dev="vda" bus="virtio"/>
            <address type="pci" domain="0x0000" bus="0x04" slot="0x00" function="0x0"/>
          </disk>
          <channel type='unix'>
             <target type='virtio' name='org.qemu.guest_agent.0'/>
          </channel>
          <disk type="file" device="disk">
            <driver name="qemu" type="raw"/>
            <source file="{initImage}" index="1"/>
            <backingStore/>
            <target dev="vdb" bus="virtio"/>
            <alias name="virtio-disk1"/>
            <address type="pci" domain="0x0000" bus="0x05" slot="0x00" function="0x0"/>
          </disk>
          <console type='pty'>
            <target type='serial' port='0'/>
          </console>
          <interface type="bridge">
            <source bridge="virbr0"/>
            <model type="virtio"/>
            <address type="pci" domain="0x0000" bus="0x01" slot="0x00" function="0x0"/>
          </interface>
          <input type="keyboard" bus="ps2"/>
          <graphics type="spice" autoport="yes" listen="localhost">
            <listen type="address" address="localhost"/>
            <image compression="off"/>
          </graphics>
          <video>
            <model type="virtio" heads="1" primary="yes"/>
            <address type="pci" domain="0x0000" bus="0x00" slot="0x01" function="0x0"/>
          </video>
          <redirdev bus="usb" type="spicevmc">
            <address type="usb" bus="0" port="2"/>
          </redirdev>
          <redirdev bus="usb" type="spicevmc">
            <address type="usb" bus="0" port="3"/>
          </redirdev>
          <watchdog model="itco" action="reset"/>
          <memballoon model="virtio">
            <address type="pci" domain="0x0000" bus="0x06" slot="0x00" function="0x0"/>
          </memballoon>
          <rng model="virtio">
            <backend model="random">/dev/urandom</backend>
            <address type="pci" domain="0x0000" bus="0x07" slot="0x00" function="0x0"/>
          </rng>
        </devices>
      </domain>
      """;

    return xml;
  }
}