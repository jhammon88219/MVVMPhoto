using MVVMPhoto.Core.Photos;

namespace MVVMPhoto.Core.Services;

/// <summary>Reads a photo's size, format and file facts without decoding the whole image.</summary>
public interface IPhotoInfoReader
{
	/// <summary>
	/// Reads <paramref name="filePath"/>. Throws if the file is missing, unreadable, or not an image
	/// the system can decode.
	/// </summary>
	Task<PhotoInfo> ReadAsync(string filePath, CancellationToken cancellationToken = default);
}
