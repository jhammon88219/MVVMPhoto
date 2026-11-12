using CommunityToolkit.Mvvm.ComponentModel;

namespace MVVMPhoto.ViewModels;

/// <summary>
/// The main window's view model. It owns the app's top-level view models (viewer, later the library) as
/// constructor-injected children, plus the state that decides which of them is showing.
/// MainWindow.xaml declares one view per child and binds each to it; nothing navigates in code.
/// </summary>
public partial class ShellViewModel(ViewerViewModel viewer) : ObservableObject
{
	public ViewerViewModel Viewer { get; } = viewer;
}
