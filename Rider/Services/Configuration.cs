using MapControl.Caching;
using MapControl;
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
using static System.Environment;

namespace Rider.Services
{

	public class Configuration : BindableBase, IConfiguration
	{
		private const string DataFolder = "Data";
		private string ApplicationDirectory { get; set; } = string.Empty;
		private string ConfigurationPath { get; set; } = string.Empty;
		private string DataDirectory { get; set; } = string.Empty;

		private IFileSystem FileSystem { get; }
		private IConsole Console { get; }
		private ConfigurationData Data { get; set; } = new ConfigurationData();

		public string BrowserCacheDataFolder{ get; private set; }= string.Empty;
		public string MapControlCacheDataFolder { get; private set; } = string.Empty;
//		public string GpxDirectory { get; private set; } = string.Empty;
		public ObservableCollection<string> Maps { get; set; }= new ObservableCollection<string>();

		private string _SelectedMap=string.Empty;
		public string SelectedMap
		{ 
			get => _SelectedMap;
			set => SetProperty(ref _SelectedMap, value, OnChange);
		}

		private string _LastGpxFullPath = string.Empty;
		public string LastGpxFullPath
		{
			get => _LastGpxFullPath;
			set => SetProperty(ref _LastGpxFullPath, value, OnChange);
		}
		public string LastGpxDirectory => FileSystem.GetDirectoryName(LastGpxFullPath);
		public string LastGpxFilenameWithoutExtension =>FileSystem.GetFileNameWithoutExtension(LastGpxFullPath);

		private string _LastExportFullPath = string.Empty;
		public string LastExportFullPath
		{
			get => _LastExportFullPath;
			set => SetProperty(ref _LastExportFullPath, value, OnChange);
		}
		public string LastExportDirectory => GetLastExportDictionary();


		public Configuration(IFileSystem fileSystem, IConsole console)
		{
			FileSystem = fileSystem;
			Console = console;
		}
		public void Load()
		{
			ApplicationDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Rider";
			ConfigurationPath = $"{ApplicationDirectory}\\{DataFolder}\\Configuration.json";
			DataDirectory = $"{ApplicationDirectory}\\{DataFolder}";
			BrowserCacheDataFolder = $"{ApplicationDirectory}\\{DataFolder}\\BrowserCache";
			MapControlCacheDataFolder = $"{ApplicationDirectory}\\{DataFolder}\\MapControl";
			FileSystem.CreateDirectory(DataDirectory) ;

			if (FileSystem.FileExist(ConfigurationPath))
			{
				Data = FileSystem.LoadData<ConfigurationData>(ConfigurationPath) ;
				Console.WriteLine($"Loaded configuration: {ConfigurationPath}");
			}
			else
			{
				FileSystem.SaveData(ConfigurationPath, Data) ;
				Console.WriteLine($"Created configuration: {ConfigurationPath}");
			}
			
			Maps = new ObservableCollection<string>(Data.Maps);			
			
			_SelectedMap= Data.SelectedMap;
			_LastGpxFullPath= Data.LastGpxFullPath;
			_LastExportFullPath= Data.LastExportFullPath;

			Maps.CollectionChanged += OnMapsChanged;

			ImageLoader.HttpClient.DefaultRequestHeaders.Add("User-Agent", "XAML Map Control Test Application");
			TileImageLoader.Cache = new ImageFileCache(MapControlCacheDataFolder);

		}
		public string GetLastExportDictionary()
		{
			string path = FileSystem.GetDirectoryName(LastExportFullPath);
			if (FileSystem.DirectoryExists(path))
			{
				return path;
			}
			return DataDirectory;
		}
		private void OnMapsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Data.Maps = Maps.ToArray();
			FileSystem.SaveData(ConfigurationPath, Data);
		}


		private void OnChange()
		{
			Data.SelectedMap= SelectedMap;
			Data.LastGpxFullPath= LastGpxFullPath;
			Data.LastExportFullPath= LastExportFullPath;

			FileSystem.SaveData(ConfigurationPath, Data);
		}
		private class ConfigurationData
		{
			public string[] Maps { get; set; }
			public string SelectedMap { get; set; }
			public string LastGpxFullPath { get; set; }
			public string LastExportFullPath { get; set; }
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
				LastGpxFullPath= string.Empty;
				LastExportFullPath= string.Empty;
			}
		}

	}
}
