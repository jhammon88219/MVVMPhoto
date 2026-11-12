namespace MVVMPhoto.Core.Services;

/// <summary>Lists the photos in a folder, in the order the viewer shows them.</summary>
public interface IPhotoFolderReader
{
	/// <summary>
	/// Full paths of the supported photos directly in <paramref name="folderPath"/> (not sub-folders),
	/// sorted by name the way people read numbers. Throws if the folder can't be read.
	/// </summary>
	Task<IReadOnlyList<string>> ListPhotosAsync(string folderPath, CancellationToken cancellationToken = default);
}
