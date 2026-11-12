using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MVVMPhoto.Core.Photos;
using MVVMPhoto.Core.Services;

namespace MVVMPhoto.ViewModels;

/// <summary>
/// The photo viewer: the side pane (expanded or collapsed to a rail), the photo on show, and the folder
/// it steps through. Zooming arrives in 2d.
/// </summary>
public partial class ViewerViewModel : ObservableObject
{
	private readonly IPhotoFilePicker _picker;
	private readonly IPhotoInfoReader _reader;
	private readonly IPhotoFolderReader _folderReader;
	private readonly ILogger<ViewerViewModel> _logger;

	// Bumped by every show / folder request. A request whose number is no longer current when its read
	// finishes is dropped, so fast stepping (holding an arrow key) always ends on the newest photo.
	private int _showVersion;
	private int _folderVersion;

	public ViewerViewModel(
		IPhotoFilePicker picker,
		IPhotoInfoReader reader,
		IPhotoFolderReader folderReader,
		ILogger<ViewerViewModel> logger)
	{
		_picker = picker;
		_reader = reader;
		_folderReader = folderReader;
		_logger = logger;
	}

	/// <summary>True = the full side pane; false = the narrow icon rail.</summary>
	[ObservableProperty]
	public partial bool IsPaneExpanded { get; set; } = true;

