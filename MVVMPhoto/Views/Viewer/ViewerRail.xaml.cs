using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MVVMPhoto.ViewModels;

namespace MVVMPhoto.Views.Viewer;

public sealed partial class ViewerRail : UserControl
{
	public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
		nameof(ViewModel), typeof(ViewerViewModel), typeof(ViewerRail), new PropertyMetadata(null));

	public ViewerRail()
	{
		InitializeComponent();
	}

	public ViewerViewModel? ViewModel
	{
		get => (ViewerViewModel?)GetValue(ViewModelProperty);
		set => SetValue(ViewModelProperty, value);
	}
}
