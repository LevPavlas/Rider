using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Win32;
using Prism;
using Prism.Events;
using Rider.Map.Events;
using Rider.Contracts.Services;

namespace Rider.Map.ViewModels
{
    internal class ToolBarViewModel : BindableBase
	{
		public IConfiguration Configuration { get; }
		private IEventAggregator EventAggregator { get; }

		public ToolBarViewModel(IConfiguration configuration, IEventAggregator eventAggregator)
		{
			Configuration = configuration;
			EventAggregator = eventAggregator;
		}

		public void OnMapChanged(string map)
		{
			EventAggregator.GetEvent<MapChangedEvent>().Publish(map);
		}

	}
}
