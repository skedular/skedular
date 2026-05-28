using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace Enterprise.Shared.Image;

public interface IImageHelper
{
    bool IsImageFile(Stream stream);
    (bool IsImage, int Width, int Height) GetImageWidthHeight(Stream stream);
    (Stream ThumbnailStream, int Width, int Height, string ContentType) CreateThumbnail(Stream stream);
}

public class ImageHelper(ILogger<ImageHelper> logger) : IImageHelper
{
    private const int ThumbnailMaxSize = 200;

    public bool IsImageFile(Stream stream)
    {
        logger.LogDebug("Checking whether stream is an image. StreamCanSeek={StreamCanSeek}", stream.CanSeek);
        using var bitmap = DecodeStream(stream);
        if (bitmap is null)
        {
            logger.LogDebug("Image stream validation failed because the format is unknown");
            return false;
        }

        logger.LogInformation("Image stream validation succeeded. Width={Width}, Height={Height}", bitmap.Width, bitmap.Height);
        return true;
    }

    public (bool IsImage, int Width, int Height) GetImageWidthHeight(Stream stream)
    {
        logger.LogDebug("Reading image dimensions from stream. StreamCanSeek={StreamCanSeek}", stream.CanSeek);
        using var bitmap = DecodeStream(stream);
        if (bitmap is null)
        {
            logger.LogDebug("Image dimension read failed because the format is unknown");
            return (false, 0, 0);
        }

        logger.LogInformation("Image dimensions read successfully. Width={Width}, Height={Height}", bitmap.Width, bitmap.Height);
        return (true, bitmap.Width, bitmap.Height);
    }

    public (Stream ThumbnailStream, int Width, int Height, string ContentType) CreateThumbnail(Stream stream)
    {
        logger.LogDebug("Creating image thumbnail. StreamCanSeek={StreamCanSeek}", stream.CanSeek);
        using var source = DecodeStream(stream) ?? throw new InvalidOperationException("Stream does not contain a valid image.");
        var (thumbWidth, thumbHeight) = CalculateMaxSize(source.Width, source.Height, ThumbnailMaxSize, ThumbnailMaxSize);
        using var resized = source.Resize(new SKImageInfo(thumbWidth, thumbHeight), new SKSamplingOptions(SKFilterMode.Linear));
        using var image = SKImage.FromBitmap(resized);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        var thumbnailStream = new MemoryStream();
        data.SaveTo(thumbnailStream);
        logger.LogInformation("Created image thumbnail successfully. Width={Width}, Height={Height}", thumbWidth, thumbHeight);
        return (thumbnailStream, thumbWidth, thumbHeight, "image/png");
    }

    private static SKBitmap? DecodeStream(Stream stream)
    {
        // SKBitmap.Decode(Stream) passes stream ownership to SKData.Create() which closes the
        // stream after reading. Copy bytes first so the caller's stream stays open and seekable
        // for any later operations (e.g., uploading, then thumbnail the same stream).
        stream.Position = 0;
        byte[] bytes;
        if (stream is MemoryStream ms)
        {
            bytes = ms.ToArray();
        }
        else
        {
            using var buffer = new MemoryStream();
            stream.CopyTo(buffer);
            bytes = buffer.ToArray();
        }

        // In SkiaSharp 3.x, SKBitmap.Decode(byte[]) throws ArgumentNullException instead of
        // returning null when the data is not a valid image. Check via codec first.
        using var skData = SKData.CreateCopy(bytes);
        using var codec = SKCodec.Create(skData);
        return codec is null ? null : SKBitmap.Decode(codec);
    }

    private static (int Width, int Height) CalculateMaxSize(int sourceWidth, int sourceHeight, int maxWidth, int maxHeight)
    {
        if (sourceWidth <= maxWidth && sourceHeight <= maxHeight)
        {
            return (sourceWidth, sourceHeight);
        }

        var ratio = Math.Min((double)maxWidth / sourceWidth, (double)maxHeight / sourceHeight);
        return ((int)(sourceWidth * ratio), (int)(sourceHeight * ratio));
    }
}
