using DryIoc;
using MapControl;
using MapControl.Caching;
using Rider.Route.Data;
using Rider.Route.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Rider.Route.Views
{
	/// <summary>
	/// Interaction logic for Route.xaml
	/// </summary>
	public partial class Route : UserControl
	{
	
		private RouteViewModel? Model { get; set; }

		public Route()
		{

			DataContextChanged += OnDataContextChanged;
			Loaded += OnLoaded;

			InitializeComponent();						
		}

		private bool FirstLoad { get; set; } = true;
		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			if(FirstLoad)
			{
				FirstLoad = false;
			}
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Model = e.NewValue as RouteViewModel;
			if(Model != null)
			{
			}
		}

	}
}
