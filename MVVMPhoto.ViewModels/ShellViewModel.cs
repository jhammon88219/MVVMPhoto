using CommunityToolkit.Mvvm.ComponentModel;

namespace MVVMPhoto.ViewModels;

/// <summary>
/// The main window's view model. It owns the app's top-level view models (library, viewer, ...) as
/// constructor-injected children, plus the state that decides which of them is showing.
/// MainWindow.xaml declares one view per child and binds each to it; nothing navigates in code.
/// Empty until step 2 adds the viewer.
/// </summary>
public partial class ShellViewModel : ObservableObject
{
}
