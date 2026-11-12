using MVVMPhoto.Core.Photos;
using MVVMPhoto.Core.Services;

namespace MVVMPhoto.Tests;

/// <summary>Returns whatever path the test sets; null = the user cancelled.</summary>
internal sealed class FakePhotoFilePicker : IPhotoFilePicker
{
	public string? NextPath { get; set; }

	public Task<string?> PickPhotoAsync() => Task.FromResult(NextPath);
}

/// <summary>Returns a canned PhotoInfo for any path, or throws what the test sets.</summary>
internal sealed class FakePhotoInfoReader : IPhotoInfoReader
{
	public Exception? Throw { get; set; }

	public List<string> ReadPaths { get; } = [];

	public Task<PhotoInfo> ReadAsync(string filePath, CancellationToken cancellationToken = default)
	{
		ReadPaths.Add(filePath);
		return Throw is not null
			? Task.FromException<PhotoInfo>(Throw)
			: Task.FromResult(Sample(filePath));
	}

	public static PhotoInfo Sample(string filePath) =>
		new(filePath, 6000, 4000, 8_808_038, "JPEG", new DateTimeOffset(2026, 6, 14, 18, 42, 0, TimeSpan.Zero));
}
