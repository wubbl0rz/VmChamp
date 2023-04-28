using System.Runtime.InteropServices;
using Spectre.Console;

namespace VmChamp;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct VirDomainIpAddress
{
  public int type;
  [MarshalAs(UnmanagedType.LPUTF8Str)] public byte* addr;
  public uint prefix;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct VirDomainInterface
{
  public byte* name;
  public byte* hwaddr;
  public uint naddrs;
  public VirDomainIpAddress* addrs;
}

[StructLayout(LayoutKind.Sequential)]
public struct VirDomainInfo
{
  public byte state;
  public long maxMem;
  public long memory;
  public ushort nrVirtCpu;
  public ulong cpuTime;
}

public struct VirDomainBlockInfo
{
  public ulong capacity;
  public ulong allocation;
  public ulong physical;
}

enum VirDomainState
{
  NoState = 0,
  Running = 1,
  Blocked = 2,
  Paused = 3,
  Shutdown = 4,
  Shutoff = 5,
  Crashed = 6,
  Suspended = 7,
  Last = 8
}

public static unsafe class Interop
{
  [DllImport("libvirt.so.0",
    CallingConvention = CallingConvention.Cdecl,
    EntryPoint = "virDomainInterfaceAddresses")]
  public static extern nint virDomainInterfaceAddresses(IntPtr dom,
    VirDomainInterface*** ifaces,
    uint source,
    uint flags = 0);

  [DllImport("libvirt.so.0",
    CallingConvention = CallingConvention.Cdecl,
    EntryPoint = "virDomainCreateXML")]
  public static extern nint virDomainCreateXML(IntPtr conn,
    string xmlDesc,
    uint flags);

  [DllImport("libvirt.so.0",
    CallingConvention = CallingConvention.Cdecl,
    EntryPoint = "virConnectOpen")]
  public static extern nint virConnectOpen(string name);

  [DllImport("libvirt.so.0",
    CallingConvention = CallingConvention.Cdecl,
    EntryPoint = "virConnectClose")]
  public static extern nint virConnectClose(nint conn);

  [DllImport("libvirt.so.0",
    CallingConvention = CallingConvention.Cdecl,
    EntryPoint = "virDomainLookupByName")]
  public static extern nint virDomainLookupByName(nint conn, string name);

  [DllImport("libvirt.so.0",
    CallingConvention = CallingConvention.Cdecl,
    EntryPoint = "virDomainDestroy")]
  public static extern int virDomainDestroy(nint domain);

  [DllImport("libvirt.so.0",
    CallingConvention = CallingConvention.Cdecl,
    EntryPoint = "virDomainGetInfo")]
  public static extern int virDomainGetInfo(nint domain, VirDomainInfo* info);

  [DllImport("libvirt.so.0",
    CallingConvention = CallingConvention.Cdecl,
    EntryPoint = "virDomainGetBlockInfo")]
  public static extern int virDomainGetBlockInfo(nint domain, string disk, VirDomainBlockInfo* info, uint flags = 0);

  [DllImport("libvirt.so.0",
    CallingConvention = CallingConvention.Cdecl,
    EntryPoint = "virDomainReset")]
  public static extern int virDomainReset(nint domain, uint flags = 0);

  [DllImport("libvirt.so.0",
    CallingConvention = CallingConvention.Cdecl,
    EntryPoint = "virNetworkCreateXML")]
  public static extern nint virNetworkCreateXML(nint conn, string xml);

  [DllImport("libvirt.so.0",
    CallingConvention = CallingConvention.Cdecl,
    EntryPoint = "virNetworkLookupByName")]
  public static extern nint virNetworkLookupByName(nint conn, string name);

  [DllImport("libvirt.so.0",
    CallingConvention = CallingConvention.Cdecl,
    EntryPoint = "virNetworkDestroy")]
  public static extern int virNetworkDestroy(nint network);

  public static string? GetFirstIpById(nint id)
  {
    VirDomainInterface** ifaces = null;

    var n = Interop.virDomainInterfaceAddresses(id, &ifaces, 2);

    if (n <= 0 || ifaces == null)
      return null;

    var virDomainIpAddress = ifaces[0]->addrs[0];
    var ip = Marshal.PtrToStringUTF8((nint)virDomainIpAddress.addr);
    return ip;
  }

  public static bool IsLibvirtInstalled()
  {
    NativeLibrary.TryLoad("libvirt.so.0", out var handle);
    
    return handle != IntPtr.Zero;
  }

  public static void DestroyVm(nint vmId, string vmName, string vmDir)
  {
    AnsiConsole.MarkupLine($"[yellow]💀 Removing VM: {vmName}[/]");
    
    if(Interop.virDomainDestroy(vmId) != 0)
    {
      AnsiConsole.MarkupLine($"[red]Error removing VM: {vmName}[/]");
    }
    
    if (Directory.Exists(vmDir))
    {
      Directory.Delete(vmDir, true);
    }
  }

  public static string DefaultNetworkDefiniton = """
    <network>
      <name>default</name>
      <bridge name='virbr0'/>
      <forward/>
      <ip address='192.168.122.1' netmask='255.255.255.0'>
        <dhcp>
          <range start='192.168.122.128' end='192.168.122.254'/>
        </dhcp>
      </ip>
    </network>
    """;
}

public class LibvirtConnection : IDisposable
{
  private readonly nint _ptr;
  public nint NativePtr => _ptr;
  public bool IsValid => this._ptr != nint.Zero;

  private LibvirtConnection(nint ptr)
  {
    _ptr = ptr;
  }

  public static LibvirtConnection CreateForSession()
  {
    return Create("qemu:///session");
  }

  public static LibvirtConnection CreateForSystem()
  {
    return Create("qemu:///system");
  }

  private static LibvirtConnection Create(string target)
  {
    var ptr = Interop.virConnectOpen(target);

    if (ptr == nint.Zero)
    {
      throw new ArgumentException($"Cannot connect to: {target}");
    }

    return new LibvirtConnection(ptr);
  }

  public void Dispose()
  {
    if (this.IsValid)
    {
      Interop.virConnectClose(_ptr);
    }
  }
}