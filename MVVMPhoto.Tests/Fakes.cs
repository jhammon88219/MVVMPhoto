using MVVMPhoto.Core.Photos;
using MVVMPhoto.Core.Services;

namespace MVVMPhoto.Tests;

/// <summary>Returns whatever path the test sets; null = the user cancelled.</summary>
internal sealed class FakePhotoFilePicker : IPhotoFilePicker
{
	public string? NextPath { get; set; }

	public Task<string?> PickPhotoAsync() => Task.FromResult(NextPath);
}

/// <summary>
/// Returns a canned PhotoInfo for any path, or throws what the test sets. A path in <see cref="Held"/>
/// doesn't finish until the test completes its TaskCompletionSource, to test reads that finish out of order.
/// </summary>
internal sealed class FakePhotoInfoReader : IPhotoInfoReader
{
	public Exception? Throw { get; set; }

	public HashSet<string> FailingPaths { get; } = new(StringComparer.OrdinalIgnoreCase);

	public Dictionary<string, TaskCompletionSource> Held { get; } = new(StringComparer.OrdinalIgnoreCase);

	public List<string> ReadPaths { get; } = [];

	public async Task<PhotoInfo> ReadAsync(string filePath, CancellationToken cancellationToken = default)
	{
		ReadPaths.Add(filePath);
		if (Held.TryGetValue(filePath, out var gate))
		{
			await gate.Task;
		}

		if (Throw is not null)
		{
			throw Throw;
		}

		if (FailingPaths.Contains(filePath))
		{
			throw new IOException("damaged");
		}

		return Sample(filePath);
	}

	public static PhotoInfo Sample(string filePath) =>
		new(filePath, 6000, 4000, 8_808_038, "JPEG", new DateTimeOffset(2026, 6, 14, 18, 42, 0, TimeSpan.Zero));
}

/// <summary>Returns the listing the test sets for a folder; an unknown folder throws like a missing one.</summary>
internal sealed class FakePhotoFolderReader : IPhotoFolderReader
{
	public Dictionary<string, IReadOnlyList<string>> Folders { get; } = new(StringComparer.OrdinalIgnoreCase);

	public Task<IReadOnlyList<string>> ListPhotosAsync(string folderPath, CancellationToken cancellationToken = default) =>
		Folders.TryGetValue(folderPath, out var paths)
			? Task.FromResult(paths)
			: Task.FromException<IReadOnlyList<string>>(new DirectoryNotFoundException(folderPath));
}
