using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Contracts.Events
{
	public class DeviceConnectionEvent:PubSubEvent<string[]>
	{
	}
}
