using MapControl;
using Prism.Events;
using Rider.Contracts.Events;
using Rider.Contracts.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;

namespace Rider.Services
{
	internal class UsbMonitor: IUsbMonitor
	{
		private const string TrackFolder = "Tracks";
		private const int WM_DEVICECHANGE = 0x00000219;
		private const int DBT_DEVICEARRIVAL = 0x00008000;
		private const int DBT_DEVICEREMOVECOMPLETE = 0x00008004;


		private IConsole Console { get; }
		private IFileSystem FileSystem { get; }
		private IEventAggregator EventAggregator { get; }
		public string[] TrackDirectories { get; private set; } = new string[0];
		public bool IsConnectedDevice=>TrackDirectories.Length > 0;
		public UsbMonitor(IConsole console,IFileSystem fileSystem, IEventAggregator eventAggregator) 
		{
			Console = console;
			FileSystem = fileSystem;
			EventAggregator = eventAggregator;
		}

		public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{

			if (msg == WM_DEVICECHANGE) //Device state has changed
			{
				//				Console.WriteLine($"msg:{msg.ToString("X")}, wParam:{wParam.ToString("X")}, lParam:{lParam.ToString("X")}");

				switch (wParam.ToInt32())
				{
					case DBT_DEVICEARRIVAL:
						CheckDevices();
						break;
					case DBT_DEVICEREMOVECOMPLETE:
						CheckDevices();
						break;
					default:
						return IntPtr.Zero;
				}
			}

			return IntPtr.Zero;
		}


		public void CheckDevices()
		{
			try
			{
				List<string> directories = new List<string>();

				DriveInfo[] drives = DriveInfo.GetDrives();
				foreach (DriveInfo drive in drives)
				{
					CheckDevice(drive,directories);
				}

				if (IsChange(directories))
				{
					TrackDirectories = directories.ToArray();

					EventAggregator.GetEvent<DeviceConnectionEvent>().Publish(TrackDirectories);
				}


			}
			catch (Exception ex)
			{
				Console.WriteError(ex.ToString());
			}
		}
		void CheckDevice(DriveInfo device, List<string> directories)
		{
			string drive = device.Name;
			string directory = drive + TrackFolder;
			string volume = device.VolumeLabel;

			if(FileSystem.DirectoryExists(directory))
			{
				Console.WriteLine($"Connected Volume: {volume}, Path: {directory}");

				string[] files = FileSystem.GetDirectoryFiles(directory);
				foreach (string file in files)
				{
					Console.WriteLine($"\t{file}");
				}
				directories.Add(directory);
			}
		}
		bool IsChange(List<string> directories)
		{
			if(directories.Count != TrackDirectories.Length) return true;
			for(int i = 0; i < directories.Count; i++)
			{
				if (!TrackDirectories[i].Equals(directories[i])) return false;	
			}
			return true;
		}

	}
}
