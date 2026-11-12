using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MVVMPhoto.ViewModels;

namespace MVVMPhoto.Views.Viewer;

public sealed partial class ViewerView : UserControl
{
	public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
		nameof(ViewModel), typeof(ViewerViewModel), typeof(ViewerView), new PropertyMetadata(null));

	public ViewerView()
	{
		InitializeComponent();
	}

	public ViewerViewModel? ViewModel
	{
		get => (ViewerViewModel?)GetValue(ViewModelProperty);
		set => SetValue(ViewModelProperty, value);
	}
}
