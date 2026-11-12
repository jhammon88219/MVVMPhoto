using MVVMPhoto.Core.Photos;

namespace MVVMPhoto.Tests;

public class StartupArgumentsTests
{
	[Theory]
	[InlineData(@"C:\Pictures\a.jpg", @"C:\Pictures\a.jpg")]
	[InlineData(@"""C:\My Pictures\a b.jpg""", @"C:\My Pictures\a b.jpg")]
	[InlineData(@"  ""C:\Pictures\a.jpg""  ", @"C:\Pictures\a.jpg")]
	[InlineData(@"C:\My Pictures\a b.jpg", @"C:\My Pictures\a b.jpg")]
	public void PhotoPath_is_the_argument_without_quotes(string arguments, string expected)
	{
		Assert.Equal(expected, StartupArguments.PhotoPath(arguments));
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(@"""""")]
	public void No_arguments_means_no_photo(string? arguments)
	{
		Assert.Null(StartupArguments.PhotoPath(arguments));
	}
}
