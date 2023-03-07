using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using Prism.Regions;
using Rider.Contracts;
using Rider.Contracts.Services;
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
		Mock<IConsole> Console { get; set; } = new Mock<IConsole>();

		[TestInitialize()]
		public void Setup()
		{
			FileSystem = new Mock<IFileSystem>();
			Console = new Mock<IConsole>();
		}
		RiderGpxReader CreateTarget()
		{
			return new RiderGpxReader(FileSystem.Object, Console.Object);
		}
		[TestMethod()]
		public async Task ReadTest()
		{
			const string FileName = "FileName";
			using (Stream inputStream = GetEmbededResourceStream("Resources.Route01.gpx"))
			{
				FileSystem.Setup(f=>f.OpenRead(FileName)).Returns(inputStream);

				RiderGpxReader target = CreateTarget();
				IRoute? data = await target.Read(FileName);
				Assert.IsNotNull(data);
				Assert.AreEqual(2582, data.Points.Count);
				Assert.AreEqual(28.193186, data.LatitudeMaxNorth);
				Assert.AreEqual(28.143041, data.LatitudeMinSouth);
				Assert.AreEqual(-16.427294, data.LongitudeMaxEast);
				Assert.AreEqual(-16.799358, data.LongitudeMinWest);
				Assert.AreEqual(66228.27542327595, data.Distance);
				Assert.AreEqual(2919, data.ElevationGain);
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