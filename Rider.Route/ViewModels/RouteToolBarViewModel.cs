﻿using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Win32;
using System.Windows.Media.Animation;
using System.ComponentModel;
using Rider.Route.Data;
using Rider.Route.Services;
using Rider.Contracts.Services;
using Prism.Events;
using Rider.Contracts.Events;
using System.IO;
using System.Windows.Shapes;
using System.ComponentModel.DataAnnotations;

namespace Rider.Route.ViewModels
{
    internal class RouteToolBarViewModel : BindableBase
	{
		const int Reserved = 0;
		public IConfiguration Configuration { get; }
		private IConsole Console{get;}
		private IWpfDialogService Dialogs { get; }
		private IRiderCalculator Calculator { get; }
		private IEventAggregator EventAggregator { get; }
		public IFileSystem FileSystem { get; }
		public DelegateCommand OpenCommand { get; private set; }
		public DelegateCommand ExportCommand { get; private set; }

		RiderData? _RiderData;
		public RiderData? RiderData
		{
			get => _RiderData;
			set => SetProperty(ref _RiderData, value);
		}

		public RouteToolBarViewModel(
			IConfiguration configuration,
			IConsole console, 
			IWpfDialogService dialogs,
			IRiderCalculator calculator,
			IEventAggregator eventAggregator,
			IFileSystem fileSystem) 
		{
			Configuration = configuration;
			Console = console;
			Dialogs = dialogs;
			Calculator = calculator;

			EventAggregator = eventAggregator;
			FileSystem = fileSystem;
			EventAggregator.GetEvent<RouteDownloadedEvent>().Subscribe(OnRouteDownloaded, ThreadOption.BackgroundThread);
			EventAggregator.GetEvent<RiderDataCalculatedEvent>().Subscribe(OnDatatCalculated, ThreadOption.PublisherThread);
			OpenCommand = new DelegateCommand(Open,CanOpenExecute);
			ExportCommand = new DelegateCommand(Export, CanExportExecute);
		}


		void Open()
		{
			//		OpenInProgress = true;
			//		OpenCommand.RaiseCanExecuteChanged();
			string? file = Dialogs.OpenGpxFile();
			if(file!= null && FileSystem.FileExist(file))
			{
					Calculator.StartProcessing(file);
			}

			//		OpenInProgress = false;
			//		OpenCommand.RaiseCanExecuteChanged();	
		}
		void OnRouteDownloaded(string path)
		{
			Calculator.StartProcessing(path);
		}
		bool OpenInProgress { get; set; }
		bool CanOpenExecute()
		{
			return !OpenInProgress;
		}

		void Export()
		{
			try
			{
				string? file = Dialogs.SaveTrackFile();
				if (file != null)
				{
					Export(file);
				}
			}
			catch (Exception ex)
			{
				Console.WriteError(ex.ToString());
			}
		}
		bool CanExportExecute()
		{
			return RiderData?.Route?.Points?.Count > 2;
		}
		private void OnDatatCalculated(RiderData data)
		{
			RiderData = data;
			ExportCommand.RaiseCanExecuteChanged();
		}
		private void Export(string file)
		{		
			if (RiderData?.Route?.Points?.Count > 2)
			{
				using (Stream stream = FileSystem.OpenWrite(file))
				{
					using (BinaryWriter writer = new BinaryWriter(stream))
					{
						WriteTrack(writer, RiderData.Route.Points);
					}
				}
				Console.WriteLine($"Created file: {file}");

				string smyFile = System.IO.Path.ChangeExtension(file, "smy");
				using (Stream stream = FileSystem.OpenWrite(smyFile))
				{
					using (BinaryWriter writer = new BinaryWriter(stream))
					{
						WriteSmy(writer, RiderData.Route);
					}
				}

				Console.WriteLine($"Created file: {smyFile}");

				string tinfoFile = System.IO.Path.ChangeExtension(file, "tinfo");
				using (Stream stream = File.OpenWrite(tinfoFile))
				{
					using (BinaryWriter writer = new BinaryWriter(stream))
					{
						var challenges = from challenge in RiderData.Challenges
										 orderby challenge.Start
										 select challenge;
						WriteTinfo(writer, challenges);
					}
				}
				Console.WriteLine($"Created file: {tinfoFile}");
			}
		}

