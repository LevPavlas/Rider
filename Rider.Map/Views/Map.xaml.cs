using CefSharp;
using CefSharp.DevTools;
using CefSharp.DevTools.Browser;
using CefSharp.Wpf;
using Prism.Events;
using Rider.Contracts;
using Rider.Contracts.Events;
using Rider.Map.Events;
using Rider.Map.Services;
using Rider.Map.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rider.Map.Views
{

	/// <summary>
	/// Interaction logic for Map.xaml
	/// </summary>
	internal partial class Map : UserControl, IDownloadHandler
	{
		private MapViewModel Model { get; }
		private string SelectedMap => Model?.Configuration.SelectedMap ?? string.Empty;
		private IEventAggregator EventAggregator { get; }
	
		public Map(MapViewModel model, ICefSharpService cef, IEventAggregator eventAggregator)
		{
			DataContext= model;
			Model = model;
			EventAggregator = eventAggregator;
			cef.Initiaize();
			InitializeComponent();
			//EnableGeolocation();
			//EnableGeolocationWithPosition();
			Browser.DownloadHandler = this;
			EventAggregator.GetEvent<MapChangedEvent>().Subscribe(OnMapChanged, ThreadOption.BackgroundThread);
		}
		public bool CanDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod)
		{
			return true;
		}

		public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
		{
			if (!callback.IsDisposed)
			{
				string fullPath = Model.GetFullPathForDownload(downloadItem.SuggestedFileName);
				callback.Continue(fullPath, false);
			}
		}

		public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
		{
			if (downloadItem.IsValid)
			{
				if (downloadItem.IsComplete)
				{
					Console.WriteLine($"Downloaded GPX: {downloadItem.FullPath}");
					if (SelectedMap == Constants.Maps.BrouterDe)
					{
						SendEscToBrowser();// this close Export dialog
					}
					EventAggregator.GetEvent<RouteDownloadedEvent>().Publish(downloadItem.FullPath);
				}
			}
		}

		void OnMapChanged(string map)
		{
			Browser.LoadUrlAsync(map);
		}
		void SendEscToBrowser()
		{
			KeyEvent k = new KeyEvent
			{
				WindowsKeyCode = 0x1B, // Esc
				FocusOnEditableField = true,
				IsSystemKey = false,
				Type = KeyEventType.KeyDown
			};

			Browser.GetBrowser().GetHost().SendKeyEvent(k);

			Thread.Sleep(100);

			k = new KeyEvent
			{
				WindowsKeyCode = 0x1B, // Esc
				FocusOnEditableField = true,
				IsSystemKey = false,
				Type = KeyEventType.KeyUp
			};

			Browser.GetBrowser().GetHost().SendKeyEvent(k);

			Thread.Sleep(100);
		}
		void EnableGeolocation()
		{
			Browser.ExecuteScriptAsyncWhenPageLoaded(
				@"
					navigator.permissions.query = options => {
						return Promise.resolve({
							state: 'granted'
						});
					};", oneTime: false);
		}

		void EnableGeolocationWithPosition()
		{
			Browser.ExecuteScriptAsyncWhenPageLoaded(@"
navigator.permissions.query = options => {
  return Promise.resolve({
    state: 'granted'
  });
};
navigator.geolocation.getCurrentPosition = (success, error, options) => {
  success({
    coords: {
      latitude: 0.854477,
      longitude: 101.234738,
      accuracy: 10,
      altitude: null,
      altitudeAccuracy: null,
      heading: null,
      speed: null
    },
    timestamp: Date.now()
  });
};
navigator.geolocation.watchPosition = (success, error, options) => {
  success({
    coords: {
      latitude: -33.854477,
      longitude: 151.234738,
      accuracy: 49,
      altitude: null,
      altitudeAccuracy: null,
      heading: null,
      speed: null
    },
    timestamp: Date.now()
  });
  return 0;
};
", oneTime: false);

		}

	}
}
