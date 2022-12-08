using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Contracts.Services
{
	public interface IConfiguration
	{
		int ScreenHeight { get; } 
		int ScreenWidth { get; }

		ObservableCollection<string> Maps { get; }
		public string SelectedMap { get; }
		public string BrowserCacheDataFolder { get; }
		public string GpxDirectory { get; }
		void Load();
	}
}
