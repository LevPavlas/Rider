﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Contracts
{
	public interface IConfiguration
	{
		ObservableCollection<string> Maps { get; }
		public string SelectedMap { get; }
		public string BrowserCacheDataFolder { get; }
		void Load();
	}
}
