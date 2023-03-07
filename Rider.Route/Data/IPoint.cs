using GpxTools.Gpx;
using System;

namespace Rider.Route.Data
{
	public interface IPoint
	{
		double Latitude { get; }
		double Longitude { get; }
		double Elevation { get; }
		double Distance { get; }
		double Grade { get;}
	}

}
