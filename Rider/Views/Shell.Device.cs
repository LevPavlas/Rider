using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using System.Collections;
using CefSharp.Enums;
using System.Diagnostics;
using System.IO;
using static Rider.Views.Shell;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace Rider.Views
{
	//public class DeviceEventRegistration : IDisposable
	//{
	//	private const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;
	//	private const int DBT_DEVTYP_HANDLE = 0x00000006;
	//	private const int DBT_DEVTYP_OEM = 0x00000000;
	//	private const int DBT_DEVTYP_PORT = 0x00000003;
	//	private const int DBT_DEVTYP_VOLUME = 0x00000002;

	//	private static readonly Guid GUID_DEVINTERFACE_USB_DEVICE = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");

	//	private static readonly Guid GUID_BTHPORT_DEVICE_INTERFACE= new Guid("0850302A-B344-4fda-9BE9-90576B8D46F0");
	//	private static readonly Guid GUID_BTH_DEVICE_INTERFACE = new Guid("00F40965-E89D-4487-9890-87C3ABB211F4");
	//	private static readonly Guid GUID_BLUETOOTHLE_DEVICE_INTERFACE = new Guid("781aee18-7733-4ce4-add0-91f41c67b592");


	//	/*

	//		you need to intercept WM_DEVICECHANGE messages (obviously)
	//		you first need to 'Register' the proper GUIDs:
	//		GUID_BTHPORT_DEVICE_INTERFACE {0850302A-B344-4fda-9BE9-90576B8D46F0} to intercept events about the Radio itself
	//		GUID_BTH_DEVICE_INTERFACE {00F40965-E89D-4487-9890-87C3ABB211F4} to intercept events about any Bluetooth devices and/or
	//		GUID_BLUETOOTHLE_DEVICE_INTERFACE {781aee18-7733-4ce4-add0-91f41c67b592} to intercept events about BLE devices.
	//	 * 
	//	 * 
	//	 */

	//	[DllImport("setupapi.dll", CharSet = CharSet.Auto)]
	//	static extern IntPtr SetupDiGetClassDevs(
	//											ref Guid ClassGuid,
	//										  [MarshalAs(UnmanagedType.LPTStr)] string Enumerator,
	//										  IntPtr hwndParent,
	//										  uint Flags
	//										 );
	//	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	//	public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr notificationFilter, uint flags);
	//	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	//	public static extern uint UnregisterDeviceNotification(IntPtr hHandle);

	//	private readonly IntPtr _windowHandle;
	//	private IntPtr _notificationHandle = IntPtr.Zero;

	//	public bool IsRegistered => _notificationHandle != IntPtr.Zero;

	//	public DeviceEventRegistration(IntPtr windowHandle)
	//	{
	//		_windowHandle = windowHandle;
	//	}

	//	public void Register()
	//	{
	//		DEV_BROADCAST_DEVICE_INTERFACE dbdi = new DEV_BROADCAST_DEVICE_INTERFACE
	//		{
	//			DeviceType = DBT_DEVTYP_DEVICEINTERFACE,
	//			Reserved = 0,
	//			ClassGuid = GUID_BTHPORT_DEVICE_INTERFACE,
	//			Name = 0,
	//		};
	//		dbdi.Size = Marshal.SizeOf(dbdi);
	//		IntPtr buffer = Marshal.AllocHGlobal(dbdi.Size);
	//		Marshal.StructureToPtr(dbdi, buffer, true);
			
	//		_notificationHandle = RegisterDeviceNotification( _windowHandle, buffer, 0);
	//	}

	//	// Call on window unload.
	//	public void Dispose()
	//	{
	//		UnregisterDeviceNotification(_notificationHandle);
	//	}
	//}

	public partial class Shell
	{
		public const int WM_DEVICECHANGE = 0x00000219;
		
		public const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;
		public const int DBT_DEVTYP_HANDLE = 0x00000006;
		public const int DBT_DEVTYP_OEM = 0x00000000;
		public const int DBT_DEVTYP_PORT = 0x00000003;
		public const int DBT_DEVTYP_VOLUME = 0x00000002;



		public const int DBT_DEVICEARRIVAL = 0x00008000;
		public const int DBT_DEVICEREMOVECOMPLETE = 0x00008004;
		public const int DBT_DEVNODES_CHANGED = 0x00000007;

		private const ushort DBTF_MEDIA = 0x0001; // Media in drive changed.
		private const ushort DBTF_NET = 0x0002; // Network drive is changed.

		[StructLayout(LayoutKind.Sequential)]
		internal class DEV_BROADCAST_HDR
		{
			public int dbch_size;
			public int dbch_devicetype;
			public int dbch_reserved;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DEV_BROADCAST_DEVICEINTERFACE
		{
			public int dbcc_size;
			public int dbcc_devicetype;
			public int dbcc_reserved;
			[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
			public byte[] dbcc_classguid;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public char[] dbcc_name;
		}

		private enum VolumeType : ushort
		{
			Other = 0,
			Media = DBTF_MEDIA,
			Net = DBTF_NET,
		}
		// https://www.pinvoke.net/default.aspx/Structures.DEV_BROADCAST_DEVICEINTERFACE
		[StructLayout(LayoutKind.Sequential)]
		public struct DEV_BROADCAST_DEVICE_INTERFACE
		{
			public int Size;
			public int DeviceType;
			public int Reserved;
			public Guid ClassGuid;
			public short Name;
		}
		[StructLayoutAttribute(LayoutKind.Sequential)]
		private struct DEV_BROADCAST_VOLUME
		{
			public uint dbcv_size;
			public uint dbcv_devicetype;
			public uint dbcv_reserved;
			public uint dbcv_unitmask;
			public ushort dbcv_flags;
		}

//		private DeviceEventRegistration? _usbEventRegistration;
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			HwndSource? source = PresentationSource.FromVisual(this) as HwndSource;
			source?.AddHook(WndProc);
			//IMO this should be abstracted away from the code-behind.





			//var windowSource = (HwndSource)PresentationSource.FromVisual(this);
			//_usbEventRegistration = new DeviceEventRegistration(windowSource.Handle);
			//// This will allow your window to receive USB events.
			//_usbEventRegistration.Register();
			//// This hook is what we were aiming for. All Windows events are listened to here. We can inject our own listeners.
			//windowSource.AddHook(WndProc);
		}
		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			// Here's where the help ends. Do what you need here.
			// Get additional info from http://www.pinvoke.net/
			// USB event message is msg == 0x0219 (WM_DEVICECHANGE).
			// USB event plugin is wParam == 0x8000 (DBT_DEVICEARRIVAL).
			// USB event unplug is wParam == 0x8004 (DBT_DEVICEREMOVECOMPLETE).
			// Your device info is in lParam. Filter that.
			// You need to convert wParam/lParam to Int32 using Marshal.

			if (msg == WM_DEVICECHANGE) //Device state has changed
			{
	//				Console.WriteLine($"msg:{msg.ToString("X")}, wParam:{wParam.ToString("X")}, lParam:{lParam.ToString("X")}");

				switch (wParam.ToInt32())
				{
					case DBT_DEVICEARRIVAL:
						Console.WriteLine("Device connected");
						break;
					case DBT_DEVICEREMOVECOMPLETE:
						Console.WriteLine("Device disconnected");
						break;
					default:
						return IntPtr.Zero;
				}

				DEV_BROADCAST_HDR? hdr = Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_HDR)) as DEV_BROADCAST_HDR;
				if (hdr != null)
				{
					DEV_BROADCAST_VOLUME volume = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_VOLUME));

					int[] driveIndices = FindDriveIndices(volume.dbcv_unitmask).Take(26).ToArray();
					var driveLetters = ConvertIndicesToLetters(driveIndices);
					var volumeType = (VolumeType)Enum.ToObject(typeof(VolumeType), volume.dbcv_flags);

					Console.WriteLine($"Volume changed. Drive: {String.Join(",", driveLetters)} Type: {volumeType}");
					}
			}


			//if (msg == WM_DEVICECHANGE) //Device state has changed
			//{
			//	switch (wParam.ToInt32())
			//	{
			//		case DBT_DEVICEARRIVAL: //New device arrives
			//			DEV_BROADCAST_HDR? hdr = Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_HDR)) as DEV_BROADCAST_HDR; ;
				
			//			if (hdr?.dbch_devicetype == DBT_DEVTYP_VOLUME) //If it is a USB Mass Storage or Hard Drive
			//			{
			//				DEV_BROADCAST_VOLUME volume = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_VOLUME));

			//				int[] driveIndices = FindDriveIndices(volume.dbcv_unitmask).Take(26).ToArray();
			//				var driveLetters = ConvertIndicesToLetters(driveIndices);
			//				var volumeType = (VolumeType)Enum.ToObject(typeof(VolumeType), volume.dbcv_flags);

			//				Console.WriteLine($"Volume changed. Drive:{String.Join(",", driveLetters)} Type:{volumeType}");

			//				var fak = DriveInfo.GetDrives();
			//				//Save Device name
			//				DEV_BROADCAST_DEVICEINTERFACE deviceInterface;
			//				string deviceName = "";
			//				deviceInterface = (DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_DEVICEINTERFACE));
			//				deviceName = new string(deviceInterface.dbcc_name).Trim();
			//				Console.WriteLine(deviceName);
			//			}
			//			break;
			//	}
			//}

			//if (msg == 0x0219)
			//{
			//	Console.WriteLine($"msg:{msg.ToString("X")}, wParam:{wParam.ToString("X")}, lParam:{lParam.ToString("X")}");
			//}
			return IntPtr.Zero;
		}

		private static IEnumerable<int> FindDriveIndices(uint value)
		{
			return new BitArray(new[] { (int)value }) // Up to 31 drive indices can be accepted.
			  .Cast<bool>()
			  .Select((x, index) => x ? index : -1)
			  .Where(x => x >= 0);
		}

		private static IEnumerable<char> ConvertIndicesToLetters(int[] indices)
		{
			return Enumerable.Range('A', 'Z' - 'A' + 1)
			  .Select((x, index) => new { Letter = (char)x, Index = index })
			  .Where(x => indices.Contains(x.Index))
			  .Select(x => x.Letter);
		}

	}
}
