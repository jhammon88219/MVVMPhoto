using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MVVMPhoto.ViewModels;

/// <summary>
/// The template's sample view model, kept until step 1c replaces it.
/// Observable state uses <c>[ObservableProperty]</c> on partial properties; commands use <c>[RelayCommand]</c>.
/// </summary>
public partial class MainPageViewModel : ObservableObject
{
	[ObservableProperty]
	public partial string Greeting { get; set; } = "Hello, WinUI!";

	[ObservableProperty]
	public partial int Counter { get; set; }

	[RelayCommand]
	private void Increment() => Counter++;

	[RelayCommand]
	private void Decrement() => Counter--;
}
