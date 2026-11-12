using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVVMPhoto.Core.Services;
using MVVMPhoto.Services;
using Serilog;

namespace MVVMPhoto.Composition;

/// <summary>
/// The composition root: builds the .NET Generic Host that owns the DI container and logging.
/// </summary>
internal static class AppHost
{
	public static IHost Build()
	{
		// Paths and the logger come first, by hand, so that a failure while building the container
		// can still be written to the log. Both are then handed to the container as ready-made instances.
		IAppPaths paths = new PackagedAppPaths();
		var logger = LoggingSetup.CreateLogger(paths);

		try
		{
			// The EMPTY builder: no appsettings.json, environment variables or console logging.
			// Nothing here is configured from outside the code yet; add those sources only when needed.
			var builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings());

			// Check every registration can be built at startup, so a missing service fails on launch,
			// not halfway through a session.
			builder.ConfigureContainer(new DefaultServiceProviderFactory(new ServiceProviderOptions
			{
				ValidateOnBuild = true,
				ValidateScopes = true,
			}));

			builder.Services.AddSerilog(logger, dispose: true);
			builder.Services.AddSingleton(paths);
			builder.Services.AddMVVMPhoto();

			return builder.Build();
		}
		catch (Exception ex)
		{
			logger.Fatal(ex, "Could not build the app host");
			logger.Dispose();
			throw;
		}
	}
}
