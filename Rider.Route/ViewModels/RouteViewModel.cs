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
	internal class RouteViewModel : BindableBase, IActiveAware
	{
		public string HeaderText { get; } = "Route";
		public MintPlayer.ObservableCollection.ObservableCollection<Location> RoutePath { get; } = new MintPlayer.ObservableCollection.ObservableCollection<Location>();
		
		public event EventHandler? IsActiveChanged;

		private IRegionManager RegionManager { get; }
		private IEventAggregator EventAggregator { get; }
		public IConsole Console { get; }

		BoundingBox? _BoundingBox = null;
		public BoundingBox? BoundingBox 
		{
			get => _BoundingBox;
			set
			{
				SetProperty(ref _BoundingBox, value);
			}
		}

		RiderData? _RiderData;
		public RiderData? RiderData
		{
			get => _RiderData;
			set=> SetProperty(ref _RiderData, value);
		}

		public RouteViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IConsole console)
		{
			RegionManager = regionManager;
			EventAggregator = eventAggregator;
			Console = console;
			EventAggregator.GetEvent<RiderDataCalculatedEvent>().Subscribe(OnDatatCalculated,ThreadOption.PublisherThread);
			
		}
		private void OnDatatCalculated(RiderData data)
		{
			RiderData= data;
			BoundingBox = CreateBoundingBox(data.Route);
			UpdateRoutePath(data.Route.Points);

			Application.Current.Dispatcher.BeginInvoke(() =>
				{
					RegionManager.RequestNavigate(Constants.Regions.MainRegion, Constants.Views.Route);
				});
		}
		void UpdateRoutePath(IReadOnlyList<RoutePoint> points)
		{
			var locations = points.Select(p => new Location(p.Latitude, p.Longitude));
			RoutePath.Clear();
			RoutePath.AddRange(locations);
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
