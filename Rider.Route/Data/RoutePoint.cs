using GpxTools.Gpx;
using Rider.Contracts.Data;
using System;

namespace Rider.Route.Data
{
    internal class RoutePoint: IRoutePoint
	{
		public decimal Latitude { get; }
		public decimal Longitude { get; }
		public decimal Elevation { get; }
		public decimal Distance { get; }

		public RoutePoint(GpxPoint p)
		{
			Latitude = Convert.ToDecimal(p.Latitude * 1000000.0);
			Longitude = Convert.ToDecimal(p.Longitude * 1000000.0);
			Elevation = Convert.ToDecimal(p.Elevation ?? 0.0);
			Distance = Convert.ToDecimal(p.DistanceFromStart * 1000);
		}
	}

}
