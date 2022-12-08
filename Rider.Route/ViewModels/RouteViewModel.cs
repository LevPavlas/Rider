using MapControl;
using Prism;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Rider.Contracts.Events;
using Rider.Contracts.Services;
using Rider.Route.Data;
using Rider.Route.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rider.Route.ViewModels
{
	public class RouteViewModel : BindableBase, IActiveAware
	{
		public string HeaderText { get; } = "Route";
		public event Action<BoundingBox,MapPolyline>? RouteChanged;

		public MintPlayer.ObservableCollection.ObservableCollection<Location> RoutePath { get; } = new MintPlayer.ObservableCollection.ObservableCollection<Location>();

		public event EventHandler? IsActiveChanged;

		private IRegionManager RegionManager { get; }
		private IEventAggregator EventAggregator { get; }
		public IConsole Console { get; }

		public RouteViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IConsole console)
		{
			RegionManager = regionManager;
			EventAggregator = eventAggregator;
			Console = console;
			EventAggregator.GetEvent<RiderDataCalculatedEvent>().Subscribe(OnDatatCalculated,ThreadOption.PublisherThread);
			
		//	BindingOperations.EnableCollectionSynchronization(RoutePath, _lock);
		//	Application.Current.Dispatcher.Invoke(() => BindingOperations.EnableCollectionSynchronization(_RoutePath, _lock,OnCollectionSynchronizationCallback ));
		}
		private void OnDatatCalculated(RiderData data)
		{
			Console.WriteLine($"Start Paintin gpx{DateTime.Now}");

			var locations = data.Route.Points.Select(p => new Location(p.Latitude, p.Longitude)).ToArray();
//			Application.Current.Dispatcher.BeginInvoke(() =>
//			{
				Console.WriteLine($"Invoke gpx{DateTime.Now}");
				RouteChanged?.Invoke(CreateBoundingBox(data.Route), null);
				Console.WriteLine($"Route path gpx{DateTime.Now}");
				RoutePath.Clear();
//				foreach(RoutePoint p in data.Route.Points)
				{
					RoutePath.AddRange(locations);
				}

				Console.WriteLine($"Route path gpx{DateTime.Now}");
//			});

			Console.WriteLine($"Stop Paintin gpx{DateTime.Now}");

					Application.Current.Dispatcher.BeginInvoke(() =>
						{
			RegionManager.RequestNavigate(Constants.Regions.MainRegion, Constants.Views.Route);
						});
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
			polygon.Stroke = new SolidColorBrush(Colors.Blue);
			polygon.StrokeThickness = 3;
			polygon.Opacity = 0.7;
			LocationCollection locations = new LocationCollection();
			foreach (RoutePoint p in route.Points)
			{
				locations.Add(new Location(p.Latitude, p.Longitude));
			}
			polygon.Locations = locations;
			polygon.MouseMove += Polygon_MouseMove;
			return polygon;
		}

		private void Polygon_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
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
