﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Ioc;
using Prism.Regions;
using Rider.Constants;
using Rider.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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

			manager.Verify(m => m.RegisterViewWithRegion(Regions.ToolBar, typeof(Views.RiderToolBar)));
			manager.Verify(m => m.RegisterViewWithRegion(Regions.MainRegion, typeof(Views.Route)));

		}

		[TestMethod()]
		public void RegisterTypesTest()
		{
			var manager = new Mock<IRegionManager>();
			RouteModule target = new RouteModule(manager.Object);
			var registry = new Mock<IContainerRegistry>();
			target.RegisterTypes(registry.Object);

			registry.Verify(r => r.Register(typeof(object), typeof(Views.RiderToolBar), Constants.Views.RouteToolBar));
			registry.Verify(r => r.Register(typeof(object), typeof(Views.Route), Constants.Views.Route));
		}
	}
}
