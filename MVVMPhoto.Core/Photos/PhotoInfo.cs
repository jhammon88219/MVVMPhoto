namespace MVVMPhoto.Core.Photos;

/// <summary>
/// What the viewer knows about one photo file. Read without decoding the full image.
/// </summary>
/// <param name="FilePath">Full path on disk.</param>
/// <param name="PixelWidth">Width as displayed, after the camera's rotation (EXIF orientation) is applied.</param>
/// <param name="PixelHeight">Height as displayed, after rotation.</param>
/// <param name="FileSizeBytes">Size of the file on disk.</param>
/// <param name="FormatName">Short format name, such as "JPEG" or "PNG".</param>
/// <param name="Modified">When the file was last written.</param>
public sealed record PhotoInfo(
	string FilePath,
	int PixelWidth,
	int PixelHeight,
	long FileSizeBytes,
	string FormatName,
	DateTimeOffset Modified)
{
	public string FileName => Path.GetFileName(FilePath);

	public string FolderPath => Path.GetDirectoryName(FilePath) ?? string.Empty;
}
