using System.Globalization;
using MVVMPhoto.ViewModels;

namespace MVVMPhoto.Tests;

public class PhotoDetailsTextTests
{
	private static readonly CultureInfo _invariant = CultureInfo.InvariantCulture;

	[Theory]
	[InlineData(0, "0 bytes")]
	[InlineData(1, "1 byte")]
	[InlineData(1023, "1023 bytes")]
	[InlineData(1024, "1 KB")]
	[InlineData(1536, "1.5 KB")]
	[InlineData(8_808_038, "8.4 MB")]
	[InlineData(1_073_741_824, "1 GB")]
	[InlineData(5_497_558_138_880, "5 TB")]
	public void FileSize_uses_binary_units(long bytes, string expected)
	{
		Assert.Equal(expected, PhotoDetailsText.FileSize(bytes, _invariant));
	}

	[Fact]
	public void FileSize_follows_the_culture_decimal_separator()
	{
		Assert.Equal("1,5 KB", PhotoDetailsText.FileSize(1536, CultureInfo.GetCultureInfo("de-DE")));
	}

	[Fact]
	public void Dimensions_is_width_by_height()
	{
		Assert.Equal("6000 × 4000", PhotoDetailsText.Dimensions(6000, 4000, _invariant));
	}
}
