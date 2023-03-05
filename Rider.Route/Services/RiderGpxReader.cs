using GpxTools.Gpx;
using GpxTools;
using Rider.Contracts;
using Rider.Route.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rider.Contracts.Services;
using System.Reflection.PortableExecutable;

namespace Rider.Route.Services
{
	internal interface IGpxReader
	{
		Task<IRoute> Read(string path);
	}

	internal class RiderGpxReader : IGpxReader
	{
		private IFileSystem FileSystem { get; }
		public IConsole Console { get; }

		public RiderGpxReader(IFileSystem fileSystem, IConsole console)
		{
			FileSystem = fileSystem;
			Console = console;
		}

		public async Task<IRoute> Read(string path)
		{
			return await Task<RiderData>.Run(() =>
			{
				using(Stream stream = FileSystem.OpenRead(path))
				using (GpxReader reader = new GpxReader(stream))
				{
					while(reader.Read())
					{

					}
					GpxPointCollection<GpxPoint>? points = reader.Track.ToGpxPoints();
					if (points == null)
					{
						points = reader.Route.ToGpxPoints();
					}
					if(points != null && points.Count >1)
					{
	
						points.CalculateDistanceFromStart();
						return new Data.Route(points);
					}

					return new Data.Route(new List<GpxPoint>());
				}
			});
		}
	}
}
