using Rider.Contracts;
using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Rider.Views
{
	public partial class Console : UserControl, IConsole
	{
		public Console()
		{
			InitializeComponent();
		}

		public void WriteLine(string msg)
		{
			WriteLine(msg, Brushes.Black);
		}

		private void WriteLine(string msg, Brush brush)
		{
			Dispatcher.BeginInvoke((Action)(() =>
			{
				TextRange tr = new TextRange(Output.Document.ContentEnd, Output.Document.ContentEnd)
				{
					Text = msg + "\r"
				};
				tr.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
				Output.ScrollToEnd();
			}));

		}

	}
}
