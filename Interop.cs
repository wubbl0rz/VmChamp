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

  public static LibvirtConnection Create(string target)
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