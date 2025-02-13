﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rider.Constants;
using Rider.Contracts.Services;

namespace Rider.Tests
{
    [TestClass()]
	public class RiderModuleTests
	{

		[TestMethod()]
		public void RegisterTypesTest()
		{
			Mock<IRegionManager> manager = new Mock<IRegionManager>();
			Mock<IConfiguration> config= new Mock<IConfiguration>();
	
			RiderModule target = new RiderModule(manager.Object, config.Object);

			Mock<IContainerRegistry> container = new Mock<IContainerRegistry>();
			target.RegisterTypes(container.Object);

			container.Verify(c => c.RegisterManySingleton(typeof(Services.WpfDialogService), typeof(IWpfDialogService)));
			container.Verify(c => c.Register(typeof(object), typeof(Views.Console), Constants.Views.Console));
		}

		[TestMethod()]
		public void OnInitializedTest()
		{
			Mock<IRegionManager> manager = new Mock<IRegionManager>();
			Mock<IConfiguration> config = new Mock<IConfiguration>();
			RiderModule target = new RiderModule(manager.Object,config.Object);
			Mock<IContainerProvider> container = new Mock<IContainerProvider>();
			target.OnInitialized(container.Object);
			manager.Verify(m => m.RegisterViewWithRegion(Regions.Console, typeof(Views.Console)));
			config.Verify(c => c.Load());
		}
	}
}