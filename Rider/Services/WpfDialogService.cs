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
		public WpfDialogService(IConfiguration configuration) 
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public string? OpenGpxFile()
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "GPX Files|*.gpx";
			dlg.InitialDirectory= Configuration.LastGpxDirectory;

			if (dlg.ShowDialog() == true)
			{
				Configuration.LastGpxFullPath= dlg.FileName;
				return dlg.FileName;
			}

			return null;
		}
		public string? SaveGpxFile(string? suggestedFileName)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "GPX Files|*.gpx";
			dlg.DefaultExt = "gpx";
			dlg.InitialDirectory = Configuration.LastGpxDirectory;
			dlg.FileName = suggestedFileName ?? string.Empty;

			if (dlg.ShowDialog() == true)
			{
				Configuration.LastGpxFullPath = dlg.FileName;
				return dlg.FileName;
			}

			return null;
		}
		public string? SaveTrackFile()
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "Track Files|*.track";
			dlg.DefaultExt = "track";
			dlg.InitialDirectory = Configuration.LastExportDirectory;
			dlg.FileName = Configuration.LastGpxFilenameWithoutExtension;
			if (dlg.ShowDialog() == true)
			{
				Configuration.LastExportFullPath= dlg.FileName;
				return dlg.FileName;
			}

			return null;
		}

	}
}
