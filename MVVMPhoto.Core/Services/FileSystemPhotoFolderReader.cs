using MVVMPhoto.Core.Photos;

namespace MVVMPhoto.Core.Services;

/// <summary>
/// <see cref="IPhotoFolderReader"/> over plain .NET file APIs (no Windows-specific code, so it lives in Core
/// and is tested against real temporary folders). Runs on a worker thread: a folder can hold thousands of files.
/// </summary>
public sealed class FileSystemPhotoFolderReader : IPhotoFolderReader
{
	// Hidden and system files are skipped, as Explorer does by default; unreadable entries are skipped
	// instead of failing the whole listing.
	private static readonly EnumerationOptions _options = new()
	{
		AttributesToSkip = FileAttributes.Hidden | FileAttributes.System,
		IgnoreInaccessible = true,
		RecurseSubdirectories = false,
	};

	public Task<IReadOnlyList<string>> ListPhotosAsync(string folderPath, CancellationToken cancellationToken = default) =>
		Task.Run<IReadOnlyList<string>>(
			() => Directory.EnumerateFiles(folderPath, "*", _options)
				.Where(PhotoFormats.IsSupported)
				.OrderBy(path => Path.GetFileName(path), NaturalSortComparer.Instance)
				.ToList(),
			cancellationToken);
}
