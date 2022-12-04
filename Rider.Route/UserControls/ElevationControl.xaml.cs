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
		public ElevationControl()
		{
			InitializeComponent();
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			Test();
		}

		void Test()
		{
			double widht = canvas.Width;
			double height = canvas.Height;

			Rectangle rect = new Rectangle
			{

				Width= widht,
				Height= height,
				Fill = Brushes.Turquoise,
			};
			canvas.Children.Add(rect);
			
			//Canvas.SetLeft(rect, 100);
			//Canvas.SetTop(rect, 0);
		}
		void LenhgtScale(decimal lenght)
		{
			//canvas.
		}
	}
}
