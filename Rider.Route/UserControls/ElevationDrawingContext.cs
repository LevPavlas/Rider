﻿using MapControl;
using Rider.Route.Data;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rider.Route.UserControls
{
	internal class ElevationDrawingContext
	{
		const double Border = 50;
		const double MinCanvasSpacing = 40;
		public RiderData Data { get; }

		public double ModelXmin { get; }
		public double ModelNiceXmin { get; private set; }
		public double ModelXmax { get; }
		public double ModelNiceXmax { get; private set; }
		public double ModelWidth { get; }
		public double ModelNiceWidth { get; private set; }
		public double ModelResizeBorder { get; }

		public double ModelYmin { get; }
		public double ModelNiceYmin { get; private set; }
		public double ModelYmax { get; }
		public double ModelNiceYmax { get; private set; }
		public double ModelHeight { get; }
		public double ModelNiceHeight { get; private set; }

		public double ModelSpacingX { get; private set; }
		public double ModelSpacingY { get; private set; }


		public Canvas Canvas { get; }
		public double CanvasXmin { get; }
		public double CanvasXmax { get; }
		public double CanvasWidth { get; }
		public double CanvasYmin { get; }
		public double CanvasYmax { get; }
		public double CanvasHeight { get; }
		public int MaxTickX { get; }
		public int MaxTickY { get; }

		public double RatioX { get; }
		public double RatioY { get; }

		public ElevationDrawingContext(Canvas canvas, RiderData data)
		{
			IRoute route= data.Route;
			CanvasXmin = Border;
			CanvasXmax = canvas.ActualWidth - Border;
			CanvasWidth = CanvasXmax - CanvasXmin;
			CanvasYmin = Border;
			CanvasYmax= canvas.ActualHeight - Border;
			CanvasHeight = CanvasYmax - CanvasYmin;
			MaxTickX = (int)(CanvasWidth / MinCanvasSpacing);
			MaxTickY = (int)(1.5*CanvasHeight/ MinCanvasSpacing);

			ModelXmin = 0;
			ModelXmax = route.Distance;
			ModelYmin = route.ElevationMin;
			ModelYmax = route.ElevationMax;
			ModelWidth = ModelXmax- ModelXmin;
			ModelHeight= ModelYmax- ModelYmin;
			ModelResizeBorder = ModelWidth / 200;
			CalculateSpacing();
			
			RatioX = CanvasWidth / ModelNiceWidth;
			RatioY = CanvasHeight / ModelNiceHeight;
			Canvas = canvas;
			Data = data;
		}

		public Point ToCanvasPoint(double distance, double elevation)
		{
			double x = CanvasXmin + RatioX * (distance- ModelNiceXmin);
			double y = CanvasYmax - RatioY * (elevation - ModelNiceYmin);
			return new Point(x, y);
		}
		public double ToModelDistance(double x)
		{
			double distance = (x-CanvasXmin)/ RatioX;
			if(distance >= ModelXmin && distance<=ModelXmax ) 
			{
				return distance;
			}
			return -1;
		}
		private void CalculateSpacing()
		{
			double rangeX = NiceNum(ModelWidth, false);
			ModelSpacingX = NiceNum(rangeX / (MaxTickX - 1), true);
			ModelNiceXmin = Math.Floor(ModelXmin / ModelSpacingX) * ModelSpacingX;
			ModelNiceXmax = Math.Ceiling(ModelXmax / ModelSpacingX) * ModelSpacingX;
			ModelNiceWidth = ModelNiceXmax- ModelNiceXmin;

			double rangeY = NiceNum(ModelHeight, false);
			ModelSpacingY = NiceNum(rangeY / (MaxTickY - 1), true);
			ModelNiceYmin = Math.Floor(ModelYmin / ModelSpacingY) * ModelSpacingY;
			ModelNiceYmax = Math.Ceiling(ModelYmax / ModelSpacingY) * ModelSpacingY;
			ModelNiceHeight = ModelNiceYmax- ModelNiceYmin;
		}

		private  double NiceNum(double range, bool round)
		{
			double pow = Math.Pow(10, Math.Floor(Math.Log10(range)));
			double fraction = range / pow;

			double niceFraction;
			if (round)
			{
				if (fraction < 1.5)
				{
					niceFraction = 1;
				}
				else if (fraction < 3)
				{
					niceFraction = 2;
				}
				else if (fraction < 7)
				{
					niceFraction = 5;
				}
				else
				{
					niceFraction = 10;
				}
			}
			else
			{
				if (fraction <= 1)
				{
					niceFraction = 1;
				}
				else if (fraction <= 2)
				{
					niceFraction = 2;
				}
				else if (fraction <= 5)
				{
					niceFraction = 5;
				}
				else
				{
					niceFraction = 10;
				}
			}

			return niceFraction * pow;
		}
		public  Size MeasureString(TextBlock textBlock, string text)
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
