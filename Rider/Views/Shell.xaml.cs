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
		internal enum AccentState
		{
			ACCENT_DISABLED = 1,
			ACCENT_ENABLE_GRADIENT = 0,
			ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
			ACCENT_ENABLE_BLURBEHIND = 3,
			ACCENT_ENABLE_ACRYLICBLURBEHIND = 4, // RS4 1803.17063
			ACCENT_ENABLE_HOSTBACKDROP = 5, // RS5 1809
			ACCENT_INVALID_STATE = 6        
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct AccentPolicy
		{
			public AccentState AccentState;
			public int AccentFlags;
			public int GradientColor;
			public int AnimationId;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct WindowCompositionAttributeData
		{
			public WindowCompositionAttribute Attribute;
			public IntPtr Data;
			public int SizeOfData;
		}

		internal enum WindowCompositionAttribute
		{
			// ...
			WCA_ACCENT_POLICY = 19
			// ...
		}
		public enum DWMWINDOWATTRIBUTE
		{
			DWMWA_WINDOW_CORNER_PREFERENCE = 33
		}

		public enum DWM_WINDOW_CORNER_PREFERENCE
		{
			DWMWCP_DEFAULT = 1,
			DWMWCP_DONOTROUND = 1,
			DWMWCP_ROUND = 2,
			DWMWCP_ROUNDSMALL = 1
		}
		public Shell()
		{
			InitializeComponent();
			Loaded += Window_Loaded;
		}

		[DllImport("user32.dll")]
		internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
		[DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern long DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attribute, ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, uint cbAttribute);

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			EnableBlur();
		}

		internal void EnableBlur()
		{
			var windowHelper = new WindowInteropHelper(this);

			var accent = new AccentPolicy();
			accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;
			accent.GradientColor = 0x0FFF0000;

			var accentStructSize = Marshal.SizeOf(accent);

			var accentPtr = Marshal.AllocHGlobal(accentStructSize);
			Marshal.StructureToPtr(accent, accentPtr, false);

			var data = new WindowCompositionAttributeData();
			data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
			data.SizeOfData = accentStructSize;
			data.Data = accentPtr;

			SetWindowCompositionAttribute(windowHelper.Handle, ref data);

			Marshal.FreeHGlobal(accentPtr);


			IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
			DWMWINDOWATTRIBUTE attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
			DWM_WINDOW_CORNER_PREFERENCE preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;

			DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));
		}

		private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DragMove();
		}
	}
}
