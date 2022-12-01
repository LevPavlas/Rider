using Rider.Contracts.Services;
using Rider.Route.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Route.Services
{
	internal interface IRouteCalculator
	{
		void StartProcessing(string path);
	}

	internal class RouteCalculator : IRouteCalculator
	{
		private IGpxReader Reader { get; }
		private IConsole Console { get; }

		private object _lock = new object();
		private bool IsProcessing { get; set; }
		public RouteCalculator(IGpxReader reader, IConsole console)
		{
			Reader = reader;
			Console = console;
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
			Console.WriteLine($"Parse file:{path}");
			RiderData data = await Reader.Read(path);
			Console.WriteLine($"Number of points: {data.Route.Points.Count}");
			Console.WriteLine($"Route distance: {data.Route.Distance}[m]");

			Console.WriteLine($"Calculate Climb Challenges");
			data = await CalculateChallenges(data);
			Console.WriteLine($"File processing finished");
			IsProcessing = false;
		}
		public Task<RiderData> CalculateChallenges(RiderData data)
		{
			return Task<RiderData>.FromResult(data);
		}

			
	


	}
}
