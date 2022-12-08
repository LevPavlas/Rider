using GpxTools.Gpx;
using MapControl;
using Prism.Events;
using Rider.Contracts.Events;
using Rider.Contracts.Services;
using Rider.Route.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Rider.Route.Services
{
	internal interface IRiderCalculator
	{
		void StartProcessing(string path);
	}

	internal class RiderCalculator : IRiderCalculator
	{

		private IGpxReader Reader { get; }
		private IConsole Console { get; }
		private IEventAggregator EventAggregator { get; }
		private IConfiguration Configuration { get; }

		private object _lock = new object();
		private bool IsProcessing { get; set; }
		public RiderCalculator(IGpxReader reader, IConsole console, IEventAggregator eventAgregator, IConfiguration configuration)
		{
			Reader = reader;
			Console = console;
			EventAggregator = eventAgregator;
			Configuration = configuration;
		}

		public void StartProcessing(string path)
		{
			lock (_lock)
			{
				if (IsProcessing)
				{
					Console.WriteWarning($"Calculator is busy. Cannot process file: '{path}'.");
					return;
				}

				IsProcessing = true;
			}

			try
			{
				Console.WriteLine($"Sart processing, Thread:{Thread.CurrentThread.ManagedThreadId}");
				Task.Run(async () => await Process(path));
			}
			catch (Exception e)
			{
				Console.WriteError($"Processing error. File: {path}");
				Console.WriteError(e.ToString());
			}

			lock(_lock)
			{
				IsProcessing = false;
			}
		}
		
		public async Task Process(string path)
		{
			try
			{
				Console.WriteLine($"Processing file:{path}");
				Data.Route route = await Reader.Read(path);
				Console.WriteLine($"Number of points: {route.Points.Count}");
				Console.WriteLine($"Route distance: {route.Distance/1000} km");
				ClimbChallengeCalculator climbCalculator = new ClimbChallengeCalculator(route.Points);
				IReadOnlyList<ClimbChallenge> chalenges = climbCalculator.Calculate();
				Console.WriteLine($"Climb Chalenges found: {chalenges.Count}");
				BoundingBox box = new BoundingBox(route.LatitudeMinSouth,route.LongitudeMinWest,route.LatitudeMaxNorth,route.LongitudeMaxEast);
				RiderData data = new RiderData(route, chalenges);
				EventAggregator.GetEvent<RiderDataCalculatedEvent>().Publish(data);
			}
			catch (Exception e)
			{
				Console.WriteError($"Processing error. File: {path}");
				Console.WriteError(e.Message);
			}

			IsProcessing = false;
		}
		int[] CreateReasterizedRouteProfile(Data.Route route)
		{
			int[] rasterized = new int [Configuration.ScreenWidth];
			if (route.Points.Count < 1) return rasterized;

			double rasterSize = route.Distance / Configuration.ScreenWidth;
			RoutePoint p0= route.Points[0];
			int lastRasterIndex = 0;

			for(int i = 0; i< route.Points.Count; i++)
			{
				RoutePoint p1= route.Points[i];
				int rasterIndex = (int)(p1.Distance / rasterSize);

				if (rasterIndex <= lastRasterIndex) 
				{
					rasterized[rasterIndex] = p0.Distance < p1.Distance ? i : i - 1;
				}
				else
				{
					for (int j = lastRasterIndex + 1; j <= rasterIndex;j++ )
					{
						rasterized[j] = i;
					}
				}
		

				p0= p1;
			}
			return rasterized;
		}
		

	}
}
