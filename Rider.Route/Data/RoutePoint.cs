using GpxTools.Gpx;
using System;

namespace Rider.Route.Data
{
    internal class RoutePoint
	{
		public double Latitude { get; }
		public double Longitude { get; }
		public double Elevation { get; }
		public double Distance { get; }

		public RoutePoint(GpxPoint p)
		{
			Latitude = p.Latitude ;
			Longitude = p.Longitude ;
			Elevation = p.Elevation ?? 0;// m
			Distance = p.DistanceFromStart * 1000;// m
			//Latitude = Convert.ToDecimal(p.Latitude * 1000000.0);
			//Longitude = Convert.ToDecimal(p.Longitude * 1000000.0);
			//Elevation = Convert.ToDecimal(p.Elevation ?? 0.0);
			//Distance = Convert.ToDecimal(p.DistanceFromStart * 1000);
		}
	}

}
