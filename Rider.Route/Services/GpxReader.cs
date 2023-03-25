//using GpxTools.Gpx;
//using GpxTools;
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
using System.Security.Cryptography.Pkcs;
using MapControl;
using Gpx = GpxTools.Gpx;
namespace Rider.Route.Services
{
	internal interface IGpxReader
	{
		Task<IRoute?> Read(string path);
	}

	internal class GpxReader : IGpxReader
	{
		private IFileSystem FileSystem { get; }
		public IConsole Console { get; }

		public GpxReader(IFileSystem fileSystem, IConsole console)
		{
			FileSystem = fileSystem;
			Console = console;
		}

		public Task<IRoute?> Read(string path)
		{
			return Task<IReadOnlyList<IPoint>>.Run(() =>
			{

				Gpx.GpxPointCollection<Gpx.GpxPoint> gpxPoints = ReadGpxPoints(path);
				IReadOnlyList<IPoint> points = ConvertGpxPointsToPoints(gpxPoints);
				if(points.Count < 2)
				{
					Console.WriteError($"Invalid GPX file. Route require least 2 points.");
					return null;
				}

				IRoute route =  CreateRoute(FileSystem.GetFileNameWithoutExtension(path), points);


				return Task.FromResult<IRoute?>(route);
			});
		}
		private Gpx.GpxPointCollection<Gpx.GpxPoint> ReadGpxPoints(string path)
		{
			Gpx.GpxPointCollection<Gpx.GpxPoint> result = new Gpx.GpxPointCollection<Gpx.GpxPoint>();

			using (Stream stream = FileSystem.OpenRead(path))
			using (GpxTools.GpxReader reader = new GpxTools.GpxReader(stream))
			{
				while (reader.Read())
				{

				}
				if (reader.Track != null)
				{
					result = reader.Track.ToGpxPoints();
				}
				else if (reader.Route != null)
				{
					result = reader.Route.ToGpxPoints();
				}

			}

			return result;
		}
		private IReadOnlyList<IPoint> ConvertGpxPointsToPoints(Gpx.GpxPointCollection<Gpx.GpxPoint> points)
		{
			const double MinimalPointDistance = 0.1;
			List<IPoint> result = new List<IPoint>();
			if (points.Count > 1)
			{
				points.CalculateDistanceFromStart();
			}

			Point p0 = new Point
			{
				Latitude = points[0].Latitude,
				Longitude = points[0].Longitude,
				Elevation = points[0].Elevation ?? 0,
				Distance = 1000* points[0].DistanceFromStart,
			};
			result.Add(p0);

			for (int i = 1; i < points.Count; i++)
			{
				double distance = 1000 * points[i].DistanceFromStart;
				if (distance - p0.Distance >= MinimalPointDistance)
				{
					Point p1 = new Point
					{
						Latitude = points[i].Latitude,
						Longitude = points[i].Longitude,
						Elevation = points[i].Elevation ?? 0,
						Distance = distance,
					};
					result.Add(p1);
					CalculateGrade(p0,p1);
					p0 = p1;
				}
			}

			return result;

		}
		void CalculateGrade(Point p0, Point p1)
		{
			p0.Grade = 100*(p1.Elevation - p0.Elevation)/(p1.Distance - p0.Distance);
			if(p0.Grade < -100) p0.Grade = -100; 
			if(p0.Grade > 100) p0.Grade = 100;
		}

		private IRoute CreateRoute(string name,IReadOnlyList<IPoint> points)
		{

			const double elevationTreshold = 1.8;

			double latitudeMinSouth = double.MaxValue;
			double latitudeMaxNorth = double.MinValue;
			double longitudeMinWest = double.MaxValue;
			double longitudeMaxEast = double.MinValue;
			double elevationMax = double.MinValue;
			double elevationMin = double.MaxValue;
			double elevationGain = 0;
			SortedList<decimal, int> map = new SortedList<decimal, int>();


			double lastElevation = double.MaxValue;

			for (int i = 0; i < points.Count; i++)
			{

				IPoint point = points[i];

				if (lastElevation < point.Elevation + elevationTreshold)
				{
					elevationGain += point.Elevation - lastElevation;
					lastElevation = point.Elevation;
				}
				else if (lastElevation > point.Elevation + elevationTreshold)
				{
					lastElevation = point.Elevation;
				}


				latitudeMaxNorth = Math.Max(latitudeMaxNorth, point.Latitude);
				latitudeMinSouth = Math.Min(latitudeMinSouth, point.Latitude);

				longitudeMaxEast = Math.Max(longitudeMaxEast, point.Longitude);
				longitudeMinWest = Math.Min(longitudeMinWest, point.Longitude);

				elevationMax = Math.Max(elevationMax, point.Elevation);
				elevationMin = Math.Min(elevationMin, point.Elevation);

				map.Add((decimal)point.Distance, points.Count - 1);
			}

			RouteP route = new RouteP(name, points, map)
			{
				LatitudeMaxNorth = latitudeMaxNorth,
				LatitudeMinSouth = latitudeMinSouth,
				LongitudeMaxEast = longitudeMaxEast,
				LongitudeMinWest = longitudeMinWest,
				ElevationMax = elevationMax,
				ElevationMin = elevationMin,
				ElevationGain = elevationGain,
				Distance = points[points.Count - 1].Distance,
			};
			return route;
		}

		private class RouteP : IRoute
		{
			public double LatitudeMinSouth { get; set; }

			public double LatitudeMaxNorth { get; set; }

			public double LongitudeMinWest { get; set; }

			public double LongitudeMaxEast { get; set; }

			public double ElevationMax { get; set; }

			public double ElevationMin { get; set; }

			public double ElevationGain { get; set; }

			public double Distance { get; set; }

			public IReadOnlyList<IPoint> Points { get;}
			public SortedList<decimal, int> Map { get;}
			public string Name { get; }

			public RouteP(string name, IReadOnlyList<IPoint> points, SortedList<decimal, int> map)
			{
				Name = name;
				Points = points;
				Map = map;
			}


			private object _lock = new object();

			public int GetPointIndex(double distance)
			{
				int index;
				if (distance < 0) return -1;

				lock (_lock)
				{
					decimal dist = (decimal)distance;
					if (Map.ContainsKey(dist))
					{
						index = Map.IndexOfKey(dist);
					}
					else
					{
						Map[dist] = -1;
						index = Map.IndexOfKey(dist);
						Map.Remove(dist);
					}
					if (index >= 0 && index < Points.Count)
					{
						return index;
					}
					return -1;
				}
			}
		}
		private class Point : IPoint
		{
			public double Latitude { get; set; }
			public double Longitude { get; set; }
			public double Elevation { get; set; }
			public double Distance { get; set; }
			public double Grade { get; set; }
		}

	}
}