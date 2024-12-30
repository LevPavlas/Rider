using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using Rider.Constants;
using Rider.Map.Services;

namespace Rider.Map
{
    public class MapModule : IModule
	{
		private IRegionManager RegionManager { get; }

		public MapModule( IRegionManager regionManager)
		{
			RegionManager = regionManager;
		}
		public void OnInitialized(IContainerProvider containerProvider)
		{
			RegionManager.RegisterViewWithRegion<Views.Map>(Regions.MainRegion);
			RegionManager.RegisterViewWithRegion<Views.MapToolBar>(Regions.ToolBar);
		}

		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterManySingleton(typeof(CefSharpService), typeof(ICefSharpService));
			containerRegistry.RegisterForNavigation<Views.Map>(Constants.Views.Map);
			containerRegistry.RegisterForNavigation<Views.MapToolBar>(Constants.Views.MapToolBar);
		}

	}
}
