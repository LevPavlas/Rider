using Rider.Constants;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using Rider.Route.Services;

namespace Rider.Route
{
	public class RouteModule : IModule
	{
		private IRegionManager RegionManager { get; }

		public RouteModule(IRegionManager regionManager)
		{
			RegionManager = regionManager;
		}
		public void OnInitialized(IContainerProvider containerProvider)
		{
			RegionManager.RegisterViewWithRegion(Regions.ToolBar,typeof(Views.ToolBar));
			RegionManager.RegisterViewWithRegion(Regions.MainRegion, typeof(Views.Route));
		}
	
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{

			containerRegistry.RegisterManySingleton(typeof(RiderGpxReader), typeof(IGpxReader));
			containerRegistry.RegisterForNavigation<Views.ToolBar>(Constants.Views.RouteToolBar);
			containerRegistry.RegisterForNavigation<Views.Route>(Constants.Views.Route);
		}
	}
}
