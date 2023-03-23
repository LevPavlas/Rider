using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Services
{
	internal partial class BluetoothLowEnergy
	{
		public enum CredUIReturnCodes
		{
			NO_ERROR = 0,
			ERROR_CANCELLED = 1223,
			ERROR_NO_SUCH_LOGON_SESSION = 1312,
			ERROR_NOT_FOUND = 1168,
			ERROR_INVALID_ACCOUNT_NAME = 1315,
			ERROR_INSUFFICIENT_BUFFER = 122,
			ERROR_INVALID_PARAMETER = 87,
			ERROR_INVALID_FLAGS = 1004,
			ERROR_BAD_ARGUMENTS = 160
		}
		enum SetupDiGetDeviceRegistryPropertyEnum : uint
		{
			SPDRP_DEVICEDESC = 0x00000000, // DeviceDesc (R/W)
			SPDRP_HARDWAREID = 0x00000001, // HardwareID (R/W)
			SPDRP_COMPATIBLEIDS = 0x00000002, // CompatibleIDs (R/W)
			SPDRP_UNUSED0 = 0x00000003, // unused
			SPDRP_SERVICE = 0x00000004, // Service (R/W)
			SPDRP_UNUSED1 = 0x00000005, // unused
			SPDRP_UNUSED2 = 0x00000006, // unused
			SPDRP_CLASS = 0x00000007, // Class (R--tied to ClassGUID)
			SPDRP_CLASSGUID = 0x00000008, // ClassGUID (R/W)
			SPDRP_DRIVER = 0x00000009, // Driver (R/W)
			SPDRP_CONFIGFLAGS = 0x0000000A, // ConfigFlags (R/W)
			SPDRP_MFG = 0x0000000B, // Mfg (R/W)
			SPDRP_FRIENDLYNAME = 0x0000000C, // FriendlyName (R/W)
			SPDRP_LOCATION_INFORMATION = 0x0000000D, // LocationInformation (R/W)
			SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0x0000000E, // PhysicalDeviceObjectName (R)
			SPDRP_CAPABILITIES = 0x0000000F, // Capabilities (R)
			SPDRP_UI_NUMBER = 0x00000010, // UiNumber (R)
			SPDRP_UPPERFILTERS = 0x00000011, // UpperFilters (R/W)
			SPDRP_LOWERFILTERS = 0x00000012, // LowerFilters (R/W)
			SPDRP_BUSTYPEGUID = 0x00000013, // BusTypeGUID (R)
			SPDRP_LEGACYBUSTYPE = 0x00000014, // LegacyBusType (R)
			SPDRP_BUSNUMBER = 0x00000015, // BusNumber (R)
			SPDRP_ENUMERATOR_NAME = 0x00000016, // Enumerator Name (R)
			SPDRP_SECURITY = 0x00000017, // Security (R/W, binary form)
			SPDRP_SECURITY_SDS = 0x00000018, // Security (W, SDS form)
			SPDRP_DEVTYPE = 0x00000019, // Device Type (R/W)
			SPDRP_EXCLUSIVE = 0x0000001A, // Device is exclusive-access (R/W)
			SPDRP_CHARACTERISTICS = 0x0000001B, // Device Characteristics (R/W)
			SPDRP_ADDRESS = 0x0000001C, // Device Address (R)
			SPDRP_UI_NUMBER_DESC_FORMAT = 0X0000001D, // UiNumberDescFormat (R/W)
			SPDRP_DEVICE_POWER_DATA = 0x0000001E, // Device Power Data (R)
			SPDRP_REMOVAL_POLICY = 0x0000001F, // Removal Policy (R)
			SPDRP_REMOVAL_POLICY_HW_DEFAULT = 0x00000020, // Hardware Removal Policy (R)
			SPDRP_REMOVAL_POLICY_OVERRIDE = 0x00000021, // Removal Policy Override (RW)
			SPDRP_INSTALL_STATE = 0x00000022, // Device Install State (R)
			SPDRP_LOCATION_PATHS = 0x00000023, // Device Location Paths (R)
			SPDRP_BASE_CONTAINERID = 0x00000024  // Base ContainerID (R)
		}


		//	[PInvokeData("Devpropdef.h")]
		[StructLayout(LayoutKind.Sequential)]
		public struct DEVPROPKEY
		{
			/// <summary>
			/// <para>A DEVPROPGUID-typed value that specifies a property category.</para>
			/// <para>The DEVPROPGUID data type is defined as:</para>
			/// </summary>
			public Guid fmtid;

			/// <summary>
			/// <para>
			/// <c>pid</c> A DEVPROPID-typed value that uniquely identifies the property within the property category. For internal system
			/// reasons, a property identifier must be greater than or equal to two.
			/// </para>
			/// <para>The DEVPROPID data type is defined as:</para>
			/// </summary>
			public uint pid;

		}
		static DEVPROPKEY DEFINE_DEVPROPKEY(UInt32 l, UInt16 w1, UInt16 w2, Byte b1, Byte b2, Byte b3, Byte b4, Byte b5, Byte b6, Byte b7, Byte b8, uint pid)
		{
			return new DEVPROPKEY
			{
				fmtid = new Guid(l, w1, w2, b1, b2, b3, b4, b5, b6, b7, b8),
				pid = pid,
			};
		}

		// https://github.com/tpn/winsdk-10/blob/master/Include/10.0.16299.0/shared/devpkey.h

		static readonly DEVPROPKEY DEVPKEY_Device_DevNodeStatus = DEFINE_DEVPROPKEY(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 2);
		static readonly DEVPROPKEY DEVPKEY_Device_ProblemCode = DEFINE_DEVPROPKEY(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 3);
		static readonly DEVPROPKEY DEVPKEY_DeviceContainer_IsConnected = DEFINE_DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 55);
		static readonly DEVPROPKEY DEVPKEY_DeviceContainer_IsPaired = DEFINE_DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 56);
		static readonly DEVPROPKEY DEVPKEY_Device_FriendlyName = DEFINE_DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 14);

		[Flags]
		public enum DIGCF : int
		{
			DIGCF_DEFAULT = 0x00000001,    // only valid with DIGCF_DEVICEINTERFACE
			DIGCF_PRESENT = 0x00000002,
			DIGCF_ALLCLASSES = 0x00000004,
			DIGCF_PROFILE = 0x00000008,
			DIGCF_DEVICEINTERFACE = 0x00000010,
		}

		[StructLayout(LayoutKind.Sequential)]
		struct SP_DEVINFO_DATA
		{
			public UInt32 cbSize;
			public Guid ClassGuid;
			public UInt32 DevInst;
			public IntPtr Reserved;
		}
		[StructLayout(LayoutKind.Sequential)]
		struct SP_DEVICE_INTERFACE_DATA
		{
			public Int32 cbSize;
			public Guid interfaceClassGuid;
			public Int32 flags;
			private UIntPtr reserved;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		struct SP_DEVICE_INTERFACE_DETAIL_DATA
		{
			public int cbSize;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string DevicePath;
		}

		static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
		//	private static  Guid GUID_BLUETOOTHLE_DEVICE_INTERFACE = new Guid("781aee18-7733-4ce4-add0-91f41c67b592");


		private static Guid RaiderContainerId = new Guid("bbc06db0-919a-58d5-ac03-4e3ccca1a2bd");

		private static  Guid GUID_BTHPORT_DEVICE_INTERFACE = new Guid("0850302A-B344-4fda-9BE9-90576B8D46F0");

		public static Guid GUID_DEVCLASS_BLUETOOTH = new Guid("{0xe0cbf06c, 0xcd8b, 0x4647, {0xbb, 0x8a, 0x26, 0x3b, 0x43, 0xf0, 0xf9, 0x74}}");


		[DllImport("setupapi.dll", SetLastError = true)]
		static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);



		/// <summary>
		/// Open a handle to the plug and play dev node.
		/// SetupDiGetClassDevs() returns a device information set that contains 
		/// info on all installed devices of a specified class.
		/// </summary>
		/// <param name="ClassGuid"></param>
		/// <param name="Enumerator"></param>
		/// <param name="hwndParent"></param>
		/// <param name="Flags"></param>
		/// <returns></returns>
		[DllImport("setupapi.dll", CharSet = CharSet.Auto)]
		static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, int Flags);

		[DllImport("kernel32.dll")]
		static extern uint GetLastError();

		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern bool SetupDiGetDeviceRegistryProperty(
			IntPtr deviceInfoSet,
			ref SP_DEVINFO_DATA deviceInfoData,
			uint property,
			out UInt32 propertyRegDataType,
			byte[] propertyBuffer,
			uint propertyBufferSize,
			out UInt32 requiredSize);
		
		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern bool SetupDiGetDeviceProperty(
					IntPtr deviceInfoSet,
					ref SP_DEVINFO_DATA DeviceInfoData,
					ref DEVPROPKEY propertyKey,
					out UInt32 propertyType,
					byte[] propertyBuffer,
					uint propertyBufferSize,
					out UInt32 requiredSize,
					UInt32 flags);

		[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern bool SetupDiGetDeviceInstanceId(
			IntPtr DeviceInfoSet,
			ref SP_DEVINFO_DATA DeviceInfoData,
			byte[] DeviceInstanceId,
			int DeviceInstanceIdSize,
			out int RequiredSize);

		[DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern Boolean SetupDiEnumDeviceInterfaces(
			IntPtr hDevInfo,
			ref SP_DEVINFO_DATA devInfo,
			ref Guid interfaceClassGuid,
			UInt32 memberIndex,
			ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);
		
		[DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern Boolean SetupDiEnumDeviceInterfaces(
			IntPtr hDevInfo,
			IntPtr devInfo,
			ref Guid interfaceClassGuid,
			UInt32 memberIndex,
			ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);


		//[DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		//static extern Boolean SetupDiGetDeviceInterfaceDetail(
		//   IntPtr hDevInfo,
		//   ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
		//   ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
		//   UInt32 deviceInterfaceDetailDataSize,
		//   ref UInt32 requiredSize,
		//   ref SP_DEVINFO_DATA deviceInfoData
		//);

		//[DllImport(@"Setupapi.dll", CharSet= CharSet.Auto, SetLastError = true)]
		//static extern bool SetupDiGetDeviceInterfaceDetail(
		//	IntPtr DeviceInfoSet, 
		//	ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData,
		//	byte[] DeviceInterfaceDetailData, 
		//	int DeviceInterfaceDetailDataSize,
		//	out int RequiredSize,
		//	ref SP_DEVINFO_DATA DeviceInfoData);
		[DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr nSetupDiGetClassDevs, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr Ptr, uint DeviceInterfaceDetailDataSize, ref int RequiredSize, IntPtr PtrInfo);


	}
}
