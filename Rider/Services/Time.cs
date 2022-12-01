using Rider.Contracts;
using Rider.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Services
{
	internal class Time : ITime
	{
		public string TimeStamp => DateTime.Now.ToString("yyyyMMddHHmmss");
	}
}
