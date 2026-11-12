using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using MVVMPhoto.Composition;
using MVVMPhoto.Core.Services;
using Windows.ApplicationModel;

namespace MVVMPhoto;

/// <summary>
/// App entry point: builds the host (DI + logging), hooks the crash handlers and opens the main window.
/// </summary>
public partial class App : Application
{
	private readonly IHost _host;
	private readonly ILogger<App> _logger;

	public App()
	{
		InitializeComponent();

		_host = AppHost.Build();
		_logger = _host.Services.GetRequiredService<ILogger<App>>();

		HookCrashHandlers();
	}

	protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
	{
		LogStartup();

		// Resolved here, on the UI thread: WinUI objects must be created on the thread that shows them.
		var window = _host.Services.GetRequiredService<MainWindow>();
		window.Closed += OnMainWindowClosed;
		window.Activate();
	}

	private void LogStartup()
	{
		// Guarded: the values are built (string formatting, a WinRT call) only when the line will be written.
		if (_logger.IsEnabled(LogLevel.Information))
		{
			var version = Package.Current.Id.Version;
			var versionText = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
			var architecture = RuntimeInformation.ProcessArchitecture.ToString(); // as text: Serilog quotes enums
			var logsFolder = _host.Services.GetRequiredService<IAppPaths>().LogsFolder;

			LogStarting(versionText, RuntimeInformation.OSDescription, architecture, logsFolder);
		}
	}

	private void OnMainWindowClosed(object sender, WindowEventArgs args)
	{
		LogShuttingDown();

		// Disposes every service, including the logger, which flushes and closes the log file.
		_host.Dispose();
	}

	/// <summary>
	/// Last-chance logging. These only RECORD the failure; the app still crashes as it would have,
	/// because carrying on in an unknown state is worse than stopping.
	/// </summary>
	private void HookCrashHandlers()
	{
		// An exception thrown on the UI thread (event handlers, bindings, layout).
		UnhandledException += (_, e) => LogUnhandledUiException(e.Exception);

		// Any other thread.
		AppDomain.CurrentDomain.UnhandledException += (_, e) =>
			LogUnhandledException(e.ExceptionObject as Exception, e.IsTerminating);

		// A faulted Task nobody awaited. Doesn't crash the app, but is always a bug.
		TaskScheduler.UnobservedTaskException += (_, e) => LogUnobservedTaskException(e.Exception);
	}

	// Source-generated log methods (house rule, see CLAUDE.md). They write through the _logger field.

	[LoggerMessage(Level = LogLevel.Information, Message = "MVVMPhoto {Version} starting on {OS} ({Architecture}); logs in {LogsFolder}")]
	private partial void LogStarting(string version, string os, string architecture, string logsFolder);

	[LoggerMessage(Level = LogLevel.Information, Message = "Main window closed; shutting down")]
	private partial void LogShuttingDown();

	[LoggerMessage(Level = LogLevel.Critical, Message = "Unhandled exception on the UI thread")]
	private partial void LogUnhandledUiException(Exception exception);

	[LoggerMessage(Level = LogLevel.Critical, Message = "Unhandled exception (terminating: {IsTerminating})")]
	private partial void LogUnhandledException(Exception? exception, bool isTerminating);

	[LoggerMessage(Level = LogLevel.Error, Message = "Unobserved task exception")]
	private partial void LogUnobservedTaskException(Exception exception);
}
