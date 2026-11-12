using Microsoft.UI.Xaml;
using MVVMPhoto.Services;
using MVVMPhoto.ViewModels;

namespace MVVMPhoto;

/// <summary>
/// The app shell window. Created by the DI container; its view model is injected.
/// </summary>
public sealed partial class MainWindow : Window
{
	public MainWindow(ShellViewModel viewModel, MainWindowContext windowContext)
	{
		// Important: assign before InitializeComponent, or x:Bind reads a null ViewModel.
		ViewModel = viewModel;

		InitializeComponent();

		// Lets pickers and dialogs find their owner window.
		windowContext.Attach(AppWindow.Id);

		ExtendsContentIntoTitleBar = true;
		SetTitleBar(AppTitleBar);

		AppWindow.SetIcon("Assets/AppIcon.ico");
	}

	public ShellViewModel ViewModel { get; }
}
