using DryIoc.Messages;
using GpxTools;
using GpxTools.Gpx;
using System;
using System.Collections.Generic;
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
			   
		public double Distance { get; } = 0;
		public IReadOnlyList<RoutePoint> Points { get; private set; } = new List<RoutePoint>();

	
		public Route(IList<GpxPoint> data)
		{
			List<RoutePoint> points = new List<RoutePoint>();
			double lastDistance = -1;
			foreach (GpxPoint p in data)
			{

				RoutePoint point = new RoutePoint(p);
				if(lastDistance >= point.Distance) continue; 
				
				lastDistance = point.Distance;

				LatitudeMaxNorth = Math.Max(LatitudeMaxNorth, point.Latitude);
				LatitudeMinSouth = Math.Min(LatitudeMinSouth, point.Latitude);

				LongitudeMaxEast = Math.Max(LongitudeMaxEast, point.Longitude);
				LongitudeMinWest = Math.Min(LongitudeMinWest, point.Longitude);

				points.Add(point);
			}

			Points = points;
			Distance = lastDistance < 0 ? 0 : lastDistance;
		}

	}

}
