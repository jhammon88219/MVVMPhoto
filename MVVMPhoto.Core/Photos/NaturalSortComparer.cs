namespace MVVMPhoto.Core.Photos;

/// <summary>
/// Orders names the way people read them: runs of digits compare by number, so "IMG_2" comes before
/// "IMG_10" (Explorer does the same). Letters compare without regard to case.
/// </summary>
public sealed class NaturalSortComparer : IComparer<string>
{
	public static NaturalSortComparer Instance { get; } = new();

	private NaturalSortComparer()
	{
	}

	public int Compare(string? x, string? y)
	{
		if (ReferenceEquals(x, y))
		{
			return 0;
		}

		if (x is null)
		{
			return -1;
		}

		if (y is null)
		{
			return 1;
		}

		var i = 0;
		var j = 0;
		while (i < x.Length && j < y.Length)
		{
			if (char.IsAsciiDigit(x[i]) && char.IsAsciiDigit(y[j]))
			{
				var numberCompare = CompareNumberRuns(x, ref i, y, ref j);
				if (numberCompare != 0)
				{
					return numberCompare;
				}
			}
			else
			{
				var charCompare = char.ToUpperInvariant(x[i]).CompareTo(char.ToUpperInvariant(y[j]));
				if (charCompare != 0)
				{
					return charCompare;
				}

				i++;
				j++;
			}
		}

		// One name is a prefix of the other: the shorter comes first.
		var remaining = (x.Length - i).CompareTo(y.Length - j);

		// Still equal ("a.jpg" vs "A.jpg", "1" vs "01"): fall back to ordinal so the order is stable.
		return remaining != 0 ? remaining : string.CompareOrdinal(x, y);
	}

	/// <summary>Compares the digit runs starting at i and j by value, and moves both indexes past them.</summary>
	private static int CompareNumberRuns(string x, ref int i, string y, ref int j)
	{
		var xStart = i;
		var yStart = j;
		while (i < x.Length && char.IsAsciiDigit(x[i]))
		{
			i++;
		}

		while (j < y.Length && char.IsAsciiDigit(y[j]))
		{
			j++;
		}

		// Without leading zeros, a longer run is a bigger number; equal lengths compare digit by digit.
		// Works for any length, no overflow.
		var xDigits = x.AsSpan(xStart, i - xStart).TrimStart('0');
		var yDigits = y.AsSpan(yStart, j - yStart).TrimStart('0');
		if (xDigits.Length != yDigits.Length)
		{
			return xDigits.Length.CompareTo(yDigits.Length);
		}

		return Math.Sign(xDigits.SequenceCompareTo(yDigits));
	}
}
