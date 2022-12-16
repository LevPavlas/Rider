using Rider.Route.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rider.Route.UserControls
{
	internal class ChallengeController
	{
		private ElevationDrawingContext Context { get; }
		private Data.Route Route=>Context.Data.Route;
		private IReadOnlyList<ClimbChallenge> Challenges => Context.Data.Challenges;
		private Polygon? Polygon { get; set; }
		private ClimbChallenge Challenge { get; }
		
		bool IsMoving { get; set; } = false;
		double HalfOfStartMoveSize { get; set; }

		public ChallengeController(ElevationDrawingContext context, int challenge) 
		{;
			Context = context;
			Challenge = Challenges[challenge];
			Context.Canvas.MouseMove += OnCanvasMouseMove;
			Context.Canvas.MouseLeftButtonUp += OCanvasMouseLeftButtonUp;
			Context.Canvas.MouseLeave += OnCanvasMouseLeave;
		}


		public void Draw()
		{
			Context.Canvas.Children.Remove(Polygon);

			PointCollection points = new PointCollection();

			points.Add(Context.ToCanvasPoint(Challenge.StartPoint.Distance, Context.ModelNiceYmin));

			for (int i = Challenge.Start; i <= Challenge.End; i++)
			{
				points.Add(Context.ToCanvasPoint(Challenge.Points[i].Distance, Challenge.Points[i].Elevation));
			}
			points.Add(Context.ToCanvasPoint(Challenge.EndPoint.Distance, Context.ModelNiceYmin));

			GradientStopCollection gradientColection = new GradientStopCollection();

			gradientColection.Add(new GradientStop(Color.FromArgb(0x70, 0xA0, 0x00, 0x00), 0));
//			gradientColection.Add(new GradientStop(Color.FromArgb(0xFF, 0x00, 0xB0, 0x00), 0.6));
//			gradientColection.Add(new GradientStop(Color.FromArgb(0xFF, 0x00, 0x90, 0x00), 0.80));
			gradientColection.Add(new GradientStop(Color.FromArgb(0x00, 0x80, 0x00, 0x00), 1));

			LinearGradientBrush fill = new LinearGradientBrush(gradientColection, 90);


			Polygon = new Polygon
			{
				Stroke = new SolidColorBrush(Color.FromArgb(0xA0, 0xFF, 0x00, 0x00)),
				StrokeThickness = 1,
				Fill = new SolidColorBrush(Color.FromArgb(0x50, 0xFF, 0x00, 0x00)),
				Points = points,
			};

			Polygon.MouseMove += OnPolygonMouseMove;
			Polygon.MouseLeave += OnPolygonMouseLeave;
			Polygon.MouseEnter += OnPolygonMouseEnter;
			Polygon.MouseRightButtonDown += OnPolygonMouseRightButtonDown;
			Polygon.MouseRightButtonUp += OnPolygonMouseRightButtonUp;
			Polygon.MouseLeftButtonDown += OnPolygonMouseLeftButtonDown;
			Polygon.MouseLeftButtonUp += OnPolygonMouseLeftButtonUp;

			Context.Canvas.Children.Add(Polygon);
		}
		public void Close()
		{
			if(Polygon == null) return;
			
			Polygon.MouseMove -= OnPolygonMouseMove;
			Polygon.MouseLeave -= OnPolygonMouseLeave;
			Polygon.MouseEnter -= OnPolygonMouseEnter;
			Polygon.MouseRightButtonDown -= OnPolygonMouseRightButtonDown;
			Polygon.MouseRightButtonUp -= OnPolygonMouseRightButtonUp;
			Polygon.MouseLeftButtonDown -= OnPolygonMouseLeftButtonDown;
			Polygon.MouseLeftButtonUp -= OnPolygonMouseLeftButtonUp;

			Context.Canvas.MouseMove -= OnCanvasMouseMove;
			Context.Canvas.MouseLeftButtonUp -= OCanvasMouseLeftButtonUp;
			Context.Canvas.MouseLeave -= OnCanvasMouseLeave;

			Context.Canvas.Children.Remove(Polygon);


		}
		private void OnCanvasMouseMove(object sender, MouseEventArgs e)
		{
			if (!IsMoving) return;

			double mouseDistance = Context.ToModelDistance(e.GetPosition(Context.Canvas).X);
			if (mouseDistance < 0) return;
		//	Console.WriteLine($"Canvas mouse x:{mouseDistance}");
			double startDistance = mouseDistance - HalfOfStartMoveSize;
			double endDistance = mouseDistance + HalfOfStartMoveSize;

			if (startDistance < Context.ModelXmin)
			{
				startDistance = Context.ModelXmin;
				endDistance = Context.ModelXmin + 2 * HalfOfStartMoveSize;
			}
			if (endDistance > Context.ModelXmax)
			{
				startDistance = Context.ModelXmax - 2 * HalfOfStartMoveSize;
				endDistance = Context.ModelXmax;

			}

			Challenge.Start = (ushort)Context.Data.Route.GetPointIndex(startDistance);
			Challenge.End = (ushort)Context.Data.Route.GetPointIndex(endDistance);
			Draw();
		}
		private void OnCanvasMouseLeave(object sender, MouseEventArgs e)
		{
			StopMoving();
		}

		private void OCanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			StopMoving();
		}
		private void StopMoving()
		{
			if (IsMoving)
			{
				Context.Canvas.ReleaseMouseCapture();
				Context.Canvas.Cursor = Cursors.Arrow;
				IsMoving = false;
			}
		}
		private void OnPolygonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
		}

		private void OnPolygonMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			IsMoving = true;
			HalfOfStartMoveSize = (Challenge.EndPoint.Distance - Challenge.StartPoint.Distance) / 2;
			Context.Canvas.Cursor = Cursors.None;
			Context.Canvas.CaptureMouse();
		}
		private void OnPolygonMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
		}

		private void OnPolygonMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
		}

		private void OnPolygonMouseEnter(object sender, MouseEventArgs e)
		{
			if(Polygon != null)
			{
				//Polygon.Fill = new SolidColorBrush(Color.FromArgb(0x80, 0xFF, 0x80, 0x80));
			}
		}

		private void OnPolygonMouseLeave(object sender, MouseEventArgs e)
		{
		}

		private void OnPolygonMouseMove(object sender, MouseEventArgs e)
		{
		}

	}
}
