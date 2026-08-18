using Enterprise.Shared.Image;
using SkiaSharp;

namespace Enterprise.Shared.UnitTests.Image.ImageHelperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class IsImageFileShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_true_for_valid_image(ImageHelper sut)
    {
        var pngBytes = CreateMinimalPng();
        using var stream = new MemoryStream(pngBytes);

        var result = sut.IsImageFile(stream);

        result.ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_false_for_non_image_stream(ImageHelper sut)
    {
        using var stream = new MemoryStream([.. "not an image"u8]);

        var result = sut.IsImageFile(stream);

        result.ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_width_height_for_valid_image(ImageHelper sut)
    {
        var pngBytes = CreateMinimalPng();
        using var stream = new MemoryStream(pngBytes);

        var (isImage, width, height) = sut.GetImageWidthHeight(stream);

        isImage.ShouldBeTrue();
        width.ShouldBeGreaterThan(0);
        height.ShouldBeGreaterThan(0);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_false_for_non_image_on_get_dimensions(ImageHelper sut)
    {
        using var stream = new MemoryStream([.. "not an image"u8]);

        var (isImage, width, height) = sut.GetImageWidthHeight(stream);

        isImage.ShouldBeFalse();
        width.ShouldBe(0);
        height.ShouldBe(0);
    }

    private static byte[] CreateMinimalPng()
    {
        using var bitmap = new SKBitmap(1, 1);
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }
}
