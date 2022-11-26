using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider
{
	public class RiderException:Exception
	{
		public RiderException(string msg):base(msg) { }
	}
}