	/// <summary>The photo on show, or null before one is opened (or when the current file fails to open).</summary>
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(HasPhoto), nameof(FileName), nameof(HeaderTitle), nameof(HeaderSubtitle))]
	[NotifyPropertyChangedFor(nameof(DimensionsText), nameof(FileSizeText), nameof(TypeText), nameof(ModifiedText))]
	public partial PhotoInfo? Photo { get; private set; }

	/// <summary>Why the current file failed to open, in words for the user; null when it didn't.</summary>
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(HeaderSubtitle))]
	public partial string? OpenError { get; private set; }

	/// <summary>The folder being stepped through and the current position in it.</summary>
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PositionText))]
	[NotifyCanExecuteChangedFor(nameof(PreviousCommand), nameof(NextCommand))]
	public partial PhotoSequence Sequence { get; private set; } = PhotoSequence.Empty;

	public bool HasPhoto => Photo is not null;

	/// <summary>The open photo's file name, or empty. Shown in the title bar.</summary>
	public string FileName => Photo?.FileName ?? string.Empty;

	public string HeaderTitle => Photo?.FileName ?? "No photo open";

	public string HeaderSubtitle => OpenError ?? Photo?.FolderPath ?? "Open a photo to see it here";

	/// <summary>"12 / 340"; "0 / 0" with nothing open.</summary>
	public string PositionText =>
		string.Format(CultureInfo.CurrentCulture, "{0} / {1}", Sequence.Index + 1, Sequence.Count);

	public string DimensionsText =>
		Photo is { } photo ? PhotoDetailsText.Dimensions(photo.PixelWidth, photo.PixelHeight, CultureInfo.CurrentCulture) : PhotoDetailsText.None;

	public string FileSizeText =>
		Photo is { } photo ? PhotoDetailsText.FileSize(photo.FileSizeBytes, CultureInfo.CurrentCulture) : PhotoDetailsText.None;

	public string TypeText => Photo?.FormatName ?? PhotoDetailsText.None;

	public string ModifiedText =>
		Photo is { } photo ? PhotoDetailsText.Modified(photo.Modified, CultureInfo.CurrentCulture) : PhotoDetailsText.None;

	[RelayCommand]
	private void TogglePane() => IsPaneExpanded = !IsPaneExpanded;

	/// <summary>Asks the user for a photo, then opens it. Does nothing if they cancel.</summary>
	[RelayCommand]
	private async Task OpenAsync()
	{
		var filePath = await _picker.PickPhotoAsync();
		if (filePath is not null)
		{
			await OpenFileAsync(filePath);
		}
	}

	// AllowConcurrentExecutions: otherwise the button greys out while each photo loads (a flicker on every
	// press), and a key held down would be ignored until the read finishes. Stale reads are dropped instead.
	[RelayCommand(CanExecute = nameof(CanGoPrevious), AllowConcurrentExecutions = true)]
	private Task PreviousAsync() => StepAsync(Sequence.Previous());

	[RelayCommand(CanExecute = nameof(CanGoNext), AllowConcurrentExecutions = true)]
	private Task NextAsync() => StepAsync(Sequence.Next());

	private bool CanGoPrevious() => Sequence.HasPrevious;

	private bool CanGoNext() => Sequence.HasNext;

	/// <summary>
	/// Opens <paramref name="filePath"/> (from the picker, Explorer or the command line), then lists its
	/// folder for previous / next. Never throws: a file that fails to open is reported in <see cref="OpenError"/>.
	/// </summary>
	public async Task OpenFileAsync(string filePath)
	{
		if (!PhotoFormats.IsSupported(filePath))
		{
			LogUnsupportedType(filePath);
			_showVersion++;
			_folderVersion++;
			Sequence = PhotoSequence.Empty;
			Show(null, $"{Path.GetFileName(filePath)} isn't a photo type MVVMPhoto opens.");
			return;
		}

		// Position on just this file straight away; the folder listing fills in the rest.
		Sequence = PhotoSequence.Create([filePath], filePath);
		await ShowFileAsync(filePath);
		await LoadFolderAsync(filePath);
	}

	private async Task StepAsync(PhotoSequence moved)
	{
		Sequence = moved;
		if (moved.CurrentPath is { } path)
		{
			await ShowFileAsync(path);
		}
	}

	private async Task ShowFileAsync(string filePath)
	{
		var version = ++_showVersion;
		try
		{
			var photo = await _reader.ReadAsync(filePath);
			if (version == _showVersion)
			{
				LogOpened(filePath, photo.PixelWidth, photo.PixelHeight, photo.FormatName);
				Show(photo, null);
			}
		}
		catch (Exception ex)
		{
			// Any failure here is a file problem (missing, locked, damaged, no codec), not an app bug:
			// report it and carry on; previous / next still work. The exception is logged in full.
			LogOpenFailed(ex, filePath);
			if (version == _showVersion)
			{
				Show(null, $"Couldn't open {Path.GetFileName(filePath)}. It may be damaged, or in a format Windows can't read.");
			}
		}
	}

	private async Task LoadFolderAsync(string filePath)
	{
		var version = ++_folderVersion;
		var folderPath = Path.GetDirectoryName(filePath);
		if (string.IsNullOrEmpty(folderPath))
		{
			return;
		}

		try
		{
			var paths = await _folderReader.ListPhotosAsync(folderPath);
			if (version == _folderVersion)
			{
				// Re-anchor on the photo showing now: the user may have stepped while the folder was listed.
				Sequence = PhotoSequence.Create(paths, Sequence.CurrentPath ?? filePath);
				LogFolderListed(folderPath, paths.Count);
			}
		}
		catch (Exception ex)
		{
			// The photo itself is open; only stepping is lost. Keep the one-file sequence.
			LogFolderFailed(ex, folderPath);
		}
	}

	private void Show(PhotoInfo? photo, string? error)
	{
		OpenError = error;
		Photo = photo;
	}

	[LoggerMessage(Level = LogLevel.Information, Message = "Opened {FilePath} ({Width} x {Height}, {Format})")]
	private partial void LogOpened(string filePath, int width, int height, string format);

	[LoggerMessage(Level = LogLevel.Warning, Message = "Not a supported photo type: {FilePath}")]
	private partial void LogUnsupportedType(string filePath);

	[LoggerMessage(Level = LogLevel.Warning, Message = "Couldn't open {FilePath}")]
	private partial void LogOpenFailed(Exception exception, string filePath);

	[LoggerMessage(Level = LogLevel.Information, Message = "Listed {Count} photos in {FolderPath}")]
	private partial void LogFolderListed(string folderPath, int count);

	[LoggerMessage(Level = LogLevel.Warning, Message = "Couldn't list the photos in {FolderPath}")]
	private partial void LogFolderFailed(Exception exception, string folderPath);
}
