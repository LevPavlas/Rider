using MapControl;
using MapControl.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rider.Route.UserControls
{
	/// <summary>
	/// Interaction logic for RouteMapControl.xaml
	/// </summary>
	public partial class RouteMapControl : UserControl
	{
		public static readonly DependencyProperty RoutePathProperty = DependencyProperty.Register(
				"RoutePath",
				typeof(MintPlayer.ObservableCollection.ObservableCollection<Location>),
				typeof(RouteMapControl),
				new PropertyMetadata(null, new PropertyChangedCallback(OnRoutePathChanged)));

		public MintPlayer.ObservableCollection.ObservableCollection<Location> RoutePath
		{
			get { return (MintPlayer.ObservableCollection.ObservableCollection<Location>)GetValue(RoutePathProperty); }
			set { SetValue(RoutePathProperty, value); }
		}

		private static void OnRoutePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RouteMapControl? route = d as RouteMapControl;
			route?.OnRoutePathChanged(e);
		}

		public static readonly DependencyProperty TargetCenterProperty = DependencyProperty.Register(
		"TargetCenter",
		typeof(Location),
		typeof(RouteMapControl),
		new PropertyMetadata(null, new PropertyChangedCallback(OnTargetCenterChanged)));

		private static void OnTargetCenterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RouteMapControl? route = d as RouteMapControl;
			route?.OnTargetCenterChanged(e);
		}
		public Location? TargetCenter
		{
			get { return GetValue(TargetCenterProperty) as Location; }
			set { SetValue(TargetCenterProperty, value); }
		}

		public static readonly DependencyProperty BoundingBoxProperty = DependencyProperty.Register(
			"BoundingBox",
			typeof(BoundingBox),
			typeof(RouteMapControl),
			new PropertyMetadata(null, new PropertyChangedCallback(OnBoundingBoxChanged)));
		public BoundingBox BoundingBox
		{
			get { return (BoundingBox)GetValue(BoundingBoxProperty); }
			set { SetValue(BoundingBoxProperty, value); }
		}

		private static void OnBoundingBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RouteMapControl? route = d as RouteMapControl;
			route?.OnBoundingBoxChanged(e);
		}

	
		public RouteMapControl()
		{
			InitializeComponent();
			map.MapLayer = MapTileLayer.OpenStreetMapTileLayer;
			map.TargetCenter = new Location(120, 30);
			map.Loaded += OnMapLoaded;
		}

		private void OnMapLoaded(object sender, RoutedEventArgs e)
		{
			if (BoundingBox != null)
			{
				map.ZoomToBounds(BoundingBox);
			}
			else
			{
				map.Center = new Location(60.0, -10.0);
				map.ZoomLevel = 3;
				map.Heading = 0;
			}

		}

		public static MapTileLayer OpenTopoMapTileLayer => new MapTileLayer
		{
			TileSource = new TileSource { UriTemplate = "https://tile.opentopomap.org/{z}/{x}/{y}.png" },
			SourceName = "OpenTopoMap",
			Description = "© [OpenTopoMap](https://opentopomap.org/) © [OpenStreetMap contributors](http://www.openstreetmap.org/copyright)"
		};


		private void OnTargetCenterChanged(DependencyPropertyChangedEventArgs e)
		{
			Location? location = e.NewValue as Location;
			if (location != null)
			{
				map.Center = location;
				map.ZoomLevel = 15;
				map.Heading = 0;
			}
			else if(BoundingBox != null)
			{
				map.ZoomToBounds(BoundingBox);
			}
		}
		private void OnBoundingBoxChanged(DependencyPropertyChangedEventArgs e)
		{
			if (TargetCenter == null)
			{
				BoundingBox? box = e.NewValue as BoundingBox;
				if (box != null) map.ZoomToBounds(box);
			}
		}

		private void OnRoutePathChanged(DependencyPropertyChangedEventArgs e)
		{
			MintPlayer.ObservableCollection.ObservableCollection<Location>? path = e.NewValue as MintPlayer.ObservableCollection.ObservableCollection<Location>;
			routePath.Locations = path;
		}

		private void MapManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
		{
			e.TranslationBehavior.DesiredDeceleration = 0.001;
		}

		private void MapItemTouchDown(object sender, TouchEventArgs e)
		{
			var mapItem = (MapItem)sender;
			mapItem.IsSelected = !mapItem.IsSelected;
			e.Handled = true;
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (BoundingBox != null)
			{
				map.ZoomToBounds(BoundingBox);
			}
		}
	}
}
