using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Ioc;
using Prism.Regions;
using Rider.Map;
using Rider.Map.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Rider.Map.Tests
{
	[TestClass()]
	public class MapModuleTests
	{
		[TestMethod()]
		public void RegisterTypesTest()
		{
			var regionManager = new Mock<IRegionManager>();
			MapModule target = new MapModule(regionManager.Object);
			var registry = new Mock<IContainerRegistry>();
			target.RegisterTypes(registry.Object);
			registry.Verify(r => r.RegisterManySingleton(typeof(CefSharpService), typeof(ICefSharpService)));
			registry.Verify(r => r.Register(typeof(object), typeof(Views.Map), Constants.Views.Map));
			registry.Verify(r => r.Register(typeof(object), typeof(Views.ToolBar), Constants.Views.MapToolBar));
			registry.VerifyNoOtherCalls();
			regionManager.VerifyNoOtherCalls();
		}

		[TestMethod()]
		public void OnInitializedTest()
		{
			var regionManager = new Mock<IRegionManager>();
			MapModule target = new MapModule(regionManager.Object);
			var container = new Mock<IContainerProvider>();
			target.OnInitialized(container.Object);
			regionManager.Verify(r => r.RegisterViewWithRegion(Constants.Regions.MainRegion, typeof(Views.Map)));
			regionManager.Verify(r => r.RegisterViewWithRegion(Constants.Regions.ToolBar, typeof(Views.ToolBar)));
			regionManager.VerifyNoOtherCalls();
		}
	}
}