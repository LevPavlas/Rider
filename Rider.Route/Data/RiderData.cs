using MapControl;
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
		public IList<ClimbChallenge> Challenges { get; }
		public RiderData(Route route, IList<ClimbChallenge> challenges)
		{
			Route = route;
			Challenges = challenges;
		}
	}

}