		public void WriteTrack(BinaryWriter writer, IReadOnlyList<IPoint> points)
		{
			IPoint p0 = points[0];
			writer.Write(Convert.ToInt32(p0.Latitude * 1000000.0));
			writer.Write(Convert.ToInt32(p0.Longitude * 1000000.0));
			writer.Write(Convert.ToInt16(p0.Elevation));
			writer.Write((short)0);  // Grade for climb ??? color ???
			writer.Write(Reserved);
			short grade = (short)0;


			for(int i = 1; i < points.Count; i++ )
			{
				IPoint p1 = points[i];
				writer.Write(Convert.ToInt32(p1.Latitude * 1000000.0));
				writer.Write(Convert.ToInt32(p1.Longitude * 1000000.0));
				writer.Write(Convert.ToInt16(p1.Elevation));
				grade = CalculateGrade(p0, p1, grade);
				writer.Write(grade);  // Grade for climb ??? color ???
				writer.Write(Reserved);
				
				p0 = p1;
			}
		}
		short CalculateGrade(IPoint p0, IPoint p1, short previousGrade)
		{
			double distance = p1.Distance - p0.Distance;
			if ( Math.Abs(distance) < 0.1) return previousGrade;

			double elevation = p1.Elevation - p0.Elevation;
			double grade = 100* elevation / distance;

			if (grade < -100) grade = -100;
			if(grade> 100) grade = 100;
			
			return (byte)Convert.ToInt32(grade);
		}

		public void WriteSmy(BinaryWriter writer, Data.Route route)
		{
			//int altitudegain = 798;

			byte[] reserved01 = new byte[] { 1, 0 }; //dword 2
			writer.Write(reserved01);								// byte 0-1
			short count = (short)route.Points.Count;
			writer.Write(count);									// byte 2-3
			writer.Write(Convert.ToInt32(route.LatitudeMaxNorth * 1000000.0));	// byte 4-7
			writer.Write(Convert.ToInt32(route.LatitudeMinSouth * 1000000.0));	// byte 8-11
			writer.Write(Convert.ToInt32(route.LongitudeMaxEast * 1000000.0));	// byte 12-15
			writer.Write(Convert.ToInt32(route.LongitudeMinWest * 1000000.0));	// byte 16-19
			writer.Write(Convert.ToInt32(route.Distance));          // byte 20-23 

			writer.Write(Convert.ToInt16(route.ElevationMax));      // byte 24-25 maximum altitude																	
			writer.Write(Convert.ToInt16(0));      // byte 26-27 minimum altitude ??? i have no clue what it is.
			writer.Write(new byte[32]);                             // byte 28-59
			writer.Write(Convert.ToInt16(route.ElevationGain));     // byte 60-61 elevation gain
			writer.Write(new byte[6]);                              // byte 62-67

		}
		public void WriteTinfo(BinaryWriter writer, IEnumerable<ClimbChallenge> challenges)
		{

			// POI 
			// 2 bytes for coordinate index
			// 1 byte for direction
			//		0x18 - turn-over
			//		0x1c - ferry
			//		0x65 - peak
			//		0x03 - left
			//		0x02 - right
			//		0x07 - close left
			//		0x06 - close right
			//		0x05 - slight left
			//		0x04 - slight right
			//		0x01 - go ahead
			//		0x01 - go ahead
			//		0x08 - exit right
			//		0x07 - uturn left
			//		0x01 - go ahead
			//		0x01 - go ahead
			//		0x09 - exit left
			//		0x08 - exit right
			//		0x01 - go ahead
			//		0xff - none
			// 1 byte reserved 0x00
			// 2 byte distance in meters
			// 2 byte reserved 0x00 0x00
			// 2 byte time in seconds
			// 2 byte reserved 0x00 0x00
			// 32 byte instruction description

			// Climb info
			// 2 bytes for coordinate index
			// BE - start
			// 1 byte reserved 0x00
			// 8 bytes FF
			// 32 bytes 0 ? descritpion??
			// 2 bytes for coordinate index
			// BF - end
			// 1 byte reserved 0x00
			// 8 bytes FF
			// 32 bytes 0 ? descritpion??

			byte start = 0xBE;
			byte end = 0xBF;
			byte reserved0 = (byte)0;
			byte[] reservedFF = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
			byte[] descriptor = new byte[]
			{
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0
			};

			foreach (ClimbChallenge c in challenges)
			{
				writer.Write((ushort)c.Start); //word 0
				writer.Write(start);
				writer.Write(reserved0);

				writer.Write(reservedFF); //word 1 -2
				writer.Write(descriptor);// word 3- 10
				writer.Write((ushort)c.End);//word 11
				writer.Write(end);
				writer.Write(reserved0);
				writer.Write(reservedFF); //word 12 - 13
				writer.Write(descriptor); //word 14 - 21
			}
		}


	}
}
