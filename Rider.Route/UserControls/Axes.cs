using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rider.Route.UserControls
{

	internal class Axes
    {
		const double StrokeThickness = 1;
		const double ScaleMarkSize = 2.5;

		//	const double Ratio = 0.1;
		private Brush Brush { get; } = new SolidColorBrush(Colors.Black);
		
		private Canvas Canvas { get; }
	
		public Axes(Canvas canvas)
		{
			Canvas = canvas;
		}

		public void Draw(ElevationDrawingContext info)
		{
			DrawXAxis(info);
			DrawYAxis(info);
		}
		private void DrawYAxis(ElevationDrawingContext info) 
		{
			Line yAxis = new Line
			{
				X1 = info.CanvasXmin,
				Y1 = info.CanvasYmin,
				X2 = info.CanvasXmin,
				Y2 = info.CanvasYmax + 5,
				Stroke = Brush,
				StrokeThickness = StrokeThickness,
			};
			Canvas.Children.Add(yAxis);

			if (!double.IsNormal(info.ModelSpacingY)) return;

			double markX1 = info.CanvasXmin - ScaleMarkSize;
			double markX2 = info.CanvasXmin + ScaleMarkSize;

			decimal spacing = Convert.ToDecimal(info.ModelSpacingY);
			decimal maxY = Convert.ToDecimal(info.ModelNiceYmax);
			decimal offsetY = (decimal)info.ModelNiceYmin;

			for (decimal y = offsetY; y <= maxY; y += spacing)
			{
				double posY = info.CanvasYmax - (double)(y- offsetY) * info.RatioY ;
				Line mark = new Line
				{
					X1 = markX1,
					Y1 = posY,
					X2 = markX2,
					Y2 = posY,
					Stroke = Brush,
					StrokeThickness = StrokeThickness,
				};
				Canvas.Children.Add(mark);
				TextY(markX1, posY, y.ToString(), Colors.Black);
			}

		}
		private void TextY(double x, double y, string text, Color color)
		{
			TextBlock textBlock = new TextBlock();

			textBlock.Text = text;
			textBlock.Foreground = new SolidColorBrush(color);

			Size size = MeasureString(textBlock, text);

			Canvas.SetLeft(textBlock, x - size.Width - 10);
			Canvas.SetTop(textBlock, y - size.Height/2 -2);

			Canvas.Children.Add(textBlock);

		}

		private void DrawXAxis(ElevationDrawingContext info)
		{
			Line xAxis = new Line
			{
				X1 = info.CanvasXmin - 5,
				Y1 = info.CanvasYmax,
				X2 = info.CanvasXmax,
				Y2 = info.CanvasYmax,
				Stroke = Brush,
				StrokeThickness = StrokeThickness,
			};
			
			Canvas.Children.Add(xAxis);

			if (!double.IsNormal(info.ModelSpacingX)) return;

			double markY1 = info.CanvasYmax - ScaleMarkSize;
			double markY2 = info.CanvasYmax + ScaleMarkSize;

			decimal spacing = Convert.ToDecimal(info.ModelSpacingX);
			decimal maxX = Convert.ToDecimal(info.ModelNiceXmax);
			decimal offsetX = (decimal)info.ModelNiceXmin;

			for (decimal x = offsetX; x <= maxX; x += spacing)
			{
				double posX = (double)(x- offsetX) * info.RatioX + info.CanvasXmin;
				Line mark = new Line
				{
					X1 = posX,
					Y1 = markY1,
					X2 = posX,
					Y2 = markY2,
					Stroke = Brush,
					StrokeThickness = StrokeThickness,
				};
				Canvas.Children.Add(mark);
				TextX(posX+2, markY2 + 3, x, Colors.Black);
			}
		}

		private void TextX(double x, double y, decimal modelX, Color color)
		{
			string text = (modelX >= 1000 ? modelX / 1000 : modelX).ToString();
			TextBlock textBlock = new TextBlock();
			
			textBlock.Text = text;
			textBlock.Foreground = new SolidColorBrush(color);

			Size size = MeasureString(textBlock, text);

			Canvas.SetLeft(textBlock, x - size.Width/2 - 2);
			Canvas.SetTop(textBlock, y);

			Canvas.Children.Add(textBlock);

		}
		private Size MeasureString(TextBlock textBlock,string text)
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
