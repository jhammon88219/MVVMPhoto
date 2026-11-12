using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MVVMPhoto.ViewModels;

namespace MVVMPhoto.Views.Shell;

public sealed partial class ShellMenuBar : UserControl
{
	public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
		nameof(ViewModel), typeof(ShellViewModel), typeof(ShellMenuBar), new PropertyMetadata(null));

	public ShellMenuBar()
	{
		InitializeComponent();
	}

	public ShellViewModel? ViewModel
	{
		get => (ShellViewModel?)GetValue(ViewModelProperty);
		set => SetValue(ViewModelProperty, value);
	}
}
