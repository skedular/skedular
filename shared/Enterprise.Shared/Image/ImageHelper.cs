using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Enterprise.Shared.Image;

public interface IImageHelper
{
    Task<bool> IsImageFileAsync(Stream stream, CancellationToken cancellationToken);
    Task<(bool IsImage, int Width, int Height)> GetImageWidthHeightAsync(Stream stream, CancellationToken cancellationToken);

    Task<(Stream ThumbnailStream, int Width, int Height, string ContentType)> CreateThumbnailAsync(
        Stream stream,
        CancellationToken cancellationToken);
}

public class ImageHelper(ILogger<ImageHelper> logger) : IImageHelper
{
    public async Task<bool> IsImageFileAsync(Stream stream, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug("Checking whether stream is an image. StreamCanSeek={StreamCanSeek}", stream.CanSeek);
            stream.Position = 0;
            using var image = await SixLabors.ImageSharp.Image.LoadAsync(stream, cancellationToken);
            logger.LogInformation("Image stream validation succeeded. Width={Width}, Height={Height}", image.Width, image.Height);
            return true;
        }
        catch (UnknownImageFormatException)
        {
            logger.LogDebug("Image stream validation failed because the format is unknown");
            return false;
        }
    }

    public async Task<(bool IsImage, int Width, int Height)> GetImageWidthHeightAsync(Stream stream, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug("Reading image dimensions from stream. StreamCanSeek={StreamCanSeek}", stream.CanSeek);
            stream.Position = 0;
            using var image = await SixLabors.ImageSharp.Image.LoadAsync(stream, cancellationToken);
            logger.LogInformation("Image dimensions read successfully. Width={Width}, Height={Height}", image.Width, image.Height);
            return (true, image.Width, image.Height);
        }
        catch (UnknownImageFormatException)
        {
            logger.LogDebug("Image dimension read failed because the format is unknown");
            return (false, 0, 0);
        }
    }

    public async Task<(Stream ThumbnailStream, int Width, int Height, string ContentType)> CreateThumbnailAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Creating image thumbnail. StreamCanSeek={StreamCanSeek}", stream.CanSeek);
        stream.Position = 0;
        using var image = await SixLabors.ImageSharp.Image.LoadAsync(stream, cancellationToken);
        using var thumbnailImage = image.Clone(ctx => ctx.Resize(new ResizeOptions { Size = new Size(200, 200), Mode = ResizeMode.Max }));
        var thumbnailStream = new MemoryStream();
        await thumbnailImage.SaveAsPngAsync(thumbnailStream, cancellationToken);
        logger.LogInformation("Created image thumbnail successfully. Width={Width}, Height={Height}", thumbnailImage.Width, thumbnailImage.Height);
        return (thumbnailStream, thumbnailImage.Width, thumbnailImage.Height, "image/png");
    }
}
