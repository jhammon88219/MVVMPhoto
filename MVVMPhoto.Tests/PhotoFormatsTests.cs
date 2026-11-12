using MVVMPhoto.Core.Photos;

namespace MVVMPhoto.Tests;

public class PhotoFormatsTests
{
	[Theory]
	[InlineData(@"C:\a\photo.jpg")]
	[InlineData(@"C:\a\PHOTO.JPG")]
	[InlineData(@"C:\a\photo.HeIc")]
	[InlineData(@"C:\a\photo.webp")]
	[InlineData("relative.png")]
	public void Photo_extensions_are_supported(string path)
	{
		Assert.True(PhotoFormats.IsSupported(path));
	}

	[Theory]
	[InlineData(@"C:\a\notes.txt")]
	[InlineData(@"C:\a\photo")]
	[InlineData(@"C:\a\photo.jpg.exe")]
	[InlineData("")]
	[InlineData("   ")]
	public void Other_paths_are_not(string path)
	{
		Assert.False(PhotoFormats.IsSupported(path));
	}

	[Fact]
	public void Extensions_are_lower_case_with_a_dot()
	{
		Assert.All(PhotoFormats.Extensions, extension =>
		{
			Assert.StartsWith(".", extension, StringComparison.Ordinal);
			Assert.Equal(extension.ToLowerInvariant(), extension);
		});
	}
}
