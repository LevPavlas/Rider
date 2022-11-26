using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Map
{
	public class RiderMapException:Exception
	{
		public RiderMapException(string msg):base(msg) { }
	}
}
