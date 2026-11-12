namespace MVVMPhoto.Core.Photos;

/// <summary>
/// The one list of file types the app opens. The picker filter, the startup check and (later) folder
/// browsing all read it. Package.appxmanifest lists the same extensions for Explorer's "Open with";
/// keep the two in step.
/// </summary>
public static class PhotoFormats
{
	/// <summary>Lower-case extensions with the leading dot.</summary>
	public static IReadOnlyList<string> Extensions { get; } =
	[
		".jpg", ".jpeg", ".jfif",
		".png",
		".gif",
		".bmp", ".dib",
		".tif", ".tiff",
		".webp",
		".heic", ".heif",
		".jxr", ".wdp",
		".ico",
	];

	private static readonly HashSet<string> _extensionSet = new(Extensions, StringComparer.OrdinalIgnoreCase);

	/// <summary>True when the path ends in one of <see cref="Extensions"/> (any letter case).</summary>
	public static bool IsSupported(string path) =>
		!string.IsNullOrWhiteSpace(path) && _extensionSet.Contains(Path.GetExtension(path));
}
