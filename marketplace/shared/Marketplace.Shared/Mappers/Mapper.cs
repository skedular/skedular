using Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value;
using Enterprise.Shared;

namespace Marketplace.Shared.Mappers;

public interface IMapper
{
    Product MapTo(Models.Product src);
}

public class Mapper : IMapper
{
    public Product MapTo(Models.Product src) =>
        new()
        {
            Id = src.Id,
            OrganizationId = src.Organization.Id,
            ProductVersion = MapTo(src.ProductVersions.OrderByDescending(item => item.CreatedAt).First())
        };

    public ProductVersion MapTo(Models.ProductVersion src)
    {
        var productVersion = new ProductVersion
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Name.ToSafeString(),
            Price = src.Price.ToRoundedPrice().ToSafeString(),
            PriceUnit = src.PriceUnit.ToString(),
            Currency = src.Currency.ToString(),
            MinDurationMinutes = src.MinDurationMinutes ?? -1,
            MaxDurationMinutes = src.MaxDurationMinutes ?? -1,
            BookAllLocationResources = src.BookAllLocationResources,
            RecurrenceIntervalDays = src.RecurrenceIntervalDays,
            ForceContinuousSlots = src.ForceContinuousSlots,
            MaxSpreadDays = src.MaxSpreadDays ?? -1
        };

        productVersion.ProductTagIds.AddRange(src.ProductTags.Select(item => item.Id));
        productVersion.LocationTagIds.AddRange(src.LocationTags.Select(item => item.Id));

        return productVersion;
    }
}
