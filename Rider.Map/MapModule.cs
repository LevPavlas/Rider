using Rider.Constants;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using CefSharp.Wpf;
using CefSharp;
using System;
using Rider.Contracts;
using Rider.Map.Services;

namespace Rider.Map
{
	public class MapModule : IModule
	{
		private IRegionManager RegionManager { get; }
		private ICefSharpService CefSharpService { get; }

		public MapModule( IRegionManager regionManager)
		{
			RegionManager = regionManager;
		}
		public void OnInitialized(IContainerProvider containerProvider)
		{
			containerProvider.Resolve<ICefSharpService>().Initiaize();
			RegionManager.RegisterViewWithRegion<Views.Map>(Regions.MainRegion);
			RegionManager.RegisterViewWithRegion<Views.ToolBar>(Regions.ToolBar);
		}

		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterManySingleton(typeof(CefSharpService), typeof(ICefSharpService));
			containerRegistry.RegisterForNavigation<Views.Map>(Constants.Views.Map);
			containerRegistry.RegisterForNavigation<Views.ToolBar>(Constants.Views.MapToolBar);
		}

	}
}
