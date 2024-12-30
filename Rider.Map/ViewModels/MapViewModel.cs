using System;
using Prism;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using Rider.Contracts.Services;

namespace Rider.Map.ViewModels
{
    internal class MapViewModel:BindableBase, IActiveAware
	{
		public string HeaderText { get; } = "Map";
		public event EventHandler? IsActiveChanged;
		public IConfiguration Configuration { get; }

		private IRegionManager RegionManager { get; }
		private IEventAggregator EventAggregator { get; }
		public IConsole Console { get; }
		private IFileSystem FileSystem { get; }


		public MapViewModel(
			IRegionManager regionManager,
			IEventAggregator eventAggregator, 
			IConfiguration configuration, 
			IConsole console,
			IFileSystem fileSystem)
		{
			RegionManager = regionManager;
			EventAggregator = eventAggregator;
			Configuration = configuration;
			Console = console;
			FileSystem = fileSystem;
		}

		bool isActive = false;
		public bool IsActive
		{
			get => isActive;
			set
			{
				if (isActive != value)
				{
					if(value) Activate();
					isActive = value;
					IsActiveChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}
		void Activate()
		{
			RegionManager.RequestNavigate(Constants.Regions.ToolBar, Constants.Views.MapToolBar);	
		}
		public void OnGpxDownloaded(string path)
		{
			Configuration.LastGpxFullPath = path;
		}
	}
}
