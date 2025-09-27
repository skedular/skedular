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

public class ImageHelper : IImageHelper
{
    public async Task<bool> IsImageFileAsync(Stream stream, CancellationToken cancellationToken)
    {
        try
        {
            stream.Position = 0;
            using var image = await SixLabors.ImageSharp.Image.LoadAsync(stream, cancellationToken);
            return true;
        }
        catch (UnknownImageFormatException)
        {
            return false;
        }
    }

    public async Task<(bool IsImage, int Width, int Height)> GetImageWidthHeightAsync(Stream stream, CancellationToken cancellationToken)
    {
        try
        {
            stream.Position = 0;
            using var image = await SixLabors.ImageSharp.Image.LoadAsync(stream, cancellationToken);
            return (true, image.Width, image.Height);
        }
        catch (UnknownImageFormatException)
        {
            return (false, 0, 0);
        }
    }

    public async Task<(Stream ThumbnailStream, int Width, int Height, string ContentType)> CreateThumbnailAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        stream.Position = 0;
        using var image = await SixLabors.ImageSharp.Image.LoadAsync(stream, cancellationToken);
        using var thumbnailImage = image.Clone(ctx => ctx.Resize(new ResizeOptions { Size = new Size(200, 200), Mode = ResizeMode.Max }));
        var thumbnailStream = new MemoryStream();
        await thumbnailImage.SaveAsPngAsync(thumbnailStream, cancellationToken);
        return (thumbnailStream, thumbnailImage.Width, thumbnailImage.Height, "image/png");
    }
}
