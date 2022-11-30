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
	internal class TrackPoint
	{
		public decimal Latitude { get; }
		public decimal Longitude { get; }
		public decimal Elevation { get; }
		public decimal Distance { get; }

		public int RiderLatitude { get; }
		public int RiderLongitude { get; }
		public int RiderElevation { get; }
		public int RiderDistance { get; }

		public TrackPoint(GpxPoint p)
		{
			Latitude = Convert.ToDecimal(p.Latitude * 1000000.0);
			Longitude = Convert.ToDecimal(p.Longitude * 1000000.0);
			Elevation = Convert.ToDecimal(p.Elevation ?? 0.0);
			Distance = Convert.ToDecimal(p.DistanceFromStart * 1000);

			RiderLatitude = Convert.ToInt32(Latitude);
			RiderLongitude = Convert.ToInt32(Longitude);
			RiderElevation = Convert.ToInt32(Elevation);
			RiderDistance = Convert.ToInt32(Distance);
		}
	}

	internal class Track
	{
		public decimal LatitudeMin  = decimal.MaxValue;
		public decimal LatitudeMax  = decimal.MinValue;
		public decimal LongitudeMin = decimal.MaxValue;
		public decimal LongitudeMax = decimal.MinValue;

		public int LatitudeMin_S = int.MaxValue;
		public int LatitudeMax_N = int.MinValue;
		public int LongitudeMin_W = int.MaxValue;
		public int LongitudeMax_E = int.MinValue;

		public decimal Distance => (int)(Points[Points.Count - 1].Distance);
		public IReadOnlyList<TrackPoint> Points { get; private set; }

		public Track(GpxAnalyser analyser)
		{
			List<TrackPoint> points = new List<TrackPoint>();

			foreach(GpxPoint p in analyser.Points)
			{

				TrackPoint point = new TrackPoint(p);

				LatitudeMax = Math.Max(LatitudeMax, point.Latitude);
				LatitudeMin = Math.Min(LatitudeMin, point.Latitude);

				LongitudeMax = Math.Max(LongitudeMax, point.Longitude);
				LongitudeMin = Math.Min(LongitudeMin, point.Longitude);

				points.Add(point);
			}
			LatitudeMax_N = (int)LatitudeMax;
			LatitudeMin_S = (int)LatitudeMin;

			LongitudeMax_E = (int)LongitudeMax;
			LongitudeMin_W = (int)LongitudeMin;

			Points = points;
		}
	}

}
