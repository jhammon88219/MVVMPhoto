using Microsoft.Windows.AppLifecycle;
using MVVMPhoto.Core.Photos;
using Windows.ApplicationModel.Activation;

namespace MVVMPhoto.Services;

/// <summary>Finds the photo the app was started with, if any.</summary>
internal static class StartupPhoto
{
	/// <summary>
	/// Explorer "Open with" / double-click arrives as a FILE activation carrying the file; a normal launch
	/// may carry a path in its arguments (command line). Null when the app was started with no photo.
	/// </summary>
	public static string? Find(string launchArguments)
	{
		var activation = AppInstance.GetCurrent().GetActivatedEventArgs();

		if (activation.Kind == ExtendedActivationKind.File
			&& activation.Data is IFileActivatedEventArgs fileActivation
			&& fileActivation.Files.Count > 0)
		{
			return fileActivation.Files[0].Path;
		}

		return StartupArguments.PhotoPath(launchArguments);
	}
}
