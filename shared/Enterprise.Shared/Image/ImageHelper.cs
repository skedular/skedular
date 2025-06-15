using SixLabors.ImageSharp;

namespace Enterprise.Shared.Image;

public interface IImageHelper
{
    Task<bool> IsImageFileAsync(Stream stream, CancellationToken cancellationToken);
    Task<(bool IsImage, int Width, int Height)> GetImageWidthHeightAsync(Stream stream, CancellationToken cancellationToken);
}

public class ImageHelper : IImageHelper
{
    public async Task<bool> IsImageFileAsync(Stream stream, CancellationToken cancellationToken)
    {
        try
        {
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
            using var image = await SixLabors.ImageSharp.Image.LoadAsync(stream, cancellationToken);
            return (true, image.Width, image.Height);
        }
        catch (UnknownImageFormatException)
        {
            return (false, 0, 0);
        }
    }
}
