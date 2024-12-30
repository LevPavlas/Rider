using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using MapControl;
using Prism;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using Rider.Contracts.Events;
using Rider.Contracts.Services;
using Rider.Route.Data;

namespace Rider.Route.ViewModels
{
    internal class RouteViewModel : BindableBase, IActiveAware
	{
		public string HeaderText { get; } = "GPX";
		public MintPlayer.ObservableCollection.ObservableCollection<Location> RoutePath { get; } = new MintPlayer.ObservableCollection.ObservableCollection<Location>();
		public MintPlayer.ObservableCollection.ObservableCollection<Location> SelectedChallengePath { get; } = new MintPlayer.ObservableCollection.ObservableCollection<Location>();

		public event EventHandler? IsActiveChanged;

		private IRegionManager RegionManager { get; }
		private IEventAggregator EventAggregator { get; }
		public IConsole Console { get; }

		Location? _TargetCenter = null;
		public Location? TargetCenter
		{
			get=> _TargetCenter;
			set => SetProperty(ref _TargetCenter, value);
		}

		BoundingBox? _BoundingBox = null;
		public BoundingBox? BoundingBox
		{
			get => _BoundingBox;
			set => SetProperty(ref _BoundingBox, value);			
		}

		RiderData? _RiderData;
		public RiderData? RiderData
		{
			get => _RiderData;
			set => SetProperty(ref _RiderData, value);
		}
		private int DashAnimationPhase { get; set; } = 0;
		DispatcherTimer AnimationTimer { get; } 
		DoubleCollection _SelectedChallengeDash = new DoubleCollection();
		public DoubleCollection SelectedChallengeDash
		{
			get => _SelectedChallengeDash;
			set => SetProperty(ref _SelectedChallengeDash, value);
		}

		public RouteViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IConsole console)
		{
			RegionManager = regionManager;
			EventAggregator = eventAggregator;
			Console = console;
			EventAggregator.GetEvent<RiderDataCalculatedEvent>().Subscribe(OnDatatCalculated,ThreadOption.PublisherThread);
			AnimationTimer = new DispatcherTimer(DispatcherPriority.Render);
			AnimationTimer.Interval = TimeSpan.FromMilliseconds(50);
			AnimationTimer.Tick += OnAnimationTimer;
			AnimationTimer.Start();
			SelectedChallengePath.CollectionChanged += OnSelectedChallengePathCollectionChanged;

		}

		private void OnSelectedChallengePathCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			UpdateBoundingBox();
		}

		private DoubleCollection[] DashPhases { get; } = new[]
		{
			new DoubleCollection() { 0, 1, 3, 2 },
			new DoubleCollection() { 0, 2, 3, 1 },
			new DoubleCollection() { 0, 3, 3, 0},
			new DoubleCollection() { 1, 3, 2, 0},
			new DoubleCollection() { 2, 3, 1, 0},
			new DoubleCollection() { 3, 3},
		};
		private void OnAnimationTimer(object? sender, EventArgs e)
		{
			SelectedChallengeDash = DashPhases[DashAnimationPhase];
			DashAnimationPhase++;
			if (DashAnimationPhase >= DashPhases.Count())
			{
				DashAnimationPhase = 0;
			}
		}
		private void OnDatatCalculated(RiderData data)
		{
			RiderData= data;
			UpdateBoundingBox();
			UpdateRoutePath(data.Route.Points);

			Application.Current.Dispatcher.BeginInvoke(() =>
				{
					RegionManager.RequestNavigate(Constants.Regions.MainRegion, Constants.Views.Route);
				});
		}
		void UpdateRoutePath(IReadOnlyList<IPoint> points)
		{
			var locations = points.Select(p => new Location(p.Latitude, p.Longitude));
			RoutePath.Clear();
			RoutePath.AddRange(locations);
		}

		void UpdateBoundingBox()
		{
			IRoute? route = RiderData?.Route;
			if (route != null)
			{
				if (SelectedChallengePath.Count > 1)
				{
					BoundingBox = CreateSelectedChallengeBoundingBox(route);
				}
				else
				{
					BoundingBox = CreateRouteBoundingBox(route);
				}
			}		

		}
		BoundingBox CreateRouteBoundingBox(IRoute route)
		{
			double border =route.Distance /10000000;
				return new BoundingBox(
				route.LatitudeMinSouth - border,
				route.LongitudeMinWest - border,
				route.LatitudeMaxNorth + border,
				route.LongitudeMaxEast + border);

		}
		BoundingBox CreateSelectedChallengeBoundingBox(IRoute route)
		{
			double border = route.Distance / 20000000;

			double latMin = double.MaxValue;
			double latMax = double.MinValue;
			double lonMin = double.MaxValue;
			double lonMax = double.MinValue;

			foreach(Location loc in SelectedChallengePath)
			{
				latMin = latMin > loc.Latitude ? loc.Latitude : latMin;
				latMax = latMax < loc.Latitude? loc.Latitude:latMax;
				lonMin = lonMin > loc.Longitude ? loc.Longitude : lonMin;
				lonMax = lonMax < loc.Longitude ? loc.Longitude : lonMax;
			}
			double latBorder = 0.03 * Math.Abs(latMax - latMin);
			double lonBorder = 0.03 * Math.Abs(lonMax - lonMin);
			return new BoundingBox(latMin - latBorder, lonMin - lonBorder, latMax + latBorder, lonMax + latBorder);
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
