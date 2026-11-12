namespace MVVMPhoto.ViewModels;

/// <summary>
/// Turns photo facts into the short text the pane shows. Pure functions, so they're unit-tested
/// with a fixed culture; the view model passes the user's culture.
/// </summary>
public static class PhotoDetailsText
{
	/// <summary>Shown in place of a value when there is no photo.</summary>
	public const string None = "-";

	private static readonly string[] _sizeUnits = ["KB", "MB", "GB", "TB"];

	public static string Dimensions(int width, int height, IFormatProvider culture) =>
		string.Format(culture, "{0} × {1}", width, height);

	/// <summary>Binary units (1 KB = 1024 bytes), as Explorer shows them: "512 bytes", "8.4 MB".</summary>
	public static string FileSize(long bytes, IFormatProvider culture)
	{
		if (bytes < 1024)
		{
			return bytes == 1 ? "1 byte" : string.Format(culture, "{0} bytes", bytes);
		}

		var value = bytes / 1024d;
		var unit = 0;
		while (value >= 1024 && unit < _sizeUnits.Length - 1)
		{
			value /= 1024;
			unit++;
		}

		return string.Format(culture, "{0:0.#} {1}", value, _sizeUnits[unit]);
	}

	/// <summary>The culture's short date and time.</summary>
	public static string Modified(DateTimeOffset modified, IFormatProvider culture) =>
		modified.ToString("g", culture);
}
