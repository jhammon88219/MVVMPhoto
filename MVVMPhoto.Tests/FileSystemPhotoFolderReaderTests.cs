using MVVMPhoto.Core.Services;

namespace MVVMPhoto.Tests;

/// <summary>Runs the real folder reader against a temporary folder, deleted afterwards.</summary>
public sealed class FileSystemPhotoFolderReaderTests : IDisposable
{
	private readonly string _folder = Directory.CreateTempSubdirectory("MVVMPhotoTests_").FullName;
	private readonly FileSystemPhotoFolderReader _reader = new();

	public void Dispose() => Directory.Delete(_folder, recursive: true);

	private string Touch(string name)
	{
		var path = Path.Combine(_folder, name);
		File.WriteAllBytes(path, []);
		return path;
	}

	[Fact]
	public async Task Lists_only_photos_in_natural_order()
	{
		Touch("IMG_10.jpg");
		Touch("IMG_2.PNG");
		Touch("notes.txt");
		Touch("IMG_1.heic");

		var names = (await _reader.ListPhotosAsync(_folder, TestContext.Current.CancellationToken))
			.Select(Path.GetFileName);

		Assert.Equal(["IMG_1.heic", "IMG_2.PNG", "IMG_10.jpg"], names);
	}

	[Fact]
	public async Task Returns_full_paths()
	{
		var path = Touch("a.jpg");

		var paths = await _reader.ListPhotosAsync(_folder, TestContext.Current.CancellationToken);

		Assert.Equal([path], paths);
	}

	[Fact]
	public async Task Skips_sub_folders_and_hidden_files()
	{
		Touch("visible.jpg");
		File.SetAttributes(Touch("hidden.jpg"), FileAttributes.Hidden);
		var sub = Directory.CreateDirectory(Path.Combine(_folder, "sub")).FullName;
		File.WriteAllBytes(Path.Combine(sub, "nested.jpg"), []);

		var names = (await _reader.ListPhotosAsync(_folder, TestContext.Current.CancellationToken))
			.Select(Path.GetFileName);

		Assert.Equal(["visible.jpg"], names);
	}

	[Fact]
	public async Task A_missing_folder_throws()
	{
		await Assert.ThrowsAsync<DirectoryNotFoundException>(
			() => _reader.ListPhotosAsync(Path.Combine(_folder, "missing"), TestContext.Current.CancellationToken));
	}
}
