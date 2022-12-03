﻿using GpxTools.Gpx;
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

		private object _lock = new object();
		private bool IsProcessing { get; set; }
		public RiderCalculator(IGpxReader reader, IConsole console, IEventAggregator eventAgregator)
		{
			Reader = reader;
			Console = console;
			EventAggregator = eventAgregator;
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
				Console.WriteLine($"Parse file:{path}");
				await Task.Delay(500);
				Data.Route route = await Reader.Read(path);
				Console.WriteLine($"Number of points: {route.Points.Count}");
				Console.WriteLine($"Route distance: {route.Distance/1000} km");

				Console.WriteLine($"Calculate Climb Challenges");
				ClimbChallengeCalculator climbCalculator = new ClimbChallengeCalculator(route.Points);
				IReadOnlyList<ClimbChallenge> chalenges = climbCalculator.Calculate();
				Console.WriteLine($"Chalenges found: {chalenges.Count}");
				BoundingBox box = new BoundingBox(route.LatitudeMinSouth,route.LongitudeMinWest,route.LatitudeMaxNorth,route.LongitudeMaxEast);
				RiderData data = new RiderData(route, chalenges);
				Console.WriteLine($"File processing finished");
				EventAggregator.GetEvent<RiderDataCalculatedEvent>().Publish(data);
			}
			catch (Exception e)
			{
				Console.WriteError($"Processing error. File: {path}");
				Console.WriteError(e.Message);
			}

			IsProcessing = false;
		}
		

	}
}