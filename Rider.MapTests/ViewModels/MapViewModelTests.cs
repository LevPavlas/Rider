using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Regions;
using Rider.Map.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Map.ViewModels.Tests
{
	[TestClass()]
	public class MapViewModelTests
	{
		[TestMethod()]
		public void HeaderTextTest()
		{
			var manager = new Mock<IRegionManager>();
			MapViewModel target = new MapViewModel(manager.Object);
			Assert.AreEqual("Map", target.HeaderText);
		}

		[TestMethod()]
		public void MapViewModelTest()
		{
			var manager = new Mock<IRegionManager>();
			MapViewModel target = new MapViewModel(manager.Object);
			bool activateChaged = false;
			target.IsActiveChanged += (sender, args) => { activateChaged = true; };
			target.IsActive = true;
			manager.Verify(m => m.RequestNavigate(Constants.Regions.ToolBar, Constants.Views.MapToolBar));
			Assert.IsTrue(activateChaged);
		}
	}
}