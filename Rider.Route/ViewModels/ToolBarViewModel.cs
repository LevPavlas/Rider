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
using AsyncAwaitBestPractices.MVVM;
using System.Windows.Media.Animation;
using System.ComponentModel;
using Rider.Route.Data;
using Rider.Route.Services;

namespace Rider.Route.ViewModels
{
	internal class ToolBarViewModel : BindableBase
	{
		private IConsole Console{get;}
		private IWpfDialogService Dialogs { get; }
		private IGpxReader Reader { get; }
		public AsyncCommand OpenCommand { get; private set; }
		public ToolBarViewModel(IConsole console, IWpfDialogService dialogs, IGpxReader reader) 
		{
			Console = console;
			Dialogs = dialogs;
			Reader = reader;
			OpenCommand = new AsyncCommand(Open,CanOpenExecute,OnOpenCommandError, true);
		}


		async Task Open()
		{
			OpenInProgress = true;
			OpenCommand.RaiseCanExecuteChanged();
			string? file = Dialogs.OpenFile("GPX Files|*.gpx");
			if(!string.IsNullOrEmpty(file))
			{
				Console.WriteLine($"Processing file:{file}");
				RiderData data = await Reader.Read(file);
				Console.WriteLine($"Track point count: {data.Track.Points.Count}");
				Console.WriteLine($"Track distance: {data.Track.Distance/1000} [km]");
			}

			OpenInProgress = false;
			OpenCommand.RaiseCanExecuteChanged();	
		}

		bool OpenInProgress { get; set; }
		bool CanOpenExecute(object? arg)
		{
			return !OpenInProgress;
		}
		private void OnOpenCommandError(Exception e)
		{
			Console.WriteLine($"Open error: {e.Message} Thread: {Thread.CurrentThread.ManagedThreadId}");
		}

	}
}
