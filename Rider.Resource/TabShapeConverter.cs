using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rider.Resource
{
	public class TabShapeConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{

			if (values?.Length==2)
			{
				ContentPresenter? content = values[0] as ContentPresenter;
				bool? isSelected= values[1] as bool?;
				if(content!= null && isSelected != null)
				{
					return Convert(content,isSelected.Value,targetType,parameter,culture);
				}
			}
			return new PathGeometry();
		}

		public object Convert(ContentPresenter content, bool isSelected, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{

			PathGeometry geometry = new PathGeometry();

			PathFigure figure = new PathFigure();

			double height = content.ActualHeight;
			double width = content.ActualWidth;
			double x0 = 0.5;
			double y0 = height + (isSelected ? 11 : 9);

			figure.IsClosed = false;
			figure.StartPoint = new Point(x0, y0);
			PathSegmentCollection segments = new PathSegmentCollection();

			segments.Add(new LineSegment(new Point(x0, y0 - 0.7 * height), true));
			segments.Add(new BezierSegment(new Point(x0, y0 - 0.9 * height), new Point(0.1 * height, 0), new Point(0.5 * height, 0), true));
			segments.Add(new LineSegment(new Point(width, 0), true));
			segments.Add(new BezierSegment(new Point(width + 0.8 * height, 0), new Point(width + height, y0), new Point(width + height * 1.3, y0), true));
		
			figure.Segments = segments;
			geometry.Figures.Add(figure);

			return geometry;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}


		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
