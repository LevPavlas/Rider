using Rider.Route.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Rider.Route.UserControls
{
	internal class ElevationLabel
	{
		private ElevationDrawingContext Context { get; }
		private Canvas Canvas{get;}

		private TextBlock ElevationTextBlock { get; }
		private Border Border { get; }
		public ElevationLabel(ElevationDrawingContext context)
		{
			Context = context;
			Canvas = Context.Canvas;
			ElevationTextBlock = new TextBlock();
			ElevationTextBlock.Foreground = new SolidColorBrush(Colors.Black);

			//	ElevationTextBlock.Background = new SolidColorBrush(Colors.White);
			//ElevationTextBlock.Padding = new Thickness(4, 3, 4, 3);
			//ElevationTextBlock.Text = "000.0";
			//ElevationTextBlock.FontSize= 16;
			//Context.Canvas.Children.Add(ElevationTextBlock);
			//ElevationTextBlock.Visibility= Visibility.Hidden;
			
			Border = new Border();
			Border.CornerRadius = new CornerRadius(3);
			Border.BorderBrush= new SolidColorBrush(Colors.Black);
			Border.BorderThickness = new Thickness(1);
			Border.Background = new SolidColorBrush(Colors.LightYellow);
			Border.Effect = new DropShadowEffect
			{
				Opacity = 0.6,
			};
	//		Border.Child = ElevationTextBlock;
			Border.Visibility = Visibility.Hidden;
			ElevationTextBlock.Visibility= Visibility.Hidden;

			Canvas.Children.Add(Border);
			Canvas.Children.Add(ElevationTextBlock);
		}

		public void Draw(RoutePoint point)
		{

			Point p = Context.ToCanvasPoint(point.Distance, point.Elevation);
			ElevationTextBlock.Text = point.Elevation.ToString("0.0");
			ElevationTextBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

			Size size = ElevationTextBlock.DesiredSize;

			Canvas.SetLeft(ElevationTextBlock, p.X - size.Width / 2);
			Canvas.SetTop(ElevationTextBlock, p.Y - 2 * size.Height);
			Border.Width= 1.3* size.Width;
			Border.Height= 1.2* size.Height;
			Canvas.SetLeft(Border, p.X - Border.Width / 2);
			Canvas.SetTop(Border, p.Y - 1.7 * Border.Height);
			
			ElevationTextBlock.Visibility = Visibility.Visible;
			Border.Visibility = Visibility.Visible;
		}
		public void Hide()
		{
			Border.Visibility = Visibility.Hidden;
			ElevationTextBlock.Visibility = Visibility.Hidden;

		}
		public void Close()
		{
			Canvas.Children.Remove(Border);
			Canvas.Children.Remove(ElevationTextBlock);
		}
		private Size MeasureString(TextBlock textBlock, string text)
		{
			var formattedText = new FormattedText(
				text,
				CultureInfo.CurrentCulture,
				FlowDirection.LeftToRight,
				new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
				textBlock.FontSize,
				Brushes.Black,
				new NumberSubstitution(),
				1);

			return new Size(formattedText.Width, formattedText.Height);
		}


	}
}
