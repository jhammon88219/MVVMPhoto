using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MVVMPhoto.ViewModels;

/// <summary>
/// The photo viewer: the side pane (expanded or collapsed to a rail) and the photo area.
/// Step 2a holds only the pane state; opening, browsing and zooming arrive in 2b to 2d.
/// </summary>
public partial class ViewerViewModel : ObservableObject
{
	/// <summary>True = the full side pane; false = the narrow icon rail.</summary>
	[ObservableProperty]
	public partial bool IsPaneExpanded { get; set; } = true;

	[RelayCommand]
	private void TogglePane() => IsPaneExpanded = !IsPaneExpanded;
}
