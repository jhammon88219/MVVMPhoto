using Microsoft.Windows.Storage.Pickers;
using MVVMPhoto.Core.Photos;
using MVVMPhoto.Core.Services;

namespace MVVMPhoto.Services;

/// <summary><see cref="IPhotoFilePicker"/> using the Windows App SDK file picker, owned by the main window.</summary>
public sealed class PhotoFilePicker(MainWindowContext window) : IPhotoFilePicker
{
	public async Task<string?> PickPhotoAsync()
	{
		var picker = new FileOpenPicker(window.WindowId)
		{
			SuggestedStartLocation = PickerLocationId.PicturesLibrary,
			ViewMode = PickerViewMode.Thumbnail,
		};

		foreach (var extension in PhotoFormats.Extensions)
		{
			picker.FileTypeFilter.Add(extension);
		}

		var result = await picker.PickSingleFileAsync();
		return result?.Path;
	}
}
