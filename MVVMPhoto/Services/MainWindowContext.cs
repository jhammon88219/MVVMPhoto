using Microsoft.UI;

namespace MVVMPhoto.Services;

/// <summary>
/// Gives app services the main window's id (pickers and dialogs need an owner window).
/// MainWindow attaches itself in its constructor. A separate object, rather than injecting MainWindow,
/// because MainWindow depends on the view models that depend on these services: a cycle.
/// </summary>
public sealed class MainWindowContext
{
	private WindowId? _windowId;

	/// <summary>The main window's id. Throws if read before MainWindow exists.</summary>
	public WindowId WindowId =>
		_windowId ?? throw new InvalidOperationException("The main window has not been created yet.");

	public void Attach(WindowId windowId) => _windowId = windowId;
}
