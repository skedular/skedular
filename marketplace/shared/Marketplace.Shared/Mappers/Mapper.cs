using Api.Shared.Services.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using CdnFile = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.CdnFile;
using CdnImageFile = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.CdnImageFile;
using Product = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Product;
using ProductVersion = Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductVersion;

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
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            Inactive = src.Inactive,
            OrganizationId = src.Organization.Id,
            LatestProductVersion = MapTo(src.ProductVersions.OrderByDescending(item => item.CreatedAt).First())
        };

    private ProductVersion MapTo(Models.ProductVersion src)
    {
        var productVersion = new ProductVersion
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Name.ToSafeString(),
            Price = src.Price.ToRoundedPrice(),
            PriceUnit = src.PriceUnit.ToPriceUnit(),
            Currency = src.Currency.ToCurrency(),
            MinDurationMinutes = src.MinDurationMinutes ?? -1,
            MaxDurationMinutes = src.MaxDurationMinutes ?? -1,
            BookAllLocationResources = src.BookAllLocationResources,
            RecurrenceWindowDays = src.RecurrenceWindowDays,
            RequireConsecutiveDays = src.RequireConsecutiveDays,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays ?? -1,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook,
            PrimaryFeatureImage = MapTo(src.PrimaryFeatureImage),
            MaxAllowedResourcesLockTimePaidByCard = src.MaxAllowedResourcesLockTimePaidByCard,
            MaxAllowedResourcesLockTimePaidThroughBankAccount = src.MaxAllowedResourcesLockTimePaidThroughBankAccount
        };

        productVersion.ProductTagIds.AddRange(src.ProductTags.Select(item => item.Id));
        productVersion.LocationTagIds.AddRange(src.LocationTags.Select(item => item.Id));

        return productVersion;
    }

    private static CdnImageFile? MapTo(Api.Shared.Services.Models.CdnImageFile? src) =>
        src is null ? null : new CdnImageFile { Original = MapTo(src.Original), Thumbnail = MapTo(src.Thumbnail) };

    private static CdnFile? MapTo(Api.Shared.Services.Models.CdnFile? src) =>
        src is null ? null : new CdnFile { Url = src.Url.ToSafeString(), Height = src.Height.ToNullInt(), Width = src.Width.ToNullInt() };
}
