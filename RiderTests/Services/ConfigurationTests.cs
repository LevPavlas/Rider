using DryIoc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rider.Contracts;
using Rider.Contracts.Services;
using Rider.Services;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
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
			Assert.AreEqual(4, target.Maps.Count());
			Assert.AreEqual("https://brouter.de/brouter-web", target.SelectedMap);
			Assert.AreEqual("Dir\\Data\\BrowserCache", target.BrowserCacheDataFolder);

			fileSystem.Verify(f => f.GetApplicationDirectory());
			fileSystem.Verify(f => f.CreateDirectory($"{AppDir}\\Data"));
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

			Assert.AreEqual("LastGpxFullPathValue", target.LastGpxFullPath);
			Assert.AreEqual("LastExportFullPathValue", target.LastExportFullPath);
			Assert.AreEqual("SelectedMapValue", target.SelectedMap);

			fileSystem.Verify(f => f.GetApplicationDirectory());
			fileSystem.Verify(f => f.CreateDirectory($"{AppDir}\\Data"));
			fileSystem.Verify(f => f.FileExist($"{AppDir}\\Data\\Configuration.json"));
			fileSystem.Verify(f => f.LoadData<It.IsAnyType>($"{AppDir}\\Data\\Configuration.json"));
		}

		[TestMethod()]
		public void SetGpxPathTest()
		{
			const string DirectoryName = "DirectoryName";
			const string GpxPath = "GpxPath";
			string changedPropertyName = string.Empty;

			Mock<IFileSystem> fileSystem = new Mock<IFileSystem>();
			fileSystem.Setup(f => f.GetDirectoryName(GpxPath)).Returns(DirectoryName);

			Configuration target = new Configuration(fileSystem.Object);
			target.PropertyChanged += (s, e) => { changedPropertyName = e.PropertyName ?? string.Empty; };

			target.LastGpxFullPath= GpxPath;

			Assert.AreEqual(nameof(Configuration.LastGpxFullPath),changedPropertyName);
			Assert.AreEqual(DirectoryName, target.LastGpxDirectory);

			fileSystem.Verify(f => f.SaveData(string.Empty, It.IsAny<It.IsAnyType>()));
			fileSystem.Verify(f => f.GetDirectoryName(GpxPath));
			fileSystem.VerifyNoOtherCalls();


		}
		[TestMethod()]
		public void SetExportPathTest()
		{
			const string DirectoryName = "DirectoryName";
			const string ExportPath = "ExportPath";
			string changedPropertyName = string.Empty;

			Mock<IFileSystem> fileSystem = new Mock<IFileSystem>();
			fileSystem.Setup(f => f.GetDirectoryName(ExportPath)).Returns(DirectoryName);
			fileSystem.Setup(f=>f.DirectoryExists(DirectoryName)).Returns(true);


			Configuration target = new Configuration(fileSystem.Object);
			target.PropertyChanged += (s, e) => { changedPropertyName = e.PropertyName ?? string.Empty; };

			target.LastExportFullPath = ExportPath;

			Assert.AreEqual(nameof(Configuration.LastExportFullPath), changedPropertyName);
			Assert.AreEqual(DirectoryName, target.LastExportDirectory);
			fileSystem.Verify(f => f.SaveData(string.Empty, It.IsAny<It.IsAnyType>()));
			fileSystem.Verify(f => f.GetDirectoryName(ExportPath));
			fileSystem.Verify(f => f.DirectoryExists(DirectoryName));
			fileSystem.VerifyNoOtherCalls();


		}

		object? CreateConfigurationData()
		{
			Assembly assembly = Assembly.LoadFrom("Rider.dll");
			Type? configType = assembly.GetType("Rider.Services.Configuration");
			Type? dataType = configType?.GetNestedType("ConfigurationData", BindingFlags.NonPublic);

			object[] paramValues = new object[] { };
			Type[] paramTypes = new Type[] { };
			ConstructorInfo? constr = dataType?.GetConstructor(Type.EmptyTypes);
			object? instance = constr?.Invoke(null);
			SetProperty(dataType, instance, "LastGpxFullPath", "LastGpxFullPathValue");
			SetProperty(dataType, instance, "LastExportFullPath", "LastExportFullPathValue");
			SetProperty(dataType, instance, "SelectedMap", "SelectedMapValue");
			return instance;
		}
		void SetProperty(Type? dataType, object? instance,string propertyName, object value)
		{
			if (dataType== null ||instance == null) return;
			PropertyInfo[] pp = dataType.GetProperties();
			PropertyInfo? propertyInfo = dataType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
			if(propertyInfo == null) return;
			propertyInfo.SetValue(instance, value);
		}
	}
}