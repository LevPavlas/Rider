using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using Prism.Regions;
using Rider.Contracts;
using Rider.Map.Events;
using Rider.Map.ViewModels;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Map.ViewModels.Tests
{
	[TestClass()]
	public class ToolBarViewModelTests
	{
		[TestMethod()]
		public void OnMapChangedTest()
		{
			const string MapName = "MyMap";
			string selectedMap = "";
			MapChangedEvent mapChangeEvent = new MapChangedEvent();
			mapChangeEvent.Subscribe((map) => selectedMap = map);

			var config = new Mock<IConfiguration>();
			var eventAgregator = new Mock<IEventAggregator>();
			eventAgregator.Setup(e => e.GetEvent<MapChangedEvent>()).Returns(mapChangeEvent);
			ToolBarViewModel target = new ToolBarViewModel(config.Object,eventAgregator.Object );
			target.OnMapChanged(MapName);
			Assert.AreEqual(MapName, selectedMap);
		}
	}
}