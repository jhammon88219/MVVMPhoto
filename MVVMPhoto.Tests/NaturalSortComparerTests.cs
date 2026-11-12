using MVVMPhoto.Core.Photos;

namespace MVVMPhoto.Tests;

public class NaturalSortComparerTests
{
	private static List<string> Sorted(params string[] names) =>
		[.. names.Order(NaturalSortComparer.Instance)];

	[Fact]
	public void Numbers_compare_by_value()
	{
		Assert.Equal(
			["IMG_1.jpg", "IMG_2.jpg", "IMG_10.jpg", "IMG_100.jpg"],
			Sorted("IMG_10.jpg", "IMG_2.jpg", "IMG_100.jpg", "IMG_1.jpg"));
	}

	[Fact]
	public void Letters_ignore_case()
	{
		Assert.Equal(["apple.jpg", "Banana.jpg", "cherry.jpg"], Sorted("cherry.jpg", "Banana.jpg", "apple.jpg"));
	}

	[Fact]
	public void Several_number_runs_each_compare_by_value()
	{
		Assert.Equal(
			["2026-6-9.jpg", "2026-6-10.jpg", "2026-11-1.jpg"],
			Sorted("2026-11-1.jpg", "2026-6-10.jpg", "2026-6-9.jpg"));
	}

	[Fact]
	public void Leading_zeros_do_not_change_the_value()
	{
		Assert.Equal(["IMG_007.jpg", "IMG_08.jpg", "IMG_9.jpg"], Sorted("IMG_9.jpg", "IMG_08.jpg", "IMG_007.jpg"));
	}

	[Fact]
	public void Very_long_numbers_do_not_overflow()
	{
		Assert.Equal(
			["99999999999999999999.jpg", "100000000000000000000.jpg"],
			Sorted("100000000000000000000.jpg", "99999999999999999999.jpg"));
	}

	[Fact]
	public void A_prefix_sorts_first()
	{
		Assert.Equal(["photo", "photo 2", "photo2"], Sorted("photo2", "photo 2", "photo"));
	}

	[Fact]
	public void Equal_looking_names_still_have_a_stable_order()
	{
		var comparer = NaturalSortComparer.Instance;

		Assert.NotEqual(0, comparer.Compare("a.jpg", "A.jpg"));
		Assert.Equal(-comparer.Compare("a.jpg", "A.jpg"), comparer.Compare("A.jpg", "a.jpg"));
		Assert.Equal(0, comparer.Compare("a.jpg", "a.jpg"));
	}

	[Fact]
	public void Null_sorts_first()
	{
		Assert.True(NaturalSortComparer.Instance.Compare(null, "a") < 0);
		Assert.True(NaturalSortComparer.Instance.Compare("a", null) > 0);
	}
}
