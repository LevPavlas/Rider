using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Regions;
using Rider.Route.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Route.ViewModels.Tests
{
	[TestClass()]
	public class RouteViewModelTests
	{
		[TestMethod()]
		public void HeaderTextTest()
		{
			var manager = new Mock<IRegionManager>();
			RouteViewModel target = new RouteViewModel(manager.Object);
			Assert.AreEqual("Route", target.HeaderText);
		}

		[TestMethod()]
		public void MapViewModelTest()
		{
			var manager = new Mock<IRegionManager>();
			RouteViewModel target = new RouteViewModel(manager.Object);
			bool activateChaged = false;
			target.IsActiveChanged += (sender, args) => { activateChaged = true; };
			target.IsActive = true;
			manager.Verify(m => m.RequestNavigate(Constants.Regions.ToolBar, Constants.Views.RouteToolBar));
			Assert.IsTrue(activateChaged);
		}
	}
}