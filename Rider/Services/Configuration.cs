using Prism.Mvvm;
using Rider.Contracts;
using Rider.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace Rider.Services
{

	public class Configuration : BindableBase, IConfiguration
	{
		private const string DataFolder = "Data";
		private string ApplicationDirectory { get; set; } = string.Empty;
		private string ConfigurationPath { get; set; } = string.Empty;
		private string DataDirectory { get; set; } = string.Empty;

		private IFileSystem FileSystem { get; }
		private ConfigurationData Data { get; set; } = new ConfigurationData();

		public string BrowserCacheDataFolder{ get; private set; }= string.Empty;
		public string GpxDirectory { get; private set; } = string.Empty;
		public ObservableCollection<string> Maps { get; set; }= new ObservableCollection<string>();
		
		private string _SelectedMap=string.Empty;
		public string SelectedMap
		{ 
			get => _SelectedMap;
			set => SetProperty(ref _SelectedMap, value, OnChange);
		}
		
		public Configuration(IFileSystem fileSystem)
		{
			FileSystem = fileSystem;
		}
		public void Load()
		{
			ApplicationDirectory = FileSystem.GetApplicationDirectory();
			ConfigurationPath = $"{ApplicationDirectory}\\{DataFolder}\\Configuration.json";
			DataDirectory = $"{ApplicationDirectory}\\{DataFolder}";
			BrowserCacheDataFolder = $"{ApplicationDirectory}\\{DataFolder}\\BrowserCache";
			GpxDirectory = $"{ApplicationDirectory}\\{DataFolder}\\Gpx";

			FileSystem.CreateDirectory(DataDirectory) ;
			FileSystem.CreateDirectory(GpxDirectory);
			if (FileSystem.FileExist(ConfigurationPath))
			{
				Data = FileSystem.LoadData<ConfigurationData>(ConfigurationPath) ;
			}
			else
			{
				FileSystem.SaveData(ConfigurationPath, Data) ;
			}
			Maps = new ObservableCollection<string>(Data.Maps);
			SelectedMap= Data.SelectedMap;

			Maps.CollectionChanged += OnMapsChanged;
			double height = SystemParameters.FullPrimaryScreenHeight;
			double width = SystemParameters.FullPrimaryScreenWidth;
			double height2 = SystemParameters.VirtualScreenHeight;
			double width2 = SystemParameters.VirtualScreenWidth;
		}

		private void OnMapsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Data.Maps = Maps.ToArray();
			FileSystem.SaveData(ConfigurationPath, Data);
		}


		private void OnChange()
		{
			Data.SelectedMap= SelectedMap;
			FileSystem.SaveData(ConfigurationPath, Data);
		}
		private class ConfigurationData
		{
			public string[] Maps { get; set; }
			public string SelectedMap { get; set; }
			public ConfigurationData()
			{
				Maps = new string[]
				{
					Constants.Maps.BrouterDe,
					Constants.Maps.MapyCz,
					Constants.Maps.Graphhoper,
					Constants.Maps.CycleTravel
				};
				SelectedMap = Constants.Maps.BrouterDe;
			}
		}

	}
}
