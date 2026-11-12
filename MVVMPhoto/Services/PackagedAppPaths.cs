using MVVMPhoto.Core.Services;
using Windows.Storage;

namespace MVVMPhoto.Services;

/// <summary>
/// <see cref="IAppPaths"/> for the packaged (MSIX) app: everything lives under the package's LocalState
/// folder, <c>%LocalAppData%\Packages\&lt;package family name&gt;\LocalState</c>.
/// </summary>
public sealed class PackagedAppPaths : IAppPaths
{
	public PackagedAppPaths()
	{
		LocalDataFolder = ApplicationData.Current.LocalFolder.Path;
		LogsFolder = Path.Combine(LocalDataFolder, "Logs");
	}

	public string LocalDataFolder { get; }

	public string LogsFolder { get; }
}
