using CefSharp;
using CefSharp.DevTools;
using CefSharp.DevTools.Browser;
using CefSharp.Wpf;
using Rider.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	public class DownloadHandler : IDownloadHandler
	{
		//public event EventHandler<DownloadItem> OnBeforeDownloadFired;

		//public event EventHandler<DownloadItem> OnDownloadUpdatedFired;

		public bool CanDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod)
		{
			return true;
		}

		public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
		{
			//OnBeforeDownloadFired?.Invoke(this, downloadItem);

			if (!callback.IsDisposed)
			{
				using (callback)
				{
					callback.Continue(downloadItem.SuggestedFileName, showDialog: true);
				}
			}
		}

		public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
		{
			//OnDownloadUpdatedFired?.Invoke(this, downloadItem);
		}
	}

	/// <summary>
	/// Interaction logic for Map.xaml
	/// </summary>
	public partial class Map : UserControl
	{
		private IConsole Console { get; }

		public Map(IConsole console)
		{
			InitializeComponent();
			Browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
			Browser.ConsoleMessage += OnConsoleMessage;
			EnableGeolocation();
			Console = console;
		}

		private void OnConsoleMessage(object? sender, ConsoleMessageEventArgs e)
		{
			Console.WriteLine(e.Message);
		}

		void OnIsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (Browser.IsBrowserInitialized)
			{
				Browser.Load("https://www.mapy.cz/");
			}
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
      latitude: -33.854477,
      longitude: 151.234738,
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
