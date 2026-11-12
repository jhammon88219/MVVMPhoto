namespace MVVMPhoto.Core.Photos;

/// <summary>
/// The photos the viewer steps through (one folder, in display order) and which one is current.
/// Immutable: moving returns a new sequence. Stepping stops at either end; it does not wrap around.
/// </summary>
public sealed class PhotoSequence
{
	private PhotoSequence(IReadOnlyList<string> paths, int index)
	{
		Paths = paths;
		Index = index;
	}

	/// <summary>No photos: nothing open, or the open file isn't a photo.</summary>
	public static PhotoSequence Empty { get; } = new([], -1);

	public IReadOnlyList<string> Paths { get; }

	/// <summary>Zero-based position of the current photo; -1 when empty.</summary>
	public int Index { get; }

	public int Count => Paths.Count;

	public string? CurrentPath => Index >= 0 ? Paths[Index] : null;

	public bool HasPrevious => Index > 0;

	public bool HasNext => Index >= 0 && Index < Count - 1;

	/// <summary>
	/// A sequence over <paramref name="paths"/>, positioned on <paramref name="currentPath"/> (matched without
	/// regard to case). If the listing doesn't contain it (say the folder changed in between), the sequence
	/// is just that one file, so the viewer still has a position.
	/// </summary>
	public static PhotoSequence Create(IReadOnlyList<string> paths, string currentPath)
	{
		for (var i = 0; i < paths.Count; i++)
		{
			if (string.Equals(paths[i], currentPath, StringComparison.OrdinalIgnoreCase))
			{
				return new PhotoSequence(paths, i);
			}
		}

		return new PhotoSequence([currentPath], 0);
	}

	/// <summary>The sequence moved one forward; this same sequence when already at the end.</summary>
	public PhotoSequence Next() => HasNext ? new PhotoSequence(Paths, Index + 1) : this;

	/// <summary>The sequence moved one back; this same sequence when already at the start.</summary>
	public PhotoSequence Previous() => HasPrevious ? new PhotoSequence(Paths, Index - 1) : this;
}
