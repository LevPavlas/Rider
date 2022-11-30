using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Route.Data
{
	internal class RiderData
	{
		public Track Track { get; }

		public RiderData(Track track)
		{
			Track = track;
		}
	}

}
