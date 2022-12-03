using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Route.Data
{
    internal class RiderData
	{
			
		public Route Route { get; } 

		public RiderData(Route route)
		{
			Route = route;
		}
	}

}
