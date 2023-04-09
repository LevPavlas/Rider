using Rider.Views;
using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Rider.Contracts.Services;
using System.Runtime.InteropServices;

namespace Rider
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : PrismApplication
	{
	
		public static Rules DefaultRules => Rules.Default.WithConcreteTypeDynamicRegistrations(reuse: Reuse.Transient)
												 .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
												 .WithFuncAndLazyWithoutRegistration()
												 .WithTrackingDisposableTransients()
												 //.WithoutFastExpressionCompiler()
												 .WithFactorySelector(Rules.SelectLastRegisteredFactory());
		protected override Rules CreateContainerRules()
		{
			return DefaultRules;
		}
		protected override Window CreateShell()
		{
			return Container.Resolve<Shell>();
		}
		protected override IModuleCatalog CreateModuleCatalog()
		{
			return base.CreateModuleCatalog();
		}
		protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
		{
			base.ConfigureModuleCatalog(moduleCatalog);

			Type main = typeof(RiderModule);
			moduleCatalog.AddModule(new ModuleInfo()
			{
				ModuleName = main.Name,
				ModuleType = main.AssemblyQualifiedName,
			});
			
			Type map = typeof(Map.MapModule);
			moduleCatalog.AddModule(new ModuleInfo()
			{
				ModuleName = map.Name,
				ModuleType = map.AssemblyQualifiedName,
			});

			Type route = typeof(Route.RouteModule);
			moduleCatalog.AddModule(new ModuleInfo()
			{
				ModuleName = route.Name,
				ModuleType = route.AssemblyQualifiedName,
			});	

		}
		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterManySingleton(typeof(Services.Time), typeof(Contracts.Services.ITime));
			containerRegistry.RegisterManySingleton(typeof(Services.FileSystem), typeof(Contracts.Services.IFileSystem));
			containerRegistry.RegisterManySingleton(typeof(Services.Configuration), typeof(Contracts.Services.IConfiguration));
			containerRegistry.RegisterManySingleton(typeof(Services.UsbMonitor), typeof(Contracts.Services.IUsbMonitor));
			containerRegistry.RegisterManySingleton(typeof(Views.Console), typeof(Views.Console), typeof(IConsole));
		}
	}
}
