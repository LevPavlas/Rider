using DryIoc.Messages;
using GpxTools;
using GpxTools.Gpx;
using Rider.Contracts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Route.Data
{

    internal class Route: IRoute
	{
		public static Route Empty = new Route();

		public decimal LatitudeMin { get; }  = decimal.MaxValue;
		public decimal LatitudeMax { get; } = decimal.MinValue;
		public decimal LongitudeMin { get; } = decimal.MaxValue;
		public decimal LongitudeMax { get; } = decimal.MinValue;

		public decimal Distance { get; } = 0;
		public IReadOnlyList<IRoutePoint> Points { get; private set; } = new List<IRoutePoint>();

		public bool IsEmpty => this == Empty;

		public Route(GpxAnalyser analyser)
		{
			List<RoutePoint> points = new List<RoutePoint>();

			foreach (GpxPoint p in analyser.Points)
			{

				RoutePoint point = new RoutePoint(p);

				LatitudeMax = Math.Max(LatitudeMax, point.Latitude);
				LatitudeMin = Math.Min(LatitudeMin, point.Latitude);

				LongitudeMax = Math.Max(LongitudeMax, point.Longitude);
				LongitudeMin = Math.Min(LongitudeMin, point.Longitude);

				points.Add(point);
			}

			Points = points;
			Distance = Points.Count > 0 ? (int)(Points[Points.Count - 1].Distance) : 0;
		}

		private Route()
		{
		}
	}

}
