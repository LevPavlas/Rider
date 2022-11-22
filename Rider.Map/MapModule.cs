using Rider.Constants;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using CefSharp.Wpf;
using CefSharp;
using System;
using Rider.Contracts;

namespace Rider.Map
{
	public class MapModule : IModule
	{
		private IRegionManager RegionManager { get; }

		public MapModule( IRegionManager regionManager)
		{
			RegionManager = regionManager;
			InitBrowser();
		}
		public void OnInitialized(IContainerProvider containerProvider)
		{
			RegionManager.RegisterViewWithRegion<Views.Map>(Regions.MainRegion);
			RegionManager.RegisterViewWithRegion<Views.ToolBar>(Regions.ToolBar);
		}

		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterForNavigation<Views.Map>(Constants.Views.Map);
			containerRegistry.RegisterForNavigation<Views.ToolBar>(Constants.Views.MapToolBar);
		}
		public void InitBrowser()
		{
			if (!Cef.IsInitialized)
			{

				CefSettings setting = new CefSettings();
				
//				setting.CachePath = System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, "Cache");
				setting.CachePath = System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, "Cache");
				//		setting.LogFile= System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Log\\Log.log");
				//		setting.LogSeverity = LogSeverity.Info;

				//setting.CefCommandLineArgs.Add("enable-media-stream", "1");
				//setting.CefCommandLineArgs.Add("allow-running-insecure-content", "1");
				//setting.CefCommandLineArgs.Add("use-fake-ui-for-media-stream", "1");
				//setting.CefCommandLineArgs.Add("enable-speech-input", "1");
				//setting.CefCommandLineArgs.Add("enable-usermedia-screen-capture", "1");

				if (!Cef.Initialize(setting)) throw new Exception("Browser not initialized");

				//BrowserExigencia.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;

			}

		}
	}
}
