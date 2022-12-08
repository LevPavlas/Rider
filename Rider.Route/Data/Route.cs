using DryIoc.Messages;
using GpxTools;
using GpxTools.Gpx;
using Rider.Route.UserControls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Route.Data
{

    internal class Route
	{
		public double LatitudeMinSouth { get; } = double.MaxValue;
		public double LatitudeMaxNorth { get; } = double.MinValue;
		public double LongitudeMinWest { get; } = double.MaxValue;
		public double LongitudeMaxEast { get; } = double.MinValue;
		public double ElevationMax { get; } = double.MinValue;
		public double ElevationMin { get; }= double.MaxValue;
		public double Distance { get; } = 0;
		public IReadOnlyList<RoutePoint> Points { get; private set; } = new List<RoutePoint>();

		private double RasterSize { get; }
		public int[] RasterizedProfile { get; }

		public Route(IList<GpxPoint> data, int pixelWidth)
		{
			if (data==null || data.Count < 2) throw new ArgumentException($"Route need least 2 waypoints.");
			if (pixelWidth < 1) throw new ArgumentException($"Invalid raster");

			RasterizedProfile = new int[pixelWidth];
			Distance = data[data.Count - 1].DistanceFromStart;
			RasterSize = Distance / pixelWidth;

			List < RoutePoint> points = new List<RoutePoint>();
			double lastDistance = -1;
			for(int i= 0; i< data.Count; i++)
			{
				GpxPoint p = data[i];

				RoutePoint point = new RoutePoint(p);
				if(lastDistance >= point.Distance) continue; 
				
				lastDistance = point.Distance;

				LatitudeMaxNorth = Math.Max(LatitudeMaxNorth, point.Latitude);
				LatitudeMinSouth = Math.Min(LatitudeMinSouth, point.Latitude);

				LongitudeMaxEast = Math.Max(LongitudeMaxEast, point.Longitude);
				LongitudeMinWest = Math.Min(LongitudeMinWest, point.Longitude);

				ElevationMax= Math.Max(ElevationMax, point.Elevation);
				ElevationMin= Math.Min(ElevationMin, point.Elevation);

				points.Add(point);
			}

			Points = points;
			Distance = lastDistance < 0 ? 0 : lastDistance;
		}

		int[] CreateReasterizedRouteProfile(Data.Route route)
		{
			int[] rasterized = new int[Configuration.ScreenWidth];
			if (route.Points.Count < 1) return rasterized;

			double rasterSize = route.Distance / Configuration.ScreenWidth;
			RoutePoint p0 = route.Points[0];
			int lastRasterIndex = 0;

			for (int i = 0; i < route.Points.Count; i++)
			{
				RoutePoint p1 = route.Points[i];
				int rasterIndex = (int)(p1.Distance / rasterSize);

				if (rasterIndex <= lastRasterIndex)
				{
					rasterized[rasterIndex] = p0.Distance < p1.Distance ? i : i - 1;
				}
				else
				{
					for (int j = lastRasterIndex + 1; j <= rasterIndex; j++)
					{
						rasterized[j] = i;
					}
				}


				p0 = p1;
			}
			return rasterized;
		}


	}

}
