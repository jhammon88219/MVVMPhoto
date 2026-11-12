using MVVMPhoto.ViewModels;

namespace MVVMPhoto.Tests;

public class ViewerViewModelTests
{
	[Fact]
	public void Pane_starts_expanded()
	{
		var viewer = new ViewerViewModel();

		Assert.True(viewer.IsPaneExpanded);
	}

	[Fact]
	public void TogglePane_collapses_then_expands()
	{
		var viewer = new ViewerViewModel();

		viewer.TogglePaneCommand.Execute(null);
		Assert.False(viewer.IsPaneExpanded);

		viewer.TogglePaneCommand.Execute(null);
		Assert.True(viewer.IsPaneExpanded);
	}

	[Fact]
	public void TogglePane_raises_property_changed()
	{
		var viewer = new ViewerViewModel();
		var raised = new List<string?>();
		viewer.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

		viewer.TogglePaneCommand.Execute(null);

		Assert.Equal([nameof(ViewerViewModel.IsPaneExpanded)], raised);
	}
}
