using DryIoc.Messages;
using GpxTools;
using GpxTools.Gpx;
using Rider.Route.UserControls;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
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
		public double ElevationGain { get; } = 0;
		public double Distance { get; } = 0;
		public IReadOnlyList<RoutePoint> Points { get; private set; } = new List<RoutePoint>();

		private SortedList<decimal,int> Map { get; }= new SortedList<decimal,int>();
		private object _lock = new object();
		public Route(IList<GpxPoint> data)
		{
			const double elevationTreshold = 2.5;

			if (data==null || data.Count < 2) throw new ArgumentException($"Route need least 2 waypoints.");

			List < RoutePoint> points = new List<RoutePoint>();
			double lastDistance = -1;
			double lastElevation = double.MaxValue;

			for(int i= 0; i< data.Count; i++)
			{
				GpxPoint p = data[i];

				RoutePoint point = new RoutePoint(p);
				if(lastDistance >= point.Distance) continue; 
				
				lastDistance = point.Distance;
				
				if (lastElevation < point.Elevation + elevationTreshold)
				{
					ElevationGain += point.Elevation - lastElevation;
					lastElevation = point.Elevation;
				}
				else if(lastElevation > point.Elevation + elevationTreshold) 
				{
					lastElevation = point.Elevation;
				}


				LatitudeMaxNorth = Math.Max(LatitudeMaxNorth, point.Latitude);
				LatitudeMinSouth = Math.Min(LatitudeMinSouth, point.Latitude);

				LongitudeMaxEast = Math.Max(LongitudeMaxEast, point.Longitude);
				LongitudeMinWest = Math.Min(LongitudeMinWest, point.Longitude);

				ElevationMax= Math.Max(ElevationMax, point.Elevation);
				ElevationMin= Math.Min(ElevationMin, point.Elevation);

				points.Add(point);
				Map.Add((decimal)point.Distance,points.Count - 1);
			}

			Points = points;
			Distance = lastDistance < 0 ? 0 : lastDistance;
		}
		public int GetPointIndex(double distance)
		{
			int index;
			if (distance < 0) return -1;

			lock(_lock)
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
				if(index >=0 && index < Points.Count)
				{
					return index;
				}
				return -1;
			}
		}
	
	}

}
