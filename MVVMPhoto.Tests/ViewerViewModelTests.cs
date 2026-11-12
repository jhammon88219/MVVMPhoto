using Microsoft.Extensions.Logging.Abstractions;
using MVVMPhoto.ViewModels;

namespace MVVMPhoto.Tests;

public class ViewerViewModelTests
{
	private const string PhotoPath = @"C:\Pictures\Trips\IMG_2041.jpg";

	private readonly FakePhotoFilePicker _picker = new();
	private readonly FakePhotoInfoReader _reader = new();

	private ViewerViewModel CreateViewer() => new(_picker, _reader, NullLogger<ViewerViewModel>.Instance);

	// Pane

	[Fact]
	public void Pane_starts_expanded()
	{
		Assert.True(CreateViewer().IsPaneExpanded);
	}

	[Fact]
	public void TogglePane_collapses_then_expands()
	{
		var viewer = CreateViewer();

		viewer.TogglePaneCommand.Execute(null);
		Assert.False(viewer.IsPaneExpanded);

		viewer.TogglePaneCommand.Execute(null);
		Assert.True(viewer.IsPaneExpanded);
	}

	[Fact]
	public void TogglePane_raises_property_changed()
	{
		var viewer = CreateViewer();
		var raised = new List<string?>();
		viewer.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

		viewer.TogglePaneCommand.Execute(null);

		Assert.Equal([nameof(ViewerViewModel.IsPaneExpanded)], raised);
	}

	// Empty state

	[Fact]
	public void Starts_with_no_photo_and_placeholder_text()
	{
		var viewer = CreateViewer();

		Assert.False(viewer.HasPhoto);
		Assert.Equal("No photo open", viewer.HeaderTitle);
		Assert.Equal("Open a photo to see it here", viewer.HeaderSubtitle);
		Assert.Equal("-", viewer.DimensionsText);
		Assert.Equal("-", viewer.FileSizeText);
		Assert.Equal("-", viewer.TypeText);
		Assert.Equal("-", viewer.ModifiedText);
	}

	// Opening

	[Fact]
	public async Task OpenFileAsync_shows_the_photo_and_its_details()
	{
		var viewer = CreateViewer();

		await viewer.OpenFileAsync(PhotoPath);

		Assert.True(viewer.HasPhoto);
		Assert.Null(viewer.OpenError);
		Assert.Equal("IMG_2041.jpg", viewer.HeaderTitle);
		Assert.Equal(@"C:\Pictures\Trips", viewer.HeaderSubtitle);
		Assert.Equal("JPEG", viewer.TypeText);
	}

	[Fact]
	public async Task OpenFileAsync_raises_change_for_every_text_the_pane_binds()
	{
		var viewer = CreateViewer();
		var raised = new HashSet<string?>();
		viewer.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

		await viewer.OpenFileAsync(PhotoPath);

		Assert.Superset(
			new HashSet<string?>
			{
				nameof(ViewerViewModel.Photo), nameof(ViewerViewModel.HasPhoto),
				nameof(ViewerViewModel.HeaderTitle), nameof(ViewerViewModel.HeaderSubtitle),
				nameof(ViewerViewModel.DimensionsText), nameof(ViewerViewModel.FileSizeText),
				nameof(ViewerViewModel.TypeText), nameof(ViewerViewModel.ModifiedText),
			},
			raised);
	}

	[Fact]
	public async Task OpenFileAsync_rejects_an_unsupported_type_without_reading_it()
	{
		var viewer = CreateViewer();

		await viewer.OpenFileAsync(@"C:\Docs\notes.txt");

		Assert.False(viewer.HasPhoto);
		Assert.Empty(_reader.ReadPaths);
		Assert.Equal("notes.txt isn't a photo type MVVMPhoto opens.", viewer.OpenError);
		Assert.Equal(viewer.OpenError, viewer.HeaderSubtitle);
	}

	[Fact]
	public async Task OpenFileAsync_reports_a_read_failure_and_clears_the_previous_photo()
	{
		var viewer = CreateViewer();
		await viewer.OpenFileAsync(PhotoPath);
		_reader.Throw = new IOException("damaged");

		await viewer.OpenFileAsync(@"C:\Pictures\broken.jpg");

		Assert.False(viewer.HasPhoto);
		Assert.Equal("No photo open", viewer.HeaderTitle);
		Assert.StartsWith("Couldn't open broken.jpg.", viewer.OpenError, StringComparison.Ordinal);
	}

	[Fact]
	public async Task A_successful_open_clears_the_previous_error()
	{
		var viewer = CreateViewer();
		await viewer.OpenFileAsync(@"C:\Docs\notes.txt");

		await viewer.OpenFileAsync(PhotoPath);

		Assert.Null(viewer.OpenError);
		Assert.Equal(@"C:\Pictures\Trips", viewer.HeaderSubtitle);
	}

	[Fact]
	public async Task OpenCommand_opens_the_picked_file()
	{
		var viewer = CreateViewer();
		_picker.NextPath = PhotoPath;

		await viewer.OpenCommand.ExecuteAsync(null);

		Assert.Equal([PhotoPath], _reader.ReadPaths);
		Assert.True(viewer.HasPhoto);
	}

	[Fact]
	public async Task OpenCommand_does_nothing_when_the_picker_is_cancelled()
	{
		var viewer = CreateViewer();
		_picker.NextPath = null;

		await viewer.OpenCommand.ExecuteAsync(null);

		Assert.Empty(_reader.ReadPaths);
		Assert.False(viewer.HasPhoto);
		Assert.Null(viewer.OpenError);
	}
}
