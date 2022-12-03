using Rider.Map.ViewModels;
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
using System.Windows.Shell;
using System.Xml.Linq;

namespace Rider.Map.Views
{
	/// <summary>
	/// Interaction logic for ToolBar.xaml
	/// </summary>
	public partial class MapToolBar : UserControl
	{
		private MapToolBarViewModel? Model => DataContext as MapToolBarViewModel;

		public MapToolBar()
		{
			InitializeComponent();
		}

		private void OnMapChanged(object sender, SelectionChangedEventArgs e)
		{
			if(e?.AddedItems?.Count == 1)
			{
				string map = e.AddedItems[0]?.ToString() ?? string.Empty;
				Model?.OnMapChanged(map);
			}
		}
	}
}
