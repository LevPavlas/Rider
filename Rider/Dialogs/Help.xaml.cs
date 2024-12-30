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
using System.Windows.Shapes;

namespace Rider.Dialogs
{
	/// <summary>
	/// Interaction logic for Help.xaml
	/// </summary>
	public partial class Help : Window
	{
		public Help()
		{
			InitializeComponent();
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			EnableBlur();
		}

		private void OnExit(object sender, RoutedEventArgs e)
		{
			this.Close();
        }

		private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

        private void RichTextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }
    }
}
