using Prism.Events;
using Rider.Route.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Contracts.Events
{
	internal class RiderDataCalculatedEvent: PubSubEvent<RiderData>
	{
	}
}
