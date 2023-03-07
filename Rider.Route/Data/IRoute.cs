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
	public interface IRoute
	{
		double LatitudeMinSouth { get; }
		double LatitudeMaxNorth { get; }
		double LongitudeMinWest { get; }
		double LongitudeMaxEast { get; }
		double ElevationMax { get; }
		double ElevationMin { get; }
		double ElevationGain { get; }
		double Distance { get; }
		IReadOnlyList<IPoint> Points { get; }

		int GetPointIndex(double distance);
	}
}
