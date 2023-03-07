using GpxTools.Gpx;
using System;

namespace Rider.Route.Data
{
	public interface IPoint
	{
		public double Latitude { get; }
		public double Longitude { get; }
		public double Elevation { get; }
		public double Distance { get; }
	}

}
