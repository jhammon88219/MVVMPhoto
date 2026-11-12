using MVVMPhoto.Core.Photos;

namespace MVVMPhoto.Tests;

public class PhotoSequenceTests
{
	private static readonly string[] _paths = [@"C:\p\a.jpg", @"C:\p\b.jpg", @"C:\p\c.jpg"];

	[Fact]
	public void Empty_has_no_position()
	{
		var empty = PhotoSequence.Empty;

		Assert.Equal(-1, empty.Index);
		Assert.Equal(0, empty.Count);
		Assert.Null(empty.CurrentPath);
		Assert.False(empty.HasNext);
		Assert.False(empty.HasPrevious);
		Assert.Same(empty, empty.Next());
	}

	[Fact]
	public void Create_finds_the_current_file_ignoring_case()
	{
		var sequence = PhotoSequence.Create(_paths, @"C:\P\B.JPG");

		Assert.Equal(1, sequence.Index);
		Assert.Equal(@"C:\p\b.jpg", sequence.CurrentPath);
	}

	[Fact]
	public void Create_falls_back_to_just_the_file_when_the_listing_lacks_it()
	{
		var sequence = PhotoSequence.Create(_paths, @"C:\p\new.jpg");

		Assert.Equal(1, sequence.Count);
		Assert.Equal(@"C:\p\new.jpg", sequence.CurrentPath);
	}

	[Fact]
	public void Next_and_previous_move_one_and_stop_at_the_ends()
	{
		var first = PhotoSequence.Create(_paths, _paths[0]);
		Assert.False(first.HasPrevious);
		Assert.Same(first, first.Previous());

		var last = first.Next().Next();
		Assert.Equal(_paths[2], last.CurrentPath);
		Assert.False(last.HasNext);
		Assert.Same(last, last.Next());

		Assert.Equal(_paths[1], last.Previous().CurrentPath);
	}

	[Fact]
	public void Moving_does_not_change_the_original()
	{
		var first = PhotoSequence.Create(_paths, _paths[0]);

		_ = first.Next();

		Assert.Equal(0, first.Index);
	}
}
