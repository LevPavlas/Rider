using Prism;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Map.ViewModels
{
	internal class MapViewModel:BindableBase, IActiveAware
	{
		public string HeaderText { get; } = "Map";
		public event EventHandler? IsActiveChanged;

		private IRegionManager RegionManager { get; }

		public MapViewModel(IRegionManager regionManager)
		{
			RegionManager = regionManager;
		}

		bool isActive = false;
		public bool IsActive
		{
			get
			{
				return isActive;
			}

			set
			{
				if (isActive != value)
				{
					if(value) Activate();
					isActive = value;
					IsActiveChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}
		void Activate()
		{
			RegionManager.RequestNavigate(Constants.Regions.ToolBar, Constants.Views.MapToolBar);
		}

	}
}
