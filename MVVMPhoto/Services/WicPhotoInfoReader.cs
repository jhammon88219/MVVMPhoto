using MVVMPhoto.Core.Photos;
using MVVMPhoto.Core.Services;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace MVVMPhoto.Services;

/// <summary>
/// <see cref="IPhotoInfoReader"/> backed by Windows Imaging Component (WIC), which decodes every format
/// with an installed codec (HEIC, WebP and camera RAW included when present). Creating the decoder
/// reads only the header, not the pixels.
/// </summary>
public sealed class WicPhotoInfoReader : IPhotoInfoReader
{
	private const string DecoderSuffix = " Decoder";

	public async Task<PhotoInfo> ReadAsync(string filePath, CancellationToken cancellationToken = default)
	{
		var file = await StorageFile.GetFileFromPathAsync(filePath).AsTask(cancellationToken);
		using var stream = await file.OpenReadAsync().AsTask(cancellationToken);
		var decoder = await BitmapDecoder.CreateAsync(stream).AsTask(cancellationToken);

		var fileInfo = new FileInfo(filePath);

		return new PhotoInfo(
			filePath,
			checked((int)decoder.OrientedPixelWidth),
			checked((int)decoder.OrientedPixelHeight),
			fileInfo.Length,
			FormatName(decoder.DecoderInformation.FriendlyName),
			fileInfo.LastWriteTime);
	}

	// WIC names its decoders "JPEG Decoder", "PNG Decoder", ...; the format is the part before " Decoder".
	private static string FormatName(string decoderName) =>
		decoderName.EndsWith(DecoderSuffix, StringComparison.OrdinalIgnoreCase)
			? decoderName[..^DecoderSuffix.Length]
			: decoderName;
}
