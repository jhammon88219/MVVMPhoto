using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using MVVMPhoto.Core.Photos;

namespace MVVMPhoto.Views.Viewer;

/// <summary>
/// x:Bind function that turns the view model's photo into something an Image can draw. Lives in the app,
/// not the view model, because ImageSource is a WinUI type.
/// </summary>
public static class PhotoImageSource
{
	/// <summary>A bitmap that loads (and decodes off the UI thread) from the photo's file; null for no photo.</summary>
	public static ImageSource? From(PhotoInfo? photo) =>
		photo is null ? null : new BitmapImage(new Uri(photo.FilePath));
}
