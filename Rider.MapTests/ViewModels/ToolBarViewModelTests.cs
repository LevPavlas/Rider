using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rider.Contracts.Services;
using Rider.Map.Events;

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
			MapToolBarViewModel target = new MapToolBarViewModel(config.Object,eventAgregator.Object );
			target.OnMapChanged(MapName);
			Assert.AreEqual(MapName, selectedMap);
		}
	}
}