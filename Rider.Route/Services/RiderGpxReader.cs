﻿using GpxTools.Gpx;
using GpxTools;
using Rider.Contracts;
using Rider.Route.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rider.Contracts.Services;

namespace Rider.Route.Services
{
	internal interface IGpxReader
	{
		Task<Data.Route> Read(string path);
	}

	internal class RiderGpxReader : IGpxReader
	{
		private IFileSystem FileSystem { get; }
		public RiderGpxReader(IFileSystem fileSystem)
		{
			FileSystem = fileSystem;	
		}

		public async Task<Data.Route> Read(string path)
		{
			return await Task<RiderData>.Run(() =>
			{
				using(Stream stream = FileSystem.OpenFile(path))
				using (GpxReader reader = new GpxReader(stream))
				{
					GpxAnalyser analyser = new GpxAnalyser(reader);
					analyser.Analyse();

					return new Data.Route(analyser);
				}
			});
		}
	}
}