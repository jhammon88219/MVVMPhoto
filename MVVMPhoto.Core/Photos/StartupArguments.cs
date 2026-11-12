namespace MVVMPhoto.Core.Photos;

/// <summary>Reads the photo to open from the app's launch arguments ("MVVMPhoto.exe C:\Pictures\a.jpg").</summary>
public static class StartupArguments
{
	/// <summary>
	/// The whole argument string as one path, with surrounding quotes and spaces removed; null when empty.
	/// One path only: a path may contain spaces, and Windows passes a single quoted path for "Open with".
	/// </summary>
	public static string? PhotoPath(string? arguments)
	{
		var path = arguments?.Trim().Trim('"').Trim();
		return string.IsNullOrEmpty(path) ? null : path;
	}
}
