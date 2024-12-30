using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rider.Constants;
using Rider.Route.Services;

namespace Rider.Route.Tests
{
    [TestClass()]
	public class RouteModuleTests
	{
		[TestMethod()]
		public void OnInitializedTest()
		{
			var manager = new Mock<IRegionManager>();

			RouteModule target = new RouteModule(manager.Object);

			var containerProvider = new Mock<IContainerProvider>();

			target.OnInitialized(containerProvider.Object);

			manager.Verify(m => m.RegisterViewWithRegion(Regions.ToolBar, typeof(Views.RouteToolBar)));
			manager.Verify(m => m.RegisterViewWithRegion(Regions.MainRegion, typeof(Views.Route)));

		}

		[TestMethod()]
		public void RegisterTypesTest()
		{
			var manager = new Mock<IRegionManager>();
			RouteModule target = new RouteModule(manager.Object);
			var registry = new Mock<IContainerRegistry>();
			target.RegisterTypes(registry.Object);

			registry.Verify(r => r.Register(typeof(object), typeof(Views.RouteToolBar), Constants.Views.RouteToolBar));
			registry.Verify(r => r.Register(typeof(object), typeof(Views.Route), Constants.Views.Route));
			registry.Verify(r => r.RegisterManySingleton(typeof(GpxReader), typeof(IGpxReader)));
			registry.Verify(r => r.RegisterManySingleton(typeof(ClimbChallengeCalculator), typeof(IClimbChallengeCalculator)));
			registry.Verify(r => r.RegisterManySingleton(typeof(RiderCalculator), typeof(IRiderCalculator)));
			registry.Verify(r => r.RegisterManySingleton(typeof(RiderWriter), typeof(IRiderWriter)));

		}
	}
}
