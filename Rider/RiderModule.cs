using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Services.Dialogs;
using Rider.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;
using System.Xml.Linq;

namespace Rider
{
    internal class RiderModule : IModule
	{
		private IRegionManager RegionManager { get; }

		public RiderModule( IRegionManager regionManger)
		{
			RegionManager = regionManger;
		}
		public void OnInitialized(IContainerProvider containerProvider)
		{
			RegionManager.RegisterViewWithRegion(Regions.Console, typeof(Views.Console));
		}
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterManySingleton(typeof(Services.WpfDialogService), typeof(Contracts.IWpfDialogService));
			containerRegistry.RegisterManySingleton(typeof(Views.Console), typeof(Views.Console),typeof(Contracts.IConsole));
			containerRegistry.Register(typeof(object), typeof(Views.Console), Constants.Views.Console);
		}
	}
}
