using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rider.Contracts.Services;
using Rider.Route.Data;

namespace Rider.Route.Services.Tests
{
    [TestClass()]
	public class ClimbChanllengeCalculatorTests
	{
		Mock<IFileSystem> FileSystem { get; set; } = new Mock<IFileSystem>();
		Mock<IConsole> Console { get; set; } = new Mock<IConsole>();

		[TestInitialize()]
		public void Setup()
		{
			FileSystem = new Mock<IFileSystem>();
			Console = new Mock<IConsole>();
		}
		GpxReader CreateTarget()
		{
			return new GpxReader(FileSystem.Object, Console.Object);
		}
		[TestMethod()]
		public async Task CalculateTest()
		{
			const string FileName = "FileName";
			using (Stream inputStream = GetEmbededResourceStream("Resources.Route01.gpx"))
			{
				FileSystem.Setup(f=>f.OpenRead(FileName)).Returns(inputStream);

				GpxReader reader = new GpxReader(FileSystem.Object, Console.Object); ;
				IRoute? data = await reader.Read(FileName);
				Assert.IsNotNull(data);

				ClimbChallengeCalculator calculator = new ClimbChallengeCalculator();
				IList<ClimbChallenge>  result = calculator.Calculate(data.Points);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual(177, result[0].Start);
				Assert.AreEqual(1169, result[0].End);
				Assert.AreEqual(26597.138542678214, result[0].Size);
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