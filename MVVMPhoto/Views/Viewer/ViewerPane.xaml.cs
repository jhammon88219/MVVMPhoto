using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MVVMPhoto.ViewModels;

namespace MVVMPhoto.Views.Viewer;

public sealed partial class ViewerPane : UserControl
{
	public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
		nameof(ViewModel), typeof(ViewerViewModel), typeof(ViewerPane), new PropertyMetadata(null));

	public ViewerPane()
	{
		InitializeComponent();
	}

	public ViewerViewModel? ViewModel
	{
		get => (ViewerViewModel?)GetValue(ViewModelProperty);
		set => SetValue(ViewModelProperty, value);
	}
}
