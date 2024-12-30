using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using Rider.Constants;
using Rider.Contracts.Services;

namespace Rider
{
    internal class RiderModule : IModule
	{
		private IRegionManager RegionManager { get; }
		private IConfiguration Configuration { get; }

		public RiderModule( IRegionManager regionManger, IConfiguration configuration)
		{
			RegionManager = regionManger;
			Configuration = configuration;
		}
		public void OnInitialized(IContainerProvider containerProvider)
		{
			Configuration.Load();
			RegionManager.RegisterViewWithRegion(Regions.Console, typeof(Views.Console));
		}
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterManySingleton(typeof(Services.WpfDialogService), typeof(IWpfDialogService));
			containerRegistry.Register(typeof(object), typeof(Views.Console), Constants.Views.Console);
		}
	}
}
