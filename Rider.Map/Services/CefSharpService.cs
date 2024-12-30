using CefSharp.Wpf;
using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rider.Contracts.Services;

namespace Rider.Map.Services
{
    internal interface ICefSharpService
	{
		void Initiaize();
	}

	internal class CefSharpService : ICefSharpService,ICookieVisitor
	{
		private IConfiguration Configuration { get; }

		public CefSharpService(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public void Initiaize()
		{
			if (Cef.IsInitialized!= true)
			{

				CefSettings setting = new CefSettings();

				setting.CachePath = Configuration.BrowserCacheDataFolder;
				
				//		setting.LogFile= System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Log\\Log.log");
				//		setting.LogSeverity = LogSeverity.Info;

				//setting.CefCommandLineArgs.Add("enable-media-stream", "1");
				//setting.CefCommandLineArgs.Add("allow-running-insecure-content", "1");
				//setting.CefCommandLineArgs.Add("use-fake-ui-for-media-stream", "1");
				//setting.CefCommandLineArgs.Add("enable-speech-input", "1");
				//setting.CefCommandLineArgs.Add("enable-usermedia-screen-capture", "1");

				if (!Cef.Initialize(setting)) throw new RiderMapException("Browser not initialized");
			//	Cef.GetGlobalCookieManager().VisitAllCookies(this);
			}
		}

		public bool Visit(Cookie cookie, int count, int total, ref bool deleteCookie)
		{
	//		if(cookie.Domain ==".seznam.cz")deleteCookie=true;
			if (cookie.Domain == ".mapy.cz") deleteCookie = true;
			return true;
		}

		public void Dispose()
		{
		}
	}
}
