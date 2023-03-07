using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Win32;
using System.Windows.Media.Animation;
using System.ComponentModel;
using Rider.Route.Data;
using Rider.Route.Services;
using Rider.Contracts.Services;
using Prism.Events;
using Rider.Contracts.Events;
using System.IO;
using System.Windows.Shapes;
using System.ComponentModel.DataAnnotations;

namespace Rider.Route.ViewModels
{
    internal class RouteToolBarViewModel : BindableBase
	{
		public IConfiguration Configuration { get; }
		private IConsole Console{get;}
		private IWpfDialogService Dialogs { get; }
		private IRiderCalculator Calculator { get; }
		private IEventAggregator EventAggregator { get; }
		public IFileSystem FileSystem { get; }
		public IRiderWriter Writer { get; }
		public DelegateCommand OpenCommand { get; private set; }
		public DelegateCommand ExportCommand { get; private set; }

		RiderData? _RiderData;
		public RiderData? RiderData
		{
			get => _RiderData;
			set => SetProperty(ref _RiderData, value);
		}

		public RouteToolBarViewModel(
			IConfiguration configuration,
			IConsole console, 
			IWpfDialogService dialogs,
			IRiderCalculator calculator,
			IEventAggregator eventAggregator,
			IFileSystem fileSystem,
			IRiderWriter writer) 
		{
			Configuration = configuration;
			Console = console;
			Dialogs = dialogs;
			Calculator = calculator;

			EventAggregator = eventAggregator;
			FileSystem = fileSystem;
			Writer = writer;
			EventAggregator.GetEvent<RouteDownloadedEvent>().Subscribe(OnRouteDownloaded, ThreadOption.BackgroundThread);
			EventAggregator.GetEvent<RiderDataCalculatedEvent>().Subscribe(OnDatatCalculated, ThreadOption.PublisherThread);
			OpenCommand = new DelegateCommand(Open,CanOpenExecute);
			ExportCommand = new DelegateCommand(Export, CanExportExecute);
		}


		void Open()
		{
			//		OpenInProgress = true;
			//		OpenCommand.RaiseCanExecuteChanged();
			string? file = Dialogs.OpenGpxFile();
			if(file!= null && FileSystem.FileExist(file))
			{
					Calculator.StartProcessing(file);
			}

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

		void Export()
		{
			try
			{
				string? file = Dialogs.SaveTrackFile();
				if (file != null && RiderData != null)
				{
					Writer.Export(RiderData, file);
				}
			}
			catch (Exception ex)
			{
				Console.WriteError(ex.ToString());
			}
		}
		bool CanExportExecute()
		{
			return RiderData?.Route?.Points?.Count > 2;
		}
		private void OnDatatCalculated(RiderData data)
		{
			RiderData = data;
			ExportCommand.RaiseCanExecuteChanged();
		}


	}
}
