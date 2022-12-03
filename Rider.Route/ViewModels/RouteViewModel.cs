using MapControl;
using Prism;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Rider.Contracts.Events;
using Rider.Route.Data;
using Rider.Route.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rider.Route.ViewModels
{
	public class RouteViewModel : BindableBase, IActiveAware
	{
		public string HeaderText { get; } = "Route";
		public event Action<BoundingBox,MapPolyline>? BoundingBoxChanged;
		
		private IRegionManager RegionManager { get; }
		private IEventAggregator EventAggregator { get; }

		public event EventHandler? IsActiveChanged;
		public RouteViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
		{
			RegionManager = regionManager;
			EventAggregator = eventAggregator;
			EventAggregator.GetEvent<RiderDataCalculatedEvent>().Subscribe(OnDatatCalculated,ThreadOption.UIThread);
		}

		private void OnDatatCalculated(RiderData data)
		{
			RegionManager.RequestNavigate(Constants.Regions.MainRegion,Constants.Views.Route);
			BoundingBoxChanged?.Invoke(CreateBoundingBox(data.Route), CreateRoutePolyline(data.Route));
		}
		BoundingBox CreateBoundingBox(Data.Route route)
		{
			double border = 2 * route.Distance /10000000;
				return new BoundingBox(
				route.LatitudeMinSouth - border,
				route.LongitudeMinWest - border,
				route.LatitudeMaxNorth + border,
				route.LongitudeMaxEast + border);

		}
		MapPolyline CreateRoutePolyline(Data.Route route)
		{

			MapPolyline polygon = new MapPolyline();
			polygon.Stroke = new SolidColorBrush(Colors.Red);
			polygon.StrokeThickness = 3;
			polygon.Opacity = 0.7;
			LocationCollection locations = new LocationCollection();
			foreach (RoutePoint p in route.Points)
			{
				locations.Add(new Location(p.Latitude, p.Longitude));
			}
			polygon.Locations = locations;
			return polygon;
		}

		bool isActive = false;
		public bool IsActive
		{
			get
			{
				return isActive;
			}
			set
			{
				if (isActive != value)
				{
					if (value) Activate();
					isActive = value;
					IsActiveChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}
		void Activate()
		{
			RegionManager.RequestNavigate(Constants.Regions.ToolBar, Constants.Views.RouteToolBar );
		}
	}
}
