using Microsoft.Win32;
using Rider.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Services
{
    internal class WpfDialogService : IWpfDialogService
	{
		public string? OpenFile(string filter)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "GPX Files|*.gpx";
			if (dlg.ShowDialog() == true)
			{
				return dlg.FileName;
			}

			return null;
		}
	}
}
