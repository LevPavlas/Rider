using Rider.Contracts.Services;
using Rider.Dialogs;
using Rider.Services;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rider.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class Shell : Window
	{
		private IConsole Console { get; }
		private UsbMonitor UsbMonitor { get; }

		public Shell(IConsole console, IUsbMonitor usbMonitor)
		{
			InitializeComponent();
			Loaded += Window_Loaded;
			Console = console;
			UsbMonitor = (UsbMonitor)usbMonitor;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			EnableBlur();
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			HwndSource? source = PresentationSource.FromVisual(this) as HwndSource;
			source?.AddHook(UsbMonitor.WndProc);
			UsbMonitor.CheckDevices();
		}


		private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void OnExit(object sender, RoutedEventArgs e)
		{
			System.Windows.Application.Current.Shutdown();
		}
		private void OnMaximize(object sender, RoutedEventArgs e)
		{
			if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
			{
				Application.Current.MainWindow.WindowState = WindowState.Normal;
			}
			else
			{
				Application.Current.MainWindow.WindowState = WindowState.Maximized;
			}
		}
		private void OnHelp(object sender, RoutedEventArgs e)
		{
			Help help = new Help();
			help.Owner = this;
			help.ShowDialog();
		}
		private void OnMinimize(object sender, RoutedEventArgs e)
		{
			Application.Current.MainWindow.WindowState = WindowState.Minimized;
		}

		private void Window_StateChanged(object sender, EventArgs e)
		{
			if(WindowState == WindowState.Normal)
			{
				btnMaximize.Content = "\uD83D\uDDD6";
			}
			else if(WindowState == WindowState.Maximized)
			{
				btnMaximize.Content = "\uD83D\uDDD7";
			}
		}
    }
}
