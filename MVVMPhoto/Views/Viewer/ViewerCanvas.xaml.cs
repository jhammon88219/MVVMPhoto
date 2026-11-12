using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MVVMPhoto.ViewModels;

namespace MVVMPhoto.Views.Viewer;

public sealed partial class ViewerCanvas : UserControl
{
	public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
		nameof(ViewModel), typeof(ViewerViewModel), typeof(ViewerCanvas), new PropertyMetadata(null));

	public ViewerCanvas()
	{
		InitializeComponent();
	}

	public ViewerViewModel? ViewModel
	{
		get => (ViewerViewModel?)GetValue(ViewModelProperty);
		set => SetValue(ViewModelProperty, value);
	}
}
