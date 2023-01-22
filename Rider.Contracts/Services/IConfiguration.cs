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
		string LastGpxFullPath { get; set; }
		string LastGpxDirectory { get; }
		string LastGpxFilenameWithoutExtension { get; }
		string LastExportFullPath { get; set; }
		string LastExportDirectory { get; }
		ObservableCollection<string> Maps { get; }
		string SelectedMap { get; }
		string BrowserCacheDataFolder { get; }
		void Load();
	}
}
