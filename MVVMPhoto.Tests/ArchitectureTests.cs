using System.Reflection;

namespace MVVMPhoto.Tests;

/// <summary>
/// Guards the project split: Core and ViewModels must never depend on WinUI or WinRT.
/// If one of these fails, the new code belongs in the app project behind a Core interface.
/// </summary>
public class ArchitectureTests
{
	// Assembly-name prefixes that mean "this project now knows about Windows UI".
	private static readonly string[] _forbiddenPrefixes =
	[
		"Microsoft.UI",          // WinUI 3 (Microsoft.UI.Xaml, Microsoft.UI.Dispatching, ...)
		"Microsoft.WinUI",
		"Microsoft.WindowsAppRuntime",
		"Microsoft.Windows.SDK.NET", // WinRT projections (Windows.Storage, Windows.Graphics, ...)
		"WinRT.Runtime",
	];

	[Theory]
	[InlineData("MVVMPhoto.Core")]
	[InlineData("MVVMPhoto.ViewModels")]
	public void Project_does_not_reference_WinUI_or_WinRT(string assemblyName)
	{
		var assembly = Assembly.Load(assemblyName);

		var forbidden = assembly.GetReferencedAssemblies()
			.Select(reference => reference.Name ?? string.Empty)
			.Where(name => _forbiddenPrefixes.Any(prefix => name.StartsWith(prefix, StringComparison.Ordinal)))
			.ToList();

		Assert.Empty(forbidden);
	}
}
