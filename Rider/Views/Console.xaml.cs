using Rider.Contracts.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Rider.Views
{
    public partial class Console : UserControl, IConsole
	{
		ConsoleTextWriter SystemConsoleOutput { get; }
		public Console()
		{
			SystemConsoleOutput = new ConsoleTextWriter(this);
			InitializeComponent();
			System.Console.SetOut(SystemConsoleOutput);
		}

		public void WriteLine(string msg)
		{
			WriteLine(msg, Brushes.Black);
		}
		public void WriteError(string msg)
		{
			WriteLine(msg, Brushes.Red);
		}
		public void WriteWarning(string msg)
		{
			WriteLine(msg, Brushes.Blue);
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

		private class ConsoleTextWriter : TextWriter
		{
			const string ErrorTag = "{Error}";

			public override Encoding Encoding => Encoding.Default;

			StringBuilder Line { get; } = new StringBuilder();
			public Console Console { get; }

			object _lock = new object();

			public ConsoleTextWriter(Console console)
			{
				Console = console;
			}
			public override void Write(char value)
			{
				lock(_lock)
				{
					if(value == '\n')
					{
						Line.Clear();
						return;
					}
					if (value == '\r') return;
					Line.Append(value);
				}
			}
			public override void WriteLine(string? value)
			{
				if(value!= null)
				{
					if(value.Contains(ErrorTag))
					{
						Console.WriteError(value.Replace(ErrorTag,""));
					}
					else
					{
						Console.WriteLine(value);
					}

				}
			}
		}


	}
}
