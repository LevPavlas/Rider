using MapControl;
using MapControl.Caching;
using Rider.Route.Data;
using Rider.Route.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Rider.Route.Views
{
	/// <summary>
	/// Interaction logic for Route.xaml
	/// </summary>
	public partial class Route : UserControl
	{
		private RouteViewModel? Model { get; set; }

		public Route()
		{
			DataContextChanged += OnDataContextChanged;

			ImageLoader.HttpClient.DefaultRequestHeaders.Add("User-Agent", "XAML Map Control Test Application");

			TileImageLoader.Cache = new ImageFileCache(TileImageLoader.DefaultCacheFolder);
			InitializeComponent();
				
			map.MapLayer = MapTileLayer.OpenStreetMapTileLayer;
			map.TargetCenter = new Location(120, 30);
	
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Model = e.NewValue as RouteViewModel;
			if(Model != null)
			{
				Model.BoundingBoxChanged += OnBoundingBoxChanged;
			}
		}
		MapPolyline LastPolyline { get; set; }
		private void OnBoundingBoxChanged(BoundingBox box, MapPolyline polygon)
		{
			map.ZoomToBounds( box);	
			map.Children.Remove( LastPolyline);
			map.Children.Add( polygon );
			LastPolyline= polygon;
		}
	

		private void MapMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				//BoundingBox box = new BoundingBox(,,);
				
				map.TargetCenter = new Location(49.358945, 17.361125);
				//map.ClipToBounds= true;
			//	map.ZoomToBounds
			//	map.

				//			map.MapLayer = MapTileLayer.OpenStreetMapTileLayer;

			//	map.TargetCenter = map.ViewToLocation(e.GetPosition(map));
			}
		}

		private void MapMouseMove(object sender, MouseEventArgs e)
		{
			var location = map.ViewToLocation(e.GetPosition(map));
			if (location != null)
			{
				var latitude = (int)Math.Round(location.Latitude * 60000d);
				var longitude = (int)Math.Round(Location.NormalizeLongitude(location.Longitude) * 60000d);
				var latHemisphere = 'N';
				var lonHemisphere = 'E';

				if (latitude < 0)
				{
					latitude = -latitude;
					latHemisphere = 'S';
				}

				if (longitude < 0)
				{
					longitude = -longitude;
					lonHemisphere = 'W';
				}

				//mouseLocation.Text = string.Format(CultureInfo.InvariantCulture,
				//	"{0}  {1:00} {2:00.000}\n{3} {4:000} {5:00.000}",
				//	latHemisphere, latitude / 60000, (latitude % 60000) / 1000d,
				//	lonHemisphere, longitude / 60000, (longitude % 60000) / 1000d);
			}
			else
			{
		//		mouseLocation.Text = string.Empty;
			}
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


	}
}
