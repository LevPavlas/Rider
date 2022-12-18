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
	internal enum PolygonArea
	{
		None,
		Over,
		LeftBorder,
		RightBorder,
	}
	internal enum Mode
	{
		None,
		Move,
		ResizeLeft,
		ResizeRight
	}
	internal readonly record struct MousePosition(double Distance, double Offset, PolygonArea Area); 

	internal class ChallengeController
	{
	
		private ElevationDrawingContext Context { get; }
		private Polygon Polygon { get; }
		private ClimbChallenge Challenge { get; }

		private Mode Mode { get; set; } = Mode.None;

		public ChallengeController(ElevationDrawingContext context, int challenge) 
		{			
			Context = context;
			Challenge = context.Data.Challenges[challenge];
			Polygon = CreatePolygon();
		}

		private double ActionStartMouseLeftOffset { get; set; } = 0;
		private double ActionStartMouseRightOffset { get; set; } = 0;
		private double ActionStartSize { get; set; } = 0;

		MousePosition _mousePosition = new MousePosition( 0, 0,PolygonArea.None);
		private MousePosition MousePosition
		{
			get => _mousePosition;
			set
			{
				MousePosition old = _mousePosition;
				_mousePosition = value;
				if(old !=_mousePosition) 
				{
					OnMousePositionChanged();
				}
			}
		}
		private Cursor MouseEnterCursor { get; set; } = Cursors.Arrow;

		void OnMousePositionChanged()
		{
			if(Mode != Mode.None)
			{
				return;
			}
			switch(MousePosition.Area)
			{
				case PolygonArea.Over:
					Polygon.Cursor = Cursors.ScrollWE;
					break;
				case PolygonArea.LeftBorder:
					Polygon.Cursor = Cursors.ScrollW;
					break;
				case PolygonArea.RightBorder:
					Polygon.Cursor = Cursors.ScrollE;
					break;
				default:
					Polygon.Cursor = Cursors.Arrow;
					break;
			}
		}
		public void Draw()
		{
			SetPolygonPath();
		}
		Polygon CreatePolygon()
		{

			GradientStopCollection gradientColection = new GradientStopCollection();

			gradientColection.Add(new GradientStop(Color.FromArgb(0x70, 0xA0, 0x00, 0x00), 0));
			//			gradientColection.Add(new GradientStop(Color.FromArgb(0xFF, 0x00, 0xB0, 0x00), 0.6));
			//			gradientColection.Add(new GradientStop(Color.FromArgb(0xFF, 0x00, 0x90, 0x00), 0.80));
			gradientColection.Add(new GradientStop(Color.FromArgb(0x00, 0x80, 0x00, 0x00), 1));

			LinearGradientBrush fill = new LinearGradientBrush(gradientColection, 90);


			Polygon polygon = new Polygon
			{
				Stroke = new SolidColorBrush(Color.FromArgb(0xA0, 0xFF, 0x00, 0x00)),
				StrokeThickness = 1,
				Fill = new SolidColorBrush(Color.FromArgb(0x50, 0xFF, 0x00, 0x00)),
			};

			polygon.MouseMove += OnPolygonMouseMove;
			polygon.MouseLeave += OnPolygonMouseLeave;
			polygon.MouseEnter += OnPolygonMouseEnter;
			polygon.MouseRightButtonDown += OnPolygonMouseRightButtonDown;
			polygon.MouseRightButtonUp += OnPolygonMouseRightButtonUp;
			polygon.MouseLeftButtonDown += OnPolygonMouseLeftButtonDown;
			polygon.MouseLeftButtonUp += OnPolygonMouseLeftButtonUp;
			
			Context.Canvas.Children.Add(polygon);
			return polygon;
		}

		private void SetPolygonPath()
		{
			PointCollection points = new PointCollection
			{
				Context.ToCanvasPoint(Challenge.StartPoint.Distance, Context.ModelNiceYmin)
			};

			for (int i = Challenge.Start; i <= Challenge.End; i++)
			{
				points.Add(Context.ToCanvasPoint(Challenge.Points[i].Distance, Challenge.Points[i].Elevation));
			}
			points.Add(Context.ToCanvasPoint(Challenge.EndPoint.Distance, Context.ModelNiceYmin));

			Polygon.Points = points;
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

			Context.Canvas.Children.Remove(Polygon);
		}

		private void OnPolygonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			StopAction();
			RefreshMousePosition(e);
		}
		private void StopAction()
		{
			if (Mode != Mode.None)
			{
				Polygon.ReleaseMouseCapture();
				Mode = Mode.None;
			}
		}

		private void OnPolygonMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			RefreshMousePosition(e);
			StartAction();
		}
		private void StartAction()
		{
			switch(MousePosition.Area)
			{
				case PolygonArea.Over:
					Mode = Mode.Move;
					break;
				case PolygonArea.LeftBorder:
					Mode = Mode.ResizeLeft;
					break;
				case PolygonArea.RightBorder:
					Mode = Mode.ResizeRight;
					break;
				default:
					return;
			}
	
			ActionStartMouseLeftOffset = MousePosition.Offset;
			ActionStartMouseRightOffset = Challenge.EndPoint.Distance - MousePosition.Distance;
			ActionStartSize = Challenge.Size;
			Polygon.Cursor = Cursors.None;
			Polygon.CaptureMouse();
		}

		private void OnPolygonMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
		}

		private void OnPolygonMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
		}

		private void OnPolygonMouseEnter(object sender, MouseEventArgs e)
		{
			MouseEnterCursor = Context.Canvas.Cursor;
			RefreshMousePosition(e);
		}

		private void OnPolygonMouseLeave(object sender, MouseEventArgs e)
		{
			RefreshMousePosition(e);
			Context.Canvas.Cursor = MouseEnterCursor;
		}

		private void OnPolygonMouseMove(object sender, MouseEventArgs e)
		{
			RefreshMousePosition(e);
			double mouseDistance = MousePosition.Distance;
			if (mouseDistance < 0) return;

			switch (Mode)
			{
				case Mode.None:
					break;
				case Mode.Move:
					OnMove(mouseDistance);
					break;
				case Mode.ResizeLeft:
					OnResizeLeft(mouseDistance);
					break;
				case Mode.ResizeRight:
					OnResizeRight(mouseDistance);
					break;
			}

		}
		private void OnResizeRight(double mouseDistance)
		{
			double endDistance = mouseDistance + ActionStartMouseRightOffset;
			if (endDistance < Context.ModelXmin)
			{
				endDistance = Context.ModelXmin;
			}
			if (endDistance  <= Challenge.StartPoint.Distance) return;
	
			Challenge.End = (ushort)Context.Data.Route.GetPointIndex(endDistance);
			Draw();

		}

		private void OnResizeLeft(double mouseDistance)
		{
			double startDistance = mouseDistance - ActionStartMouseLeftOffset;
			if (startDistance < Context.ModelXmin)
			{
				startDistance = Context.ModelXmin;
			}
			if (startDistance >= Challenge.EndPoint.Distance) return;

			Challenge.Start = (ushort)Context.Data.Route.GetPointIndex(startDistance);
			Draw();

		}
		private void OnMove(double mouseDistance)
		{
			double startDistance = mouseDistance - ActionStartMouseLeftOffset;
			double endDistance = mouseDistance + ActionStartMouseRightOffset;

			if (startDistance < Context.ModelXmin)
			{
				startDistance = Context.ModelXmin;
				endDistance = Context.ModelXmin + ActionStartSize;
			}
			if (endDistance > Context.ModelXmax)
			{
				startDistance = Context.ModelXmax - ActionStartSize;
				endDistance = Context.ModelXmax;

			}

			Challenge.Start = (ushort)Context.Data.Route.GetPointIndex(startDistance);
			Challenge.End = (ushort)Context.Data.Route.GetPointIndex(endDistance);
			Draw();
		}

		void RefreshMousePosition(MouseEventArgs e)
		{
			Point canvasPosition = e.GetPosition(Context.Canvas);
			double mouseDistance = Context.ToModelDistance(canvasPosition.X);
			PolygonArea area = PolygonArea.None;

			if(mouseDistance >= Challenge.StartPoint.Distance && mouseDistance<= Challenge.EndPoint.Distance)
			{
				area = PolygonArea.Over;
				if(mouseDistance - Challenge.StartPoint.Distance < Context.ModelResizeBorder) 
				{
					area = PolygonArea.LeftBorder;
				}
				else if(Challenge.EndPoint.Distance -mouseDistance < Context.ModelResizeBorder)
				{
					area = PolygonArea.RightBorder;
				}
			}
			MousePosition = new MousePosition( mouseDistance, mouseDistance-Challenge.StartPoint.Distance, area);
		}
	}
}
