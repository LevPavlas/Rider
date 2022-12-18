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
		private ElevationDrawingContext? Context { get; set; }
		private List<ChallengeController> Challenges { get; }= new List<ChallengeController>();

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
				Context = new ElevationDrawingContext(canvas, RiderData);
				Draw();
			}
		}
		private void SetData(RiderData? data)
		{
			try
			{
				if (RiderData != null)
				{
					Context = new ElevationDrawingContext(canvas, RiderData);
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
				Clear();
				if (Context != null)
				{
					Axes.Draw(Context);
					DrawElevation();
					DrawChallenges();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
		void Clear()
		{
			foreach(ChallengeController challenge in Challenges)
			{
				challenge.Close();
			}
			Challenges.Clear();
			canvas.Children.Clear();
		}
		private void DrawChallenges()
		{
			if (RiderData == null || Context == null ) return;

			for ( int i =0; i<RiderData.Challenges.Count; i++)
			{
				ChallengeController controler = new ChallengeController(Context, i);
				Challenges.Add(controler);
				controler.Draw();
			}
		}

		private void OnPolygonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{

		}

		private void OnPolygonMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Point p = e.GetPosition(canvas);
			if (Context != null)
			{
				double dist = Context.ToModelDistance(p.X);
				//int index = RiderData.Route.GetPointIndex(dist);

				//	Console.WriteLine($"Index: {index}, distance:{dist}");
			}
		}

		private void OnPolygonMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
		}

		private void OnPolygonMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
		}

		private void OnPolygonMouseEnter(object sender, MouseEventArgs e)
		{
		}

		private void OnPolygonMouseLeave(object sender, MouseEventArgs e)
		{
		}

		private void OnPolygonMouseMove(object sender, MouseEventArgs e)
		{
		}

		private void DrawElevation()
		{

			if (RiderData== null || Context == null) return;
	
			PointCollection points = new PointCollection();

			points.Add(Context.ToCanvasPoint(0, Context.ModelNiceYmin));
			foreach (RoutePoint p in RiderData.Route.Points)
			{
				points.Add(Context.ToCanvasPoint(p.Distance, p.Elevation));
			}
			points.Add(Context.ToCanvasPoint(RiderData.Route.Distance, Context.ModelNiceYmin));

			GradientStopCollection gradientColection = new GradientStopCollection();

			gradientColection.Add(new GradientStop(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xE0), 0));
			gradientColection.Add(new GradientStop(Color.FromArgb(0xFF, 0x00, 0xB0, 0x00), 0.6));
			gradientColection.Add(new GradientStop(Color.FromArgb(0xFF, 0x00, 0x90, 0x00), 0.80));
			gradientColection.Add(new GradientStop(Color.FromArgb(0xFF, 0x00, 0x50, 0x00), 1));

			LinearGradientBrush fill = new LinearGradientBrush(gradientColection, 90);

			Polygon polygon = new Polygon
			{
				Stroke = ElevationBrush,
				StrokeThickness = 1,
				Fill = fill,
				Points = points
			};
			canvas.Children.Add(polygon);

		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			//Point p = e.GetPosition(canvas);
			//if(Context != null)
			//{
			//	double dist = Context.ToModelDistance(p.X);
			//	int index = RiderData.Route.GetPointIndex(dist);

			////	Console.WriteLine($"Index: {index}, distance:{dist}");
			//}
		}
	}
}
