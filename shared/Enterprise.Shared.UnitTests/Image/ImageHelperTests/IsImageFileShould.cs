using Enterprise.Shared.Image;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Enterprise.Shared.UnitTests.Image.ImageHelperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class IsImageFileShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_true_for_valid_image(CancellationToken cancellationToken)
    {
        var sut = new ImageHelper();

        // Create a minimal valid PNG in memory (8 bytes header + IHDR)
        var pngBytes = CreateMinimalPng();
        using var stream = new MemoryStream(pngBytes);

        var result = await sut.IsImageFileAsync(stream, cancellationToken);

        result.ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_false_for_non_image_stream(CancellationToken cancellationToken)
    {
        var sut = new ImageHelper();
        using var stream = new MemoryStream("not an image"u8.ToArray());

        var result = await sut.IsImageFileAsync(stream, cancellationToken);

        result.ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_width_height_for_valid_image(CancellationToken cancellationToken)
    {
        var sut = new ImageHelper();
        var pngBytes = CreateMinimalPng();
        using var stream = new MemoryStream(pngBytes);

        var (isImage, width, height) = await sut.GetImageWidthHeightAsync(stream, cancellationToken);

        isImage.ShouldBeTrue();
        width.ShouldBeGreaterThan(0);
        height.ShouldBeGreaterThan(0);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_false_for_non_image_on_get_dimensions(CancellationToken cancellationToken)
    {
        var sut = new ImageHelper();
        using var stream = new MemoryStream("not an image"u8.ToArray());

        var (isImage, width, height) = await sut.GetImageWidthHeightAsync(stream, cancellationToken);

        isImage.ShouldBeFalse();
        width.ShouldBe(0);
        height.ShouldBe(0);
    }

    private static byte[] CreateMinimalPng()
    {
        var ms = new MemoryStream();
        using (var image = new Image<Rgba32>(1, 1))
        {
            image.Save(ms, new PngEncoder());
        }

        return ms.ToArray();
    }
}
