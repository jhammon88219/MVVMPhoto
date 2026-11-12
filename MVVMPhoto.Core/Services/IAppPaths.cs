namespace MVVMPhoto.Core.Services;

/// <summary>
/// Where the app keeps its own files on disk. Everything that writes app data asks this service,
/// so the locations are decided in one place.
/// </summary>
public interface IAppPaths
{
	/// <summary>The app's private, per-user data folder (survives updates, removed on uninstall).</summary>
	string LocalDataFolder { get; }

	/// <summary>Rolling log files.</summary>
	string LogsFolder { get; }
}
