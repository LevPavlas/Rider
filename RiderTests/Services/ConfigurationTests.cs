using DryIoc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rider.Contracts;
using Rider.Contracts.Services;
using Rider.Services;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Rider.Services.Tests
{
	[TestClass()]
	public class ConfigurationTests
	{
		[TestMethod()]
		public void ConfigurationTest()
		{
			Mock<IFileSystem> fileSystem = new Mock<IFileSystem>();
			Configuration target = new Configuration(fileSystem.Object);
		}

		[TestMethod()]
		public void LoadFileNotExisgTest()
		{
			const string AppDir = "Dir";
			
			Mock<IFileSystem> fileSystem = new Mock<IFileSystem>();
			
			fileSystem.Setup(f => f.GetApplicationDirectory()).Returns(AppDir);
			fileSystem.Setup(f => f.FileExist(It.IsAny<string>())).Returns(false);
		
			Configuration target = new Configuration(fileSystem.Object);
			target.Load();
			Assert.AreEqual(3, target.Maps.Count());
			Assert.AreEqual("https://brouter.de/brouter-web", target.SelectedMap);
			Assert.AreEqual("Dir\\Data\\BrowserCache", target.BrowserCacheDataFolder);
			Assert.AreEqual("Dir\\Data\\Gpx", target.GpxDirectory);

			fileSystem.Verify(f => f.GetApplicationDirectory());
			fileSystem.Verify(f => f.CreateDirectory($"{AppDir}\\Data"));
			fileSystem.Verify(f => f.CreateDirectory($"{AppDir}\\Data\\Gpx"));
			fileSystem.Verify(f => f.FileExist($"{AppDir}\\Data\\Configuration.json"));
			fileSystem.Verify(f => f.SaveData($"{AppDir}\\Data\\Configuration.json", It.IsAny<It.IsAnyType>()));
			fileSystem.VerifyNoOtherCalls();
		}

		[TestMethod()]
		public void LoadFileExistTest()
		{
			const string AppDir = "Dir";

			object? data = CreateConfigurationData();
			
			Mock<IFileSystem> fileSystem = new Mock<IFileSystem>();
			
			
			fileSystem.Setup(f => f.GetApplicationDirectory()).Returns(AppDir);
			fileSystem.Setup(f => f.FileExist(It.IsAny<string>())).Returns(true);
			fileSystem.Setup(f => f.LoadData<It.IsAnyType>($"{AppDir}\\Data\\Configuration.json")).Returns(()=>data);
	
			Configuration target = new Configuration(fileSystem.Object);
			target.Load();			
			fileSystem.Verify(f => f.GetApplicationDirectory());
			fileSystem.Verify(f => f.CreateDirectory($"{AppDir}\\Data"));
			fileSystem.Verify(f => f.FileExist($"{AppDir}\\Data\\Configuration.json"));
			fileSystem.Verify(f => f.LoadData<It.IsAnyType>($"{AppDir}\\Data\\Configuration.json"));
		}

		object? CreateConfigurationData()
		{
			Assembly assembly = Assembly.LoadFrom("Rider.dll");
			Type? configType = assembly.GetType("Rider.Services.Configuration");
			Type? dataType = configType?.GetNestedType("ConfigurationData", BindingFlags.NonPublic);

			object[] paramValues = new object[] { };
			Type[] paramTypes = new Type[] { };
			ConstructorInfo? constr = dataType?.GetConstructor(Type.EmptyTypes);
			return constr?.Invoke(null);
		}

	}
}