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

namespace Rider.Route.UserControls
{
	internal class ElevationLabel
	{
		private ElevationDrawingContext Context { get; }
		private Canvas Canvas{get;}

		private TextBlock ElevationTextBlock { get; }

		public ElevationLabel(ElevationDrawingContext context)
		{
			Context = context;
			Canvas = Context.Canvas;
			ElevationTextBlock = new TextBlock();
			ElevationTextBlock.Foreground = new SolidColorBrush(Colors.Black);
		//	ElevationTextBlock.Background = new SolidColorBrush(Colors.White);
		//	ElevationTextBlock.Padding = new Thickness(3, 3, 3, 3);
			Context.Canvas.Children.Add(ElevationTextBlock);
			ElevationTextBlock.Visibility= Visibility.Hidden;
		}

		public void Draw(RoutePoint point)
		{
			Point p = Context.ToCanvasPoint(point.Distance, point.Elevation);
			ElevationTextBlock.Text = point.Elevation.ToString("0.0");

	
			Size size = MeasureString(ElevationTextBlock, ElevationTextBlock.Text);

			Canvas.SetLeft(ElevationTextBlock, p.X - size.Width / 2 );
			Canvas.SetTop(ElevationTextBlock, p.Y - 2* size.Height);
			ElevationTextBlock.Visibility = Visibility.Visible;
		}
		public void Hide()
		{
			ElevationTextBlock.Visibility = Visibility.Hidden;

		}
		public void Close()
		{
			Canvas.Children.Remove( ElevationTextBlock );
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
