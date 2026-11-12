namespace MVVMPhoto.Core.Services;

/// <summary>Asks the user to choose a photo file.</summary>
public interface IPhotoFilePicker
{
	/// <summary>The chosen file's full path, or null if the user cancelled.</summary>
	Task<string?> PickPhotoAsync();
}
