using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MVVMPhoto.Core.Photos;
using MVVMPhoto.Core.Services;

namespace MVVMPhoto.ViewModels;

/// <summary>
/// The photo viewer: the side pane (expanded or collapsed to a rail) and the photo on show.
/// Step 2b: open one photo and describe it. Browsing the folder and zooming arrive in 2c and 2d.
/// </summary>
public partial class ViewerViewModel : ObservableObject
{
	private readonly IPhotoFilePicker _picker;
	private readonly IPhotoInfoReader _reader;
	private readonly ILogger<ViewerViewModel> _logger;

	public ViewerViewModel(IPhotoFilePicker picker, IPhotoInfoReader reader, ILogger<ViewerViewModel> logger)
	{
		_picker = picker;
		_reader = reader;
		_logger = logger;
	}

	/// <summary>True = the full side pane; false = the narrow icon rail.</summary>
	[ObservableProperty]
	public partial bool IsPaneExpanded { get; set; } = true;

	/// <summary>The photo on show, or null before one is opened (or after one fails to open).</summary>
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(HasPhoto), nameof(FileName), nameof(HeaderTitle), nameof(HeaderSubtitle))]
	[NotifyPropertyChangedFor(nameof(DimensionsText), nameof(FileSizeText), nameof(TypeText), nameof(ModifiedText))]
	public partial PhotoInfo? Photo { get; private set; }

	/// <summary>Why the last open failed, in words for the user; null when it didn't.</summary>
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(HeaderSubtitle))]
	public partial string? OpenError { get; private set; }

	public bool HasPhoto => Photo is not null;

	/// <summary>The open photo's file name, or empty. Shown in the title bar.</summary>
	public string FileName => Photo?.FileName ?? string.Empty;

	public string HeaderTitle => Photo?.FileName ?? "No photo open";

	public string HeaderSubtitle => OpenError ?? Photo?.FolderPath ?? "Open a photo to see it here";

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

	/// <summary>
	/// Opens <paramref name="filePath"/> (from the picker, Explorer or the command line). On failure the
	/// viewer goes back to its empty state and <see cref="OpenError"/> says why; it never throws.
	/// </summary>
	public async Task OpenFileAsync(string filePath)
	{
		var fileName = Path.GetFileName(filePath);

		if (!PhotoFormats.IsSupported(filePath))
		{
			LogUnsupportedType(filePath);
			Show(null, $"{fileName} isn't a photo type MVVMPhoto opens.");
			return;
		}

		try
		{
			var photo = await _reader.ReadAsync(filePath);
			LogOpened(filePath, photo.PixelWidth, photo.PixelHeight, photo.FormatName);
			Show(photo, null);
		}
		catch (Exception ex)
		{
			// Any failure here is a file problem (missing, locked, damaged, no codec), not an app bug:
			// report it and carry on. The exception is logged in full.
			LogOpenFailed(ex, filePath);
			Show(null, $"Couldn't open {fileName}. It may be damaged, or in a format Windows can't read.");
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
}
