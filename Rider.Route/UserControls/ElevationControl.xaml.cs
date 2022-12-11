using MapControl;
using Rider.Contracts.Services;
using Rider.Route.Data;
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
	/// Interaction logic for ElevationControl.xaml
	/// </summary>
	public partial class ElevationControl : UserControl
	{
		public static readonly DependencyProperty RiderDataProperty = DependencyProperty.Register(
					"RiderData",
					typeof(Data.RiderData),
					typeof(ElevationControl),
					new PropertyMetadata(null, new PropertyChangedCallback(OnRiderDataChanged)));

		internal RiderData RiderData
		{
			get { return (RiderData)GetValue(RiderDataProperty); }
			set { SetValue(RiderDataProperty, value); }
		}

		private static void OnRiderDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ElevationControl? ctrl = d as ElevationControl;
			
			ctrl?.SetData(e.NewValue as RiderData);
		}
		private Brush ElevationBrush { get; } = new SolidColorBrush(Colors.Black);

		private Axes Axes { get; }
		private ElevationDrawingData? DrawingData { get; set; }
		public ElevationControl()
		{
			InitializeComponent();
			Loaded += OnLoaded;
			Axes = new Axes(canvas);
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
		}


		private void OnControlSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (RiderData != null)
			{
				DrawingData = new ElevationDrawingData(canvas, RiderData);
				Draw();
			}
		}
		private void SetData(RiderData? data)
		{
			try
			{
				if (RiderData != null)
				{
					DrawingData = new ElevationDrawingData(canvas, RiderData);
					Draw();
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
	
		}
		private void Draw()
		{
			try
			{
				canvas.Children.Clear();
				if (DrawingData != null)
				{
					Axes.Draw(DrawingData);
					DrawElevation();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
		private void DrawElevation()
		{

			if (RiderData== null || DrawingData == null) return;
	
			PointCollection points = new PointCollection();

			points.Add(DrawingData.ToCanvasPoint(0, DrawingData.ModelNiceYmin));
			foreach (RoutePoint p in RiderData.Route.Points)
			{
				points.Add(DrawingData.ToCanvasPoint(p.Distance, p.Elevation));
			}
			points.Add(DrawingData.ToCanvasPoint(RiderData.Route.Distance, DrawingData.ModelNiceYmin));

			GradientStopCollection gradientColection = new GradientStopCollection();
			gradientColection.Add(new GradientStop(Colors.LightYellow, 0));
			gradientColection.Add(new GradientStop(Color.FromArgb(0xFF, 0x00, 0x80, 0x00), 0.6));
			gradientColection.Add(new GradientStop(Color.FromArgb(0xFF, 0x00, 0x60, 0x00), 0.8));
			gradientColection.Add(new GradientStop(Color.FromArgb(0xFF, 0x00, 0x20, 0x00), 1));

			LinearGradientBrush fill = new LinearGradientBrush(gradientColection, 90);

			Polygon line = new Polygon
			{
				Stroke = ElevationBrush,
				StrokeThickness = 0.5,
				Fill = fill,
				Points = points
			};
			canvas.Children.Add(line);

		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			Point p = e.GetPosition(canvas);
			if(DrawingData != null)
			{
				double dist = DrawingData.ToModelDistance(p.X);
				int index = RiderData.Route.GetPointIndex(dist);

			//	Console.WriteLine($"Index: {index}, distance:{dist}");
			}
		}
	}
}
