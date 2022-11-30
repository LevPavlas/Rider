using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using Prism.Regions;
using Rider.Contracts;
using Rider.Route.Data;
using Rider.Route.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rider.Route.Services.Tests
{
	[TestClass()]
	public class RiderGpxReaderTests
	{
		Mock<IFileSystem> FileSystem { get; set; } = new Mock<IFileSystem>();

		[TestInitialize()]
		public void Setup()
		{
			FileSystem = new Mock<IFileSystem>();
		}
		RiderGpxReader CreateTarget()
		{
			return new RiderGpxReader(FileSystem.Object);
		}
		[TestMethod()]
		public async Task ReadTest()
		{
			const string FileName = "FileName";
			using (Stream inputStream = GetEmbededResourceStream("Resources.Route01.gpx"))
			{
				FileSystem.Setup(f=>f.OpenFile(FileName)).Returns(inputStream);

				RiderGpxReader target = CreateTarget();
				RiderData data = await target.Read(FileName);

				Assert.AreEqual(2584, data.Track.Points.Count);
				Assert.AreEqual(28193186, data.Track.LatitudeMax_N);
				Assert.AreEqual(28143041, data.Track.LatitudeMin_S);
				Assert.AreEqual(-16427294, data.Track.LongitudeMax_E);
				Assert.AreEqual(-16799358, data.Track.LongitudeMin_W);
			}

		}
		Stream GetEmbededResourceStream(string resource)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			string path = Regex.Replace(assembly.ManifestModule.Name, @"\.(exe|dll)$", string.Empty, RegexOptions.IgnoreCase);

			Stream? stream = assembly.GetManifestResourceStream($"{path}.{resource}");
			if (stream == null) throw new ArgumentException($"Resource: {resource} not found");
			return stream;
		}

	}
}