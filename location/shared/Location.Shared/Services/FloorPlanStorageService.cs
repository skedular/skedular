using Enterprise.Shared.Cdn;
using Enterprise.Shared.Random;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Location.Shared.Services;

public interface IFloorPlanStorageService
{
    Task<(string imageUrl, string? thumbnailUrl, int width, int height)> SaveFloorPlanAsync(
        byte[] imageContent,
        string fileName,
        string contentType,
        int thumbnailWidth = 200,
        int thumbnailHeight = 200);

    Task DeleteFloorPlanAsync(string imageUrl, string? thumbnailUrl);
}

public class FloorPlanStorageService(
    ICdnService cdnService,
    IRandomHelper randomHelper,
    ILogger<FloorPlanStorageService> logger)
    : IFloorPlanStorageService
{
    private const string FloorPlanPrefix = "floor-plans";
    private const string ThumbnailPrefix = "floor-plans/thumbnails";

    public async Task<(string imageUrl, string? thumbnailUrl, int width, int height)> SaveFloorPlanAsync(
        byte[] imageContent,
        string fileName,
        string contentType,
        int thumbnailWidth = 200,
        int thumbnailHeight = 200)
    {
        using var image = Image.Load(imageContent);
        var width = image.Width;
        var height = image.Height;

        var uniqueId = randomHelper.Generate();
        var extension = Path.GetExtension(fileName);
        var uniqueFileName = $"{FloorPlanPrefix}/{uniqueId}{extension}";

        using var imageStream = new MemoryStream(imageContent);
        var (_, imageUrl) = await cdnService.UploadAsync(imageStream, contentType, uniqueFileName, CancellationToken.None);

        string? thumbnailUrl = null;

        try
        {
            using var thumbnailStream = new MemoryStream();

            using var thumbnailImage = image.Clone(ctx => ctx
                .Resize(new ResizeOptions { Size = new Size(thumbnailWidth, thumbnailHeight), Mode = ResizeMode.Max }));

            await thumbnailImage.SaveAsPngAsync(thumbnailStream);
            thumbnailStream.Position = 0;

            var thumbnailFileName = $"{ThumbnailPrefix}/{uniqueId}_thumb.png";
            var (_, thumbUrl) = await cdnService.UploadAsync(thumbnailStream, "image/png", thumbnailFileName, CancellationToken.None);
            thumbnailUrl = thumbUrl.ToString();

            logger.LogInformation(
                "Floor plan saved successfully to CDN. Original: {ImageUrl}, Thumbnail: {ThumbnailUrl}, Dimensions: {Width}x{Height}",
                imageUrl, thumbnailUrl, width, height);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create thumbnail for floor plan: {FileName}", fileName);
        }

        return (imageUrl.ToString(), thumbnailUrl, width, height);
    }

    public Task DeleteFloorPlanAsync(string imageUrl, string? thumbnailUrl)
    {
        logger.LogInformation("Floor plan deletion requested but not implemented for CDN: {ImageUrl}", imageUrl);
        return Task.CompletedTask;
    }
}
