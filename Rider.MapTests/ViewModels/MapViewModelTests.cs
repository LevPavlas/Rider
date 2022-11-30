using CefSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using Prism.Regions;
using Rider.Contracts;
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

			MapViewModel target = CreateTarget();

			DownloadItem item = new DownloadItem();
			var chrominium = new Mock<IWebBrowser>();
			var browser = new Mock<IBrowser>();
			var callback = new Mock<IDownloadItemCallback>();

			Configuration.Setup(c => c.GpxDirectory).Returns(GpxDirectory);
			item.SuggestedFileName = SuggestedFileName;
			item.IsValid = true;
			item.IsComplete = true;
			item.FullPath = $"Gpx downloaded: {GpxDirectory}\\{SuggestedFileName}";

			EventAggregator.Setup(a => a.GetEvent<GpxDownloadedEvent>()).Returns(new GpxDownloadedEvent());

			target.OnDownloadUpdated(chrominium.Object, browser.Object, item, callback.Object);
			Console.Verify(c => c.WriteLine($"Gpx downloaded: {GpxDirectory}\\{SuggestedFileName}"));
			EventAggregator.Verify(a => a.GetEvent<GpxDownloadedEvent>());

		}

		[TestMethod()]
		public void CanDownloadTest()
		{
			MapViewModel target = CreateTarget();

			var chrominium = new Mock<IWebBrowser>();
			var browser = new Mock<IBrowser>();
						
			Assert.IsTrue(target.CanDownload(chrominium.Object, browser.Object, "", ""));
		}

		[TestMethod()]
		public void OnBeforeDownloadTest()
		{
			const string GpxDirectory = "GpxDirectory";
			const string SuggestedFileName = "SuggestedFileName";
			const string TimeStamp = "TimeStamp";

			MapViewModel target = CreateTarget();

			DownloadItem item = new DownloadItem();
			var chrominium = new Mock<IWebBrowser>();
			var browser = new Mock<IBrowser>();
			var callback = new Mock<IBeforeDownloadCallback>();

			Configuration.Setup(c => c.GpxDirectory).Returns(GpxDirectory);
			item.SuggestedFileName = SuggestedFileName;
			callback.Setup(c => c.Continue($"{GpxDirectory}\\{SuggestedFileName}", false));
			FileSystem.Setup(f => f.AddTimeStamp($"{GpxDirectory}\\{SuggestedFileName}")).Returns($"{GpxDirectory}\\{SuggestedFileName}_{TimeStamp}");
			target.OnBeforeDownload(chrominium.Object,browser.Object,item,callback.Object);
			callback.Verify(c => c.Continue($"{GpxDirectory}\\{SuggestedFileName}_{TimeStamp}", false));
		}
	}
}