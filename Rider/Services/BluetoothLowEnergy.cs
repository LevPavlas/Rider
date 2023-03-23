using Rider.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Rider.Services
{
	internal partial class BluetoothLowEnergy : IBluetoothLowEnergy
	{
		private IConsole Console { get; }

		public BluetoothLowEnergy(IConsole console)
		{
			Console = console;
		}

		public void TestIface2()
		{
			IntPtr handle = SetupDiGetClassDevs(ref GUID_DEVCLASS_BLUETOOTH, IntPtr.Zero, IntPtr.Zero, (int)(DIGCF.DIGCF_PRESENT));
			if (handle == INVALID_HANDLE_VALUE)
			{
				return;
			}
			uint error = GetLastError();

			SP_DEVINFO_DATA did = new SP_DEVINFO_DATA();
			did.cbSize = (uint)Marshal.SizeOf(did);
			SP_DEVICE_INTERFACE_DATA dia = new SP_DEVICE_INTERFACE_DATA();
			dia.cbSize = Marshal.SizeOf(dia);

			for (int i = 0; SetupDiEnumDeviceInfo(handle, (uint)i, ref did); i++)
			{
				string name = GetRegistryProperty(handle, did, SetupDiGetDeviceRegistryPropertyEnum.SPDRP_FRIENDLYNAME);
				if (name.Contains("RiderS500"))
				{
					Console.WriteLine($"Name: {name}");
					bool isOk = SetupDiEnumDeviceInterfaces(handle, ref did, ref GUID_BTHPORT_DEVICE_INTERFACE, (uint)0, ref dia);
					error = GetLastError();

					//for (int propId = 0; propId < 40; propId++)
					//{
					//	string prop = GetRegistryProperty(handle, did, (SetupDiGetDeviceRegistryPropertyEnum)propId);
					//	Console.WriteLine($"Prop:{propId}, value :{prop}");
					//}

				}
			}

		}
		public void TestIface()
		{
			IntPtr handle = SetupDiGetClassDevs(ref GUID_BTHPORT_DEVICE_INTERFACE, IntPtr.Zero, IntPtr.Zero, (int)(DIGCF.DIGCF_DEVICEINTERFACE));
//			IntPtr handle = SetupDiGetClassDevs(ref GUID_BTHPORT_DEVICE_INTERFACE, IntPtr.Zero, IntPtr.Zero, (int)(DIGCF.DIGCF_DEVICEINTERFACE));
//			IntPtr handle = SetupDiGetClassDevs(ref RaiderContainerId, IntPtr.Zero, IntPtr.Zero, (int)(DIGCF.DIGCF_DEVICEINTERFACE));
			if (handle == INVALID_HANDLE_VALUE)
			{
				return;
			}

			SP_DEVINFO_DATA did = new SP_DEVINFO_DATA();
			did.cbSize = (uint)Marshal.SizeOf(did);

			SP_DEVICE_INTERFACE_DATA dia = new SP_DEVICE_INTERFACE_DATA();
			dia.cbSize = Marshal.SizeOf(dia);


			for (int i = 0; SetupDiEnumDeviceInterfaces(handle, IntPtr.Zero, ref GUID_BTHPORT_DEVICE_INTERFACE, (uint)i, ref dia); i++)
//			for (int i = 0; SetupDiEnumDeviceInterfaces(handle, IntPtr.Zero, ref RaiderContainerId, (uint)i, ref dia); i++)
			{

				if (true)

				{
					IntPtr detailDataBuffer = IntPtr.Zero;
					Int32 bufferSize = 0;

					SP_DEVINFO_DATA da = new SP_DEVINFO_DATA();

					da.cbSize = (uint)Marshal.SizeOf(da);



					SP_DEVICE_INTERFACE_DETAIL_DATA didd = new SP_DEVICE_INTERFACE_DETAIL_DATA();

//					didd.DevicePath = new byte[Win32Calls.ANYSIZE_ARRAY];
					didd.DevicePath =string.Empty;





					didd.cbSize = 4 + Marshal.SystemDefaultCharSize;

					uint nBytes = (uint)didd.cbSize;



					bool Success = SetupDiGetDeviceInterfaceDetail(handle, ref dia, IntPtr.Zero, 0, ref bufferSize, IntPtr.Zero);
					uint glerr = GetLastError();

					detailDataBuffer = Marshal.AllocHGlobal(bufferSize);

					Marshal.WriteInt32(detailDataBuffer, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);

					nBytes = (uint)bufferSize;



					Success = SetupDiGetDeviceInterfaceDetail(handle, ref dia, detailDataBuffer, nBytes, ref bufferSize, IntPtr.Zero);

					glerr = GetLastError();

				//	textBox1.Text = "SetupDiGetDeviceInterfaceDetail Success:" + Success.ToString() + " LastError:" + glerr.ToString();

					if (Success)

					{
											long cosi = detailDataBuffer.ToInt64();

										IntPtr pDevicePathName = new IntPtr(cosi + 4);

						string? path = Marshal.PtrToStringAuto(pDevicePathName);

						Console.WriteLine(path);
						//\\?\usb#vid_8087&pid_0026#5&1632802&0&14#{0850302a-b344-4fda-9be9-90576b8d46f0}

						//IntPtr ptrInstanceBuf = Marshal.AllocHGlobal((Int32)nBytes);

						//Win32Calls.CM_Get_Device_ID((UInt32)pDevicePathName.ToInt32(), ptrInstanceBuf, (Int32)nBytes, 0);

						//string InstanceID = Marshal.PtrToStringAuto(ptrInstanceBuf);

						//Marshal.FreeHGlobal(ptrInstanceBuf);

					}

				}




				SP_DEVICE_INTERFACE_DETAIL_DATA detail = new SP_DEVICE_INTERFACE_DETAIL_DATA();
		//		Marshal.WriteInt32(detail, IntPtr.Size == 8 ? 8 : 6);

				//detail.cbSize = Marshal.SizeOf(detail);

				//uint size = 0;


				//int requiredSize;
				//byte[] buffer = null;

				//// First call to get amount of memory needed
				//SetupDiGetDeviceInterfaceDetail(handle, ref dia, buffer, 0, out requiredSize, ref did);
				//uint err = GetLastError();


				//buffer =  new byte[requiredSize * 2];

				//SP_DEVICE_INTERFACE_DETAIL_DATA detailData = new
				//SP_DEVICE_INTERFACE_DETAIL_DATA();
				//detailData.cbSize =Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DETAIL_DATA));
				//Marshal.StructureToPtr(detailData, buffer, false);

				//// Second call to actually retrieve data
				//SetupDiGetDeviceInterfaceDetail(handle, ref dia, buffer, requiredSize, out requiredSize, ref did);
				//err = GetLastError();

				// Retrieve the DevicePath from SP_DEVICE_INTERFACE_DETAIL_DATA
				// 4 == Offset of first DevicePath character
				//IntPtr pDevicePath = (IntPtr)((int)buffer + 4);
				//string DevicePath = Marshal.PtrToStringAuto(pDevicePath);

				// Clean up
				//		Marshal.FreeHGlobal(buffer);

				//if (!SetupDiGetDeviceInterfaceDetail(handle, ref dia, ref detail, 0, ref size, ref did))
				//{
				//	uint err = GetLastError();

				//	if (err == (uint)CredUIReturnCodes.ERROR_NOT_FOUND) break;

				//	//SP_DEVICE_INTERFACE_DETAIL_DATA pInterfaceDetailData = new SP_DEVICE_INTERFACE_DETAIL_DATA();

				//	//pInterfaceDetailData.cbSize = Marshal.SizeOf(pInterfaceDetailData);

				//	if (!SetupDiGetDeviceInterfaceDetail(handle, ref dia, ref detail, size, ref size, ref did))
				//		break;

				//	//hComm = CreateFile(
				//	//	pInterfaceDetailData->DevicePath,
				//	//	GENERIC_WRITE | GENERIC_READ,
				//	//	FILE_SHARE_READ | FILE_SHARE_WRITE,
				//	//	NULL,
				//	//	OPEN_EXISTING,
				//	//	0,
				//	//	NULL);

				//}
			}

			uint glerrr = GetLastError();
		}

		public void Test()
		{
			//TestIface2();
			//return;
			IntPtr handle = SetupDiGetClassDevs(ref GUID_DEVCLASS_BLUETOOTH, IntPtr.Zero, IntPtr.Zero, (int)( DIGCF.DIGCF_PRESENT));
	//		IntPtr handle = SetupDiGetClassDevs(ref GUID_DEVCLASS_BLUETOOTH, IntPtr.Zero, IntPtr.Zero, (int)(DIGCF.DIGCF_DEVICEINTERFACE));
			//			IntPtr handle = SetupDiGetClassDevs(ref GUID_DEVCLASS_BLUETOOTH, IntPtr.Zero, IntPtr.Zero, (int)DIGCF.DIGCF_ALLCLASSES);
			if (handle == INVALID_HANDLE_VALUE)
			{
				return;
			}
			SP_DEVINFO_DATA did = new SP_DEVINFO_DATA();
			did.cbSize = (uint)Marshal.SizeOf(did);
			
			SP_DEVICE_INTERFACE_DATA dia = new SP_DEVICE_INTERFACE_DATA();
			dia.cbSize = Marshal.SizeOf(dia);

			//		SP_DEVICE_INTERFACE_DATA ifaceData = new SP_DEVICE_INTERFACE_DATA();
			//                   Success = Win32Calls.SetupDiEnumDeviceInterfaces(h, IntPtr.Zero, ref DiskGUID, i, ref dia);


			//for (int i = 0; SetupDiEnumDeviceInterfaces(handle, IntPtr.Zero, ref GUID_DEVCLASS_BLUETOOTH, (uint)i, ref dia); i++)
			//{

			//}

					uint error = GetLastError();

			for (int i = 0; SetupDiEnumDeviceInfo(handle, (uint)i, ref did); i++)
			{
				//bool ok = SetupDiEnumDeviceInterfaces(handle, ref did, ref GUID_DEVCLASS_BLUETOOTH, (uint)i, ref dia);
				//uint error = GetLastError();
				//for(int propId = 0; propId< 40; propId++)
				//{
				//	string prop = GetProperty(handle, did, (SetupDiGetDeviceRegistryPropertyEnum)propId);
				//	Console.WriteLine($"Prop:{propId}, value :{prop}");
				//}
				string name = GetRegistryProperty(handle, did, SetupDiGetDeviceRegistryPropertyEnum.SPDRP_FRIENDLYNAME);
				string guid = GetRegistryProperty(handle, did, SetupDiGetDeviceRegistryPropertyEnum.SPDRP_CLASSGUID);
				//{e0cbf06c-cd8b-4647-bb8a-263b43f0f974} bluetooth
				string value = GetRegistryProperty(handle, did, SetupDiGetDeviceRegistryPropertyEnum.SPDRP_BASE_CONTAINERID);
				string status = GetRegistryProperty(handle, did, SetupDiGetDeviceRegistryPropertyEnum.SPDRP_BASE_CONTAINERID);
				if(name.Contains("RiderS500"))
				{
					Console.WriteLine($"Name: {name}, CassGuid: {guid}, ContainerId:{value}");
					for (int propId = 0; propId < 40; propId++)
					{
						string prop = GetRegistryProperty(handle, did, (SetupDiGetDeviceRegistryPropertyEnum)propId);
						Console.WriteLine($"Prop:{propId}, value :{prop}");
					}

					//string connected = GetDeviceProperty(handle, did, DEVPKEY_DeviceContainer_IsConnected);
					//string connected = GetDeviceProperty(handle, did, DEVPKEY_Device_FriendlyName);
					string connected = GetDeviceProperty(handle, did, DEVPKEY_DeviceContainer_IsPaired);
				}
			}

			
		}

		private string GetRegistryProperty(IntPtr handle, SP_DEVINFO_DATA did, SetupDiGetDeviceRegistryPropertyEnum property)
		{
			uint data;
			byte[] buffer = new byte[0];
			uint bufferSize = (uint)buffer.Length;

			while (!SetupDiGetDeviceRegistryProperty(
					handle,
					ref did,
					(uint)property,
					out data,
					buffer,
					bufferSize,
					out bufferSize))
			{
				if (GetLastError() == (uint)CredUIReturnCodes.ERROR_INSUFFICIENT_BUFFER)
				{
					buffer = new byte[bufferSize * 2];
				}
				else
				{
					return string.Empty;
				}

			}

			return Encoding.Unicode.GetString(buffer);
		}

		private string GetDeviceProperty(IntPtr handle, SP_DEVINFO_DATA did, DEVPROPKEY key)
		{
			uint data;
			byte[] buffer = new byte[0];
			uint bufferSize = (uint)buffer.Length;

			while (!SetupDiGetDeviceProperty(
					handle,
					ref did,
					ref key,
					out data,
					buffer,
					bufferSize,
					out bufferSize,
					0))
			{
				uint error = GetLastError();
				if (error == (uint)CredUIReturnCodes.ERROR_INSUFFICIENT_BUFFER)
				{
					buffer = new byte[bufferSize * 2];
				}
				else
				{
					return string.Empty;
				}

			}

			return Encoding.Unicode.GetString(buffer);
		}

	}
}