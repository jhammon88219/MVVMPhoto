using Microsoft.Extensions.DependencyInjection;
using MVVMPhoto.ViewModels;

namespace MVVMPhoto.Composition;

/// <summary>
/// Every service, view model and window the app uses is registered here, and only here.
/// Add a service = one line below + a constructor parameter where it's needed.
/// </summary>
internal static class ServiceRegistration
{
	public static IServiceCollection AddMVVMPhoto(this IServiceCollection services)
	{
		// View models
		services.AddSingleton<ShellViewModel>();
		services.AddSingleton<ViewerViewModel>();

		// Windows
		services.AddSingleton<MainWindow>();

		return services;
	}
}
