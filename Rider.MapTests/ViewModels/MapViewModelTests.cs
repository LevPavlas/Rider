using CefSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using Prism.Regions;
using Rider.Contracts;
using Rider.Contracts.Events;
using Rider.Contracts.Services;
using Rider.Map.Events;
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
		Mock<IRegionManager> RegionManager { get; set; }= new Mock<IRegionManager>();
		Mock<IConfiguration> Configuration { get; set; }= new Mock<IConfiguration>();
		Mock<IEventAggregator> EventAggregator { get; set; }= new Mock<IEventAggregator>();
		Mock<IConsole> Console { get; set; }= new Mock<IConsole>();
		Mock<IFileSystem> FileSystem { get; set; } = new Mock<IFileSystem>();

		MapViewModel? Target { get; set; }

		[TestInitialize()]
		public void Setup()
		{
			RegionManager = new Mock<IRegionManager>();
			Configuration = new Mock<IConfiguration>();
			EventAggregator = new Mock<IEventAggregator>();
			Console = new Mock<IConsole>();
			FileSystem = new Mock<IFileSystem>();
		}
		MapViewModel CreateTarget()
		{
			return new MapViewModel(RegionManager.Object, EventAggregator.Object, Configuration.Object, Console.Object, FileSystem.Object);
		}

		[TestMethod()]
		public void HeaderTextTest()
		{
			MapViewModel target = CreateTarget();
			Assert.AreEqual("Map", target.HeaderText);
		}

		[TestMethod()]
		public void IsActivateChangedTest()
		{

			MapViewModel target = CreateTarget();
			bool activateChaged = false;
			target.IsActiveChanged += (sender, args) => { activateChaged = true; };
			target.IsActive = true;
			RegionManager.Verify(m => m.RequestNavigate(Constants.Regions.ToolBar, Constants.Views.MapToolBar));
			Assert.IsTrue(activateChaged);
		}

		[TestMethod()]
		public void OnDownloadUpdatedTest()
		{
			const string GpxDirectory = "GpxDirectory";
			const string SuggestedFileName = "SuggestedFileName";
			const string FilenameWithTimeStamp = "FilenameWithTimeStamp";
			MapViewModel target = CreateTarget();
			
			Configuration.Setup(c => c.GpxDirectory).Returns(GpxDirectory);

			FileSystem.Setup(f => f.AddTimeStamp($"{GpxDirectory}\\{SuggestedFileName}")).Returns(FilenameWithTimeStamp);
			string result =target.GetFullPathForDownload(SuggestedFileName);
			Assert.AreEqual(FilenameWithTimeStamp, result);

		}

	}
}