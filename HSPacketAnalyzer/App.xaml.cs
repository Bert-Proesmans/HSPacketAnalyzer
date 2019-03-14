using HSPacketAnalyzer.Views;
using Serilog;
using Serilog.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace HSPacketAnalyzer
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class HSPacketAnalyzerApp : Application
	{
		private readonly Logger _logger;

		public HSPacketAnalyzerApp()
		{
			_logger = Helpers.Logging.SetDefault();
		}

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

			// Open MainWindow
			ShutdownMode = ShutdownMode.OnMainWindowClose; // DBG; Remove this later
			MainWindow = new PacketInspectorWindow(packetsFilePath);
			MainWindow.Show();
		}

		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			Debugger.Break();
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			_logger?.Dispose();
		}

		#endregion
	}
}
