﻿using HSPacketAnalyzer.Controllers;
using HSPacketAnalyzer.Services;
using HSPacketAnalyzer.Views;
using MyToolkit.Composition;
using Serilog;
using Serilog.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Windows;

namespace HSPacketAnalyzer
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class HSPacketAnalyzerApp : Application
	{
		#region Private Fields

		private readonly Logger _logger;

		#endregion

		#region Public Constructors

		public HSPacketAnalyzerApp()
		{
			AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			_logger = Helpers.Logging.SetDefault();
		}

		#endregion

		#region Private Methods

		#region EVENT_HANDLERS
		private void Application_Startup(object sender, StartupEventArgs e)
		{
#if !DEBUG
            if (HSPacketAnalyzer.Properties.Toggles.Default.ShowConsole)
            {
                // WARN; The console window will NEVER have any data written to it
                // WHEN the VS debugger is attached (Console output is written to 
                // the Debug Output instead)!
                // WARN; Closing the console window will shut down the entire application!
                Helpers.ConsoleManager.Show();
            }
#endif

			Log.Information("Program startup at {time}", DateTime.Now);

			// Handle arguments
			string[] _ = e.Args;
			string packetsFilePath = null;

			// Initialize dependencies
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			Log.Verbose("Switched CWD to {path}", Directory.GetCurrentDirectory());

			var contextFactory = new AppContextRepository(this);
			ServiceLocator.Default.RegisterSingleton<IContextRepository, AppContextRepository>(contextFactory);

			// Open MainWindow
			ShutdownMode = ShutdownMode.OnMainWindowClose; // DBG; Remove this later
			MainWindow = new PacketInspectorWindow(packetsFilePath);
			MainWindow.Show();
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			_logger?.Dispose();
		}

		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			Debugger.Break();
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Debugger.Break();
		}

		private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
		{
			// Debugger.Break();
		}

		#endregion

		#endregion
	}
}
