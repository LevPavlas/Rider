using Rider.Contracts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Route.Data
{
    internal class RiderData: IRiderData
	{
		public static IRiderData Empty=new RiderData();
		
		public IRoute Route { get; } = Data.Route.Empty;
		public bool IsEmpty => this == Empty;

		public RiderData(Route route)
		{
			Route = route;
		}
		private RiderData()
		{ 
		}
	}

}
