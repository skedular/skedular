using Api.Shared.Services;
using Api.Shared.Services.Models;
using Organization.Api.Models;
using Organization.Shared.Database.Entities;
using ApiServiceConstants = Api.Shared.Services.Constants;
using OrganizationPhysicalAddress = Organization.Shared.Models.OrganizationPhysicalAddress;

namespace Organization.Api.Mappers;

public interface IOrganizationPatchMapper
{
    void Validate(OrganizationPatchRequest request);

    bool ApplyTo(
        OrganizationPatchRequest request,
        Shared.Database.Entities.Organization organization,
        IReadOnlyList<IndustrySubCategory> industrySubCategories);
}

public class OrganizationPatchMapper : IOrganizationPatchMapper
{
    private static readonly IReadOnlySet<OrganizationPatchField> s_supportedPatchFields = Enum.GetValues<OrganizationPatchField>().ToHashSet();

    public void Validate(OrganizationPatchRequest request)
    {
        if (request.FieldsToUpdate.Count == 0)
        {
            throw new ArgumentException("Choose at least one organisation field to update.", nameof(request));
        }

        foreach (var field in request.FieldsToUpdate)
        {
            if (!s_supportedPatchFields.Contains(field))
            {
                throw new ArgumentOutOfRangeException(nameof(request), field, "This organisation patch field is not supported.");
            }
        }

        if (request.FieldsToUpdate.Contains(OrganizationPatchField.Name))
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Organisation name is required.", nameof(request));
            }

