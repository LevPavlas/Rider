using Rider.Contracts;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Win32;

namespace Rider.Route.ViewModels
{
	internal class ToolBarViewModel : BindableBase
	{
		private IConsole Console{get;}
		private IWpfDialogService Dialogs { get; }
		public DelegateCommand OpenCommand { get; private set; }

		public ToolBarViewModel(IConsole console, IWpfDialogService dialogs) 
		{
			Console = console;
			Dialogs = dialogs;
			Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}");
			OpenCommand = new DelegateCommand(async () => await Open(), CanOpenExecute);
		}


		async Task Open()
		{
			OpenInProgress= true;
			OpenCommand.RaiseCanExecuteChanged();

			string? file = Dialogs.OpenFile("GPX Files|*.gpx");
			
			Console.WriteLine($"Open File:{file}");
			Console.WriteLine($"Open enter: {DateTime.Now}Thread: {Thread.CurrentThread.ManagedThreadId}");
			await Task.Run(() =>
			{
				Console.WriteLine($"Start {DateTime.Now}Thread: {Thread.CurrentThread.ManagedThreadId}");
				Thread.Sleep(5000);
				Console.WriteLine($"Stop {DateTime.Now}Thread: {Thread.CurrentThread.ManagedThreadId}");
			});

			OpenInProgress = false;
			OpenCommand.RaiseCanExecuteChanged();

			Console.WriteLine($"Open exit: {DateTime.Now}Thread: {Thread.CurrentThread.ManagedThreadId}");

		}
		bool OpenInProgress { get; set; }
		bool CanOpenExecute()
		{
			return !OpenInProgress;
		}

	}
}
