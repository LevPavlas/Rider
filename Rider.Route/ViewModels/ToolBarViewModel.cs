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
using Rider.Contracts.Services;
using Prism.Events;
using Rider.Contracts.Events;

namespace Rider.Route.ViewModels
{
    internal class ToolBarViewModel : BindableBase
	{
		private IConsole Console{get;}
		private IWpfDialogService Dialogs { get; }
		private IRouteCalculator Calculator { get; }
		private IEventAggregator EventAggregator { get; }
		public DelegateCommand OpenCommand { get; private set; }
		public ToolBarViewModel(
			IConsole console, 
			IWpfDialogService dialogs,
			IRouteCalculator calculator,
			IEventAggregator eventAggregator) 
		{
			Console = console;
			Dialogs = dialogs;
			Calculator = calculator;
			EventAggregator = eventAggregator;
			EventAggregator.GetEvent<RouteDownloadedEvent>().Subscribe(OnRouteDownloaded, ThreadOption.BackgroundThread);
			OpenCommand = new DelegateCommand(Open,CanOpenExecute);
		}


		void Open()
		{
	//		OpenInProgress = true;
	//		OpenCommand.RaiseCanExecuteChanged();
			
			string? file = Dialogs.OpenFile("GPX Files|*.gpx");
			if(file!= null) Calculator.StartProcessing(file);

	//		OpenInProgress = false;
	//		OpenCommand.RaiseCanExecuteChanged();	
		}
		void OnRouteDownloaded(string path)
		{
			Calculator.StartProcessing(path);
		}
		bool OpenInProgress { get; set; }
		bool CanOpenExecute()
		{
			return !OpenInProgress;
		}

	}
}
