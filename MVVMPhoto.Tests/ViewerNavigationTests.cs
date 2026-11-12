using Microsoft.Extensions.Logging.Abstractions;
using MVVMPhoto.ViewModels;

namespace MVVMPhoto.Tests;

/// <summary>Previous / next through the open photo's folder, and the position counter.</summary>
public class ViewerNavigationTests
{
	private const string Folder = @"C:\Pictures\Trips";
	private const string A = @"C:\Pictures\Trips\a.jpg";
	private const string B = @"C:\Pictures\Trips\b.jpg";
	private const string C = @"C:\Pictures\Trips\c.jpg";

	private readonly FakePhotoFilePicker _picker = new();
	private readonly FakePhotoInfoReader _reader = new();
	private readonly FakePhotoFolderReader _folders = new();

	public ViewerNavigationTests()
	{
		_folders.Folders[Folder] = [A, B, C];
	}

	private ViewerViewModel CreateViewer() => new(_picker, _reader, _folders, NullLogger<ViewerViewModel>.Instance);

	[Fact]
	public void Nothing_open_shows_zero_of_zero_and_cannot_step()
	{
		var viewer = CreateViewer();

		Assert.Equal("0 / 0", viewer.PositionText);
		Assert.False(viewer.PreviousCommand.CanExecute(null));
		Assert.False(viewer.NextCommand.CanExecute(null));
	}

	[Fact]
	public async Task Opening_a_photo_positions_it_within_its_folder()
	{
		var viewer = CreateViewer();

		await viewer.OpenFileAsync(B);

		Assert.Equal("2 / 3", viewer.PositionText);
		Assert.True(viewer.PreviousCommand.CanExecute(null));
		Assert.True(viewer.NextCommand.CanExecute(null));
	}

	[Fact]
	public async Task Next_and_previous_show_the_neighbouring_photos()
	{
		var viewer = CreateViewer();
		await viewer.OpenFileAsync(B);

		await viewer.NextCommand.ExecuteAsync(null);
		Assert.Equal("c.jpg", viewer.FileName);
		Assert.Equal("3 / 3", viewer.PositionText);

		await viewer.PreviousCommand.ExecuteAsync(null);
		await viewer.PreviousCommand.ExecuteAsync(null);
		Assert.Equal("a.jpg", viewer.FileName);
		Assert.Equal("1 / 3", viewer.PositionText);
	}

	[Fact]
	public async Task Stepping_stops_at_the_ends()
	{
		var viewer = CreateViewer();

		await viewer.OpenFileAsync(A);
		Assert.False(viewer.PreviousCommand.CanExecute(null));

		await viewer.OpenFileAsync(C);
		Assert.False(viewer.NextCommand.CanExecute(null));
	}

	[Fact]
	public async Task A_broken_photo_reports_its_error_but_stepping_carries_on()
	{
		_reader.FailingPaths.Add(B);
		var viewer = CreateViewer();
		await viewer.OpenFileAsync(A);

		await viewer.NextCommand.ExecuteAsync(null);
		Assert.False(viewer.HasPhoto);
		Assert.NotNull(viewer.OpenError);
		Assert.Equal("2 / 3", viewer.PositionText);

		await viewer.NextCommand.ExecuteAsync(null);
		Assert.True(viewer.HasPhoto);
		Assert.Null(viewer.OpenError);
		Assert.Equal("c.jpg", viewer.FileName);
	}

	[Fact]
	public async Task When_reads_finish_out_of_order_the_newest_step_wins()
	{
		var viewer = CreateViewer();
		await viewer.OpenFileAsync(A);

		// Step to b (its read is held), then on to c before b's read finishes.
		var holdB = new TaskCompletionSource();
		_reader.Held[B] = holdB;
		var stepToB = viewer.NextCommand.ExecuteAsync(null);
		await viewer.NextCommand.ExecuteAsync(null);
		Assert.Equal("c.jpg", viewer.FileName);

		// b's read finishing late must not replace c.
		holdB.SetResult();
		await stepToB;
		Assert.Equal("c.jpg", viewer.FileName);
		Assert.Equal("3 / 3", viewer.PositionText);
	}

	[Fact]
	public async Task An_unreadable_folder_still_shows_the_photo_as_one_of_one()
	{
		var viewer = CreateViewer();
		const string Lonely = @"D:\Elsewhere\lonely.jpg";

		await viewer.OpenFileAsync(Lonely);

		Assert.True(viewer.HasPhoto);
		Assert.Equal("1 / 1", viewer.PositionText);
		Assert.False(viewer.NextCommand.CanExecute(null));
	}

	[Fact]
	public async Task Opening_an_unsupported_file_clears_the_position()
	{
		var viewer = CreateViewer();
		await viewer.OpenFileAsync(B);

		await viewer.OpenFileAsync(@"C:\Docs\notes.txt");

		Assert.Equal("0 / 0", viewer.PositionText);
		Assert.False(viewer.NextCommand.CanExecute(null));
	}

	[Fact]
	public async Task Changing_position_raises_the_counter_and_command_state()
	{
		var viewer = CreateViewer();
		await viewer.OpenFileAsync(A);
		var raised = new List<string?>();
		var nextChanged = false;
		viewer.PropertyChanged += (_, e) => raised.Add(e.PropertyName);
		viewer.NextCommand.CanExecuteChanged += (_, _) => nextChanged = true;

		await viewer.NextCommand.ExecuteAsync(null);

		Assert.Contains(nameof(ViewerViewModel.PositionText), raised);
		Assert.True(nextChanged);
	}
}
