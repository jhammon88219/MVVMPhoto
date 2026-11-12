using Microsoft.UI.Xaml.Controls;

namespace MVVMPhoto.Views.Viewer;

/// <summary>The photo area. Takes no view model yet; step 2b adds one when it shows a photo.</summary>
public sealed partial class ViewerCanvas : UserControl
{
	public ViewerCanvas()
	{
		InitializeComponent();
	}
}
