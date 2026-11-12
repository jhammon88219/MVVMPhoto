using System.Globalization;
using MVVMPhoto.Core.Services;
using Serilog;
using Serilog.Events;

namespace MVVMPhoto.Composition;

/// <summary>
/// Builds the one Serilog logger the whole app writes through (via <c>ILogger&lt;T&gt;</c>).
/// Logs are written in the invariant culture so they read the same on every machine.
/// </summary>
internal static class LoggingSetup
{
	private const string OutputTemplate =
		"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";

	public static Serilog.Core.Logger CreateLogger(IAppPaths paths)
	{
		var configuration = new LoggerConfiguration()
#if DEBUG
			.MinimumLevel.Debug()
#else
			.MinimumLevel.Information()
#endif
			// Framework chatter only when something is wrong.
			.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
			.MinimumLevel.Override("System", LogEventLevel.Warning)
			.Enrich.FromLogContext()
			// One file per day, a new one past 10 MB, two weeks kept. Unbuffered, so a crash loses nothing.
			.WriteTo.File(
				Path.Combine(paths.LogsFolder, "log-.txt"),
				rollingInterval: RollingInterval.Day,
				fileSizeLimitBytes: 10 * 1024 * 1024,
				rollOnFileSizeLimit: true,
				retainedFileCountLimit: 14,
				outputTemplate: OutputTemplate,
				formatProvider: CultureInfo.InvariantCulture);

#if DEBUG
		// Visual Studio's Output window while debugging.
		configuration = configuration.WriteTo.Debug(
			outputTemplate: OutputTemplate,
			formatProvider: CultureInfo.InvariantCulture);
#endif

		return configuration.CreateLogger();
	}
}
