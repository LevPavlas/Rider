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
		private void OnRoutePathChanged(DependencyPropertyChangedEventArgs e)
		{
			MintPlayer.ObservableCollection.ObservableCollection<Location>? path = e.NewValue as MintPlayer.ObservableCollection.ObservableCollection<Location>;
			routePath.Locations = path;
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

		private void OnBoundingBoxChanged(DependencyPropertyChangedEventArgs e)
		{
			BoundingBox? box = e.NewValue as BoundingBox;
			if(box!= null) map.ZoomToBounds(box);
		}
	
		public RouteMapControl()
		{
				InitializeComponent();
//			map.MapLayer = MapTileLayer.OpenStreetMapTileLayer;
			map.MapLayer = MapTileLayer.OpenStreetMapTileLayer;
			map.TargetCenter = new Location(120, 30);

		}

		private bool FirstLoad { get; set; } = true;
		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (FirstLoad)
			{

				FirstLoad = false;
				if (BoundingBox!= null)
				{
					map.ZoomToBounds(BoundingBox);
				}
				else
				{
					map.Center = new Location(60.0, -10.0);
					map.ZoomLevel = 2;
					map.Heading = 0;
				}
			}
		}



		private void MapMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				//BoundingBox box = new BoundingBox(,,);

				//map.TargetCenter = new Location(49.358945, 17.361125);
				//map.ClipToBounds= true;
				//	map.ZoomToBounds
				//	map.

				//			map.MapLayer = MapTileLayer.OpenStreetMapTileLayer;

				//	map.TargetCenter = map.ViewToLocation(e.GetPosition(map));
			}
		}

		private void MapMouseMove(object sender, MouseEventArgs e)
		{
			//	var location = map.ViewToLocation(e.GetPosition(map));
			//	if (location != null)
			//	{
			//		var latitude = (int)Math.Round(location.Latitude * 60000d);
			//		var longitude = (int)Math.Round(Location.NormalizeLongitude(location.Longitude) * 60000d);
			//		var latHemisphere = 'N';
			//		var lonHemisphere = 'E';

			//		if (latitude < 0)
			//		{
			//			latitude = -latitude;
			//			latHemisphere = 'S';
			//		}

			//		if (longitude < 0)
			//		{
			//			longitude = -longitude;
			//			lonHemisphere = 'W';
			//		}

			//		//mouseLocation.Text = string.Format(CultureInfo.InvariantCulture,
			//		//	"{0}  {1:00} {2:00.000}\n{3} {4:000} {5:00.000}",
			//		//	latHemisphere, latitude / 60000, (latitude % 60000) / 1000d,
			//		//	lonHemisphere, longitude / 60000, (longitude % 60000) / 1000d);
			//	}
			//	else
			//	{
			////		mouseLocation.Text = string.Empty;
			//	}
		}

		private void MapMouseLeave(object sender, MouseEventArgs e)
		{
			//	mouseLocation.Text = string.Empty;
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
