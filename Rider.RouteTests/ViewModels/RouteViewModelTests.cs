using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using Prism.Regions;
using Rider.Contracts.Events;
using Rider.Contracts.Services;
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
		private Mock<IRegionManager> RegionManager { get; set; } = new Mock<IRegionManager>();
		private Mock<IEventAggregator> EventAggregator { get; set; } = new Mock<IEventAggregator>();
		private Mock<IConsole> Console { get; set; } = new Mock<IConsole>();

		[TestInitialize()]
		public void Setup()
		{
			RegionManager = new Mock<IRegionManager>();
			EventAggregator = new Mock<IEventAggregator>();
			Console = new Mock<IConsole>();
		}
		private RouteViewModel CreateTarget()
		{
			Mock<RiderDataCalculatedEvent> calculateEvent= new Mock<RiderDataCalculatedEvent>();
			//calculateEvent.SetupGet(x => x.Subscribe(It.IsAny<Action<Data.RiderData>>()))
			//	.Callback< Data.RiderData, SubscriptionToken>((action) =>
			//	{
					
			//	}
			//	);
			EventAggregator.Setup(e => e.GetEvent<RiderDataCalculatedEvent>()).Returns(calculateEvent.Object);
			return new RouteViewModel(RegionManager.Object, EventAggregator.Object,Console.Object);
		}
		[TestMethod()]
		public void HeaderTextTest()
		{
			var manager = new Mock<IRegionManager>();
			RouteViewModel target = CreateTarget();
			Assert.AreEqual("Route", target.HeaderText);
		}

		[TestMethod()]
		public void ActivatelTest()
		{
			RouteViewModel target = CreateTarget();
			bool activateChaged = false;
			target.IsActiveChanged += (sender, args) => { activateChaged = true; };
			target.IsActive = true;
			RegionManager.Verify(m => m.RequestNavigate(Constants.Regions.ToolBar, Constants.Views.RouteToolBar));
			Assert.IsTrue(activateChaged);
		}
		[TestMethod()]
		public void OnCalculatedTest()
		{
			RouteViewModel target = CreateTarget();


		}

	}
}