            if (request.Name.Length > ApiServiceConstants.MaxOrganizationNameLength)
            {
                throw new ArgumentException(
                    $"Organisation name must be {ApiServiceConstants.MaxOrganizationNameLength} characters or fewer.",
                    nameof(request));
            }
        }

        ValidateMaxLength(
            request.FieldsToUpdate,
            OrganizationPatchField.CustomDomain,
            request.CustomDomain,
            ApiServiceConstants.MaxOrganizationCustomDomainLength,
            "Organisation custom domain");

        ValidateMaxLength(
            request.FieldsToUpdate,
            OrganizationPatchField.Website,
            request.Website,
            ApiServiceConstants.MaxUrlLength,
            "Organisation website");

        ValidateMaxLength(
            request.FieldsToUpdate,
            OrganizationPatchField.LogoUrl,
            request.LogoUrl,
            ApiServiceConstants.MaxUrlLength,
            "Organisation logo URL");

        ValidateMaxLength(
            request.FieldsToUpdate,
            OrganizationPatchField.CustomerFacingTermsAndConditionsUrl,
            request.CustomerFacingTermsAndConditionsUrl,
            ApiServiceConstants.MaxUrlLength,
            "Organisation terms and conditions URL");

        ValidateMaxLength(
            request.FieldsToUpdate,
            OrganizationPatchField.ContactEmail,
            request.ContactEmail,
            ApiServiceConstants.MaxEmailLength,
            "Organisation contact email");

        ValidateMaxLength(
            request.FieldsToUpdate,
            OrganizationPatchField.ContactPhone,
            request.ContactPhone,
            ApiServiceConstants.MaxPhoneNumberLength,
            "Organisation contact phone");

        if (request.FieldsToUpdate.Contains(OrganizationPatchField.BillingCycle) && request.BillingCycle is null)
        {
            throw new ArgumentException("Organisation billing cycle is required.", nameof(request));
        }

        if (request.FieldsToUpdate.Contains(OrganizationPatchField.InvoiceDueInDays) && request.InvoiceDueInDays is null)
        {
            throw new ArgumentException("Organisation invoice due days is required.", nameof(request));
        }

        if (request.FieldsToUpdate.Contains(OrganizationPatchField.InvoiceDueInDays) && request.InvoiceDueInDays is < 1 or > 999)
        {
            throw new InvoiceDueInDaysMustBeBetween1And999();
        }

        if (request.FieldsToUpdate.Contains(OrganizationPatchField.PhysicalAddress) && request.PhysicalAddress is null)
        {
            throw new ArgumentException("Organisation physical address is required.", nameof(request));
        }
    }

    public bool ApplyTo(
        OrganizationPatchRequest request,
        Shared.Database.Entities.Organization organization,
        IReadOnlyList<IndustrySubCategory> industrySubCategories)
    {
        var changed = false;
        foreach (var field in request.FieldsToUpdate)
        {
            changed = field switch
            {
                OrganizationPatchField.Name => ApplyNamePatch(request.Name, organization) || changed,
                OrganizationPatchField.CustomDomain => ApplyCustomDomainPatch(request.CustomDomain, organization) || changed,
                OrganizationPatchField.Website => ApplyStringPatch(request.Website, organization.Website, value => organization.Website = value) ||
                                                  changed,
                OrganizationPatchField.LogoUrl => ApplyStringPatch(request.LogoUrl, organization.LogoUrl, value => organization.LogoUrl = value) ||
                                                  changed,
                OrganizationPatchField.CustomerFacingTermsAndConditionsUrl => ApplyStringPatch(
                    request.CustomerFacingTermsAndConditionsUrl,
                    organization.CustomerFacingTermsAndConditionsUrl,
                    value => organization.CustomerFacingTermsAndConditionsUrl = value) || changed,
                OrganizationPatchField.BillingCycle => ApplyBillingCyclePatch(request.BillingCycle!.Value, organization) || changed,
                OrganizationPatchField.InvoiceDueInDays => ApplyIntPatch(request.InvoiceDueInDays!.Value, organization.InvoiceDueInDays,
                    value => organization.InvoiceDueInDays = value) || changed,
                OrganizationPatchField.ContactEmail => ApplyStringPatch(request.ContactEmail, organization.ContactEmail,
                    value => organization.ContactEmail = value) || changed,
                OrganizationPatchField.ContactPhone => ApplyStringPatch(request.ContactPhone, organization.ContactPhone,
                    value => organization.ContactPhone = value) || changed,
                OrganizationPatchField.RefundNotificationEmails => ApplyStringListPatch(
                    request.RefundNotificationEmails,
                    organization.RefundNotificationEmails,
                    value => organization.RefundNotificationEmails = value.ToList()) || changed,
                OrganizationPatchField.IndustrySubCategories => ApplyIndustrySubCategoriesPatch(industrySubCategories, organization) || changed,
                OrganizationPatchField.FeatureImages => ApplyFeatureImagesPatch(request.FeatureImages, organization) || changed,
                OrganizationPatchField.MarketplaceListingMetadata => ApplyMarketplaceListingMetadataPatch(request.MarketplaceListingMetadata,
                    organization) || changed,
                OrganizationPatchField.PhysicalAddress => ApplyPhysicalAddressPatch(request.PhysicalAddress!, organization) || changed,
                _ => throw new ArgumentOutOfRangeException(nameof(request), $"This organisation patch field is not supported: {field}."),
            };
        }

        return changed;
    }

    private static bool ApplyNamePatch(string? name, Shared.Database.Entities.Organization organization)
    {
        if (organization.Name == name)
        {
            return false;
        }

        organization.Name = name!;
        return true;
    }

    private static bool ApplyCustomDomainPatch(string? customDomain, Shared.Database.Entities.Organization organization)
    {
        var normalizedCustomDomain = string.IsNullOrWhiteSpace(customDomain) ? organization.CustomDomain : customDomain.ToLowerInvariant();
        return ApplyStringPatch(normalizedCustomDomain, organization.CustomDomain, value => organization.CustomDomain = value);
    }

    private static bool ApplyBillingCyclePatch(OrganizationBillingCycle billingCycle, Shared.Database.Entities.Organization organization)
    {
        var entityBillingCycle = billingCycle.ToOrganizationBillingCycle();
        if (organization.BillingCycle == entityBillingCycle)
        {
            return false;
        }

        organization.BillingCycle = entityBillingCycle;
        return true;
    }

    private static bool ApplyIndustrySubCategoriesPatch(
        IReadOnlyList<IndustrySubCategory> industrySubCategories,
        Shared.Database.Entities.Organization organization)
    {
        var currentIds = organization.IndustrySubCategories.Select(item => item.Id).Order().ToList();
        var nextIds = industrySubCategories.Select(item => item.Id).Order().ToList();
        if (currentIds.SequenceEqual(nextIds))
        {
            return false;
        }

        organization.IndustrySubCategories = industrySubCategories.ToList();
        return true;
    }

    private static bool ApplyFeatureImagesPatch(IReadOnlyList<CdnImageFile> featureImages, Shared.Database.Entities.Organization organization)
    {
        if ((organization.FeatureImages ?? []).SequenceEqual(featureImages))
        {
            return false;
        }

        organization.FeatureImages = featureImages.ToList();
        return true;
    }

    private static bool ApplyMarketplaceListingMetadataPatch(ListingMetadata? marketplaceListingMetadata,
        Shared.Database.Entities.Organization organization)
    {
        var nextMarketplaceListingMetadata = marketplaceListingMetadata ?? ListingMetadata.Empty;
        if (organization.MarketplaceListingMetadata == nextMarketplaceListingMetadata)
        {
            return false;
        }

        organization.MarketplaceListingMetadata = nextMarketplaceListingMetadata;
        return true;
    }

    private static bool ApplyPhysicalAddressPatch(OrganizationPhysicalAddress physicalAddress, Shared.Database.Entities.Organization organization)
    {
        var currentPhysicalAddress = organization.PhysicalAddress ?? throw new OrganizationPhysicalAddressNotFound();
        if (currentPhysicalAddress.OsmType == physicalAddress.OsmType &&
            currentPhysicalAddress.OsmId == physicalAddress.OsmId &&
            currentPhysicalAddress.PlaceId == physicalAddress.PlaceId &&
            Equals(currentPhysicalAddress.Coordinates, physicalAddress.Coordinates) &&
            currentPhysicalAddress.FormattedAddress == physicalAddress.FormattedAddress &&
            currentPhysicalAddress.AddressLine1 == physicalAddress.AddressLine1 &&
            currentPhysicalAddress.AddressLine2 == physicalAddress.AddressLine2 &&
            currentPhysicalAddress.Suburb == physicalAddress.Suburb &&
            currentPhysicalAddress.City == physicalAddress.City &&
            currentPhysicalAddress.Province == physicalAddress.Province &&
            currentPhysicalAddress.Zipcode == physicalAddress.Zipcode &&
            currentPhysicalAddress.Country == physicalAddress.Country &&
            currentPhysicalAddress.CountryCode == physicalAddress.CountryCode)
        {
            return false;
        }

        currentPhysicalAddress.OsmType = physicalAddress.OsmType;
        currentPhysicalAddress.OsmId = physicalAddress.OsmId;
        currentPhysicalAddress.PlaceId = physicalAddress.PlaceId;
        currentPhysicalAddress.Coordinates = physicalAddress.Coordinates;
        currentPhysicalAddress.FormattedAddress = physicalAddress.FormattedAddress;
        currentPhysicalAddress.AddressLine1 = physicalAddress.AddressLine1;
        currentPhysicalAddress.AddressLine2 = physicalAddress.AddressLine2;
        currentPhysicalAddress.Suburb = physicalAddress.Suburb;
        currentPhysicalAddress.City = physicalAddress.City;
        currentPhysicalAddress.Province = physicalAddress.Province;
        currentPhysicalAddress.Zipcode = physicalAddress.Zipcode;
        currentPhysicalAddress.Country = physicalAddress.Country;
        currentPhysicalAddress.CountryCode = physicalAddress.CountryCode;
        return true;
    }

    private static bool ApplyStringPatch(string? nextValue, string? currentValue, Action<string?> apply)
    {
        if (currentValue == nextValue)
        {
            return false;
        }

        apply(nextValue);
        return true;
    }

    private static bool ApplyIntPatch(int nextValue, int currentValue, Action<int> apply)
    {
        if (currentValue == nextValue)
        {
            return false;
        }

        apply(nextValue);
        return true;
    }

    private static bool ApplyStringListPatch(IReadOnlyList<string> nextValue, IEnumerable<string>? currentValue, Action<IReadOnlyList<string>> apply)
    {
        if ((currentValue ?? []).SequenceEqual(nextValue))
        {
            return false;
        }

        apply(nextValue);
        return true;
    }

    private static void ValidateMaxLength(
        IReadOnlySet<OrganizationPatchField> fieldsToUpdate,
        OrganizationPatchField field,
        string? value,
        int maxLength,
        string fieldName)
    {
        if (fieldsToUpdate.Contains(field) && value?.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} must be {maxLength} characters or fewer.");
        }
    }
}
