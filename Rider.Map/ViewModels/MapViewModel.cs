using CefSharp;
using Prism;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Rider.Contracts;
using Rider.Map.Events;
using Rider.Map.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Map.ViewModels
{
	internal class MapViewModel:BindableBase, IActiveAware, IDownloadHandler
	{
		public string HeaderText { get; } = "Map";
		public event EventHandler? IsActiveChanged;
		public IConfiguration Configuration { get; }

		private IRegionManager RegionManager { get; }
		private IEventAggregator EventAggregator { get; }
		private IConsole Console { get; }
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

		public bool CanDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod)
		{
			return true;
		}

		public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
		{
			if (!callback.IsDisposed)
			{
				Encoding iso = Encoding.GetEncoding("ISO-8859-1");
				Encoding utf8 = Encoding.UTF8;
				//byte[] utfBytes = utf8.GetBytes(downloadItem.SuggestedFileName);
				//byte[] isoBytes = Encoding.Convert(utf8, iso, utfBytes);
				//string msg = iso.GetString(isoBytes);
				byte[] isoBytes = iso.GetBytes(downloadItem.SuggestedFileName);
				byte[] utfBytes = Encoding.Convert(iso, utf8, isoBytes); ;
				string msg = iso.GetString(utfBytes);


				string fullPath = FileSystem.AddTimeStamp($"{Configuration.GpxDirectory}\\{downloadItem.SuggestedFileName}");
				callback.Continue(fullPath, false);
			}
		}

		public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
		{
			if(downloadItem.IsValid)
			{
				if (downloadItem.IsComplete)
				{
					string path = $"Gpx downloaded: {Configuration.GpxDirectory}\\{downloadItem.SuggestedFileName}";
					Console.WriteLine(path);
					EventAggregator.GetEvent<GpxDownloadedEvent>().Publish(path);
					
				}
			}
		}				

	}
}
