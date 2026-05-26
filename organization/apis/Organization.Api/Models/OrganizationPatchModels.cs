using Api.Shared.Services.Models;
using HotChocolate;
using Organization.Shared.Models;

namespace Organization.Api.Models;

[GraphQLName("OrganizationPatchField")]
public enum OrganizationPatchField
{
    Name,
    CustomDomain,
    Website,
    LogoUrl,
    CustomerFacingTermsAndConditionsUrl,
    BillingCycle,
    InvoiceDueInDays,
    ContactEmail,
    ContactPhone,
    RefundNotificationEmails,
    IndustrySubCategories,
    FeatureImages,
    MarketplaceListingMetadata,
    PhysicalAddress
}

public record OrganizationPatchRequest(
    string? Id,
    string? CustomDomain,
    IReadOnlySet<OrganizationPatchField> FieldsToUpdate,
    string? Name,
    string? Website = null,
    string? LogoUrl = null,
    string? CustomerFacingTermsAndConditionsUrl = null,
    OrganizationBillingCycle? BillingCycle = null,
    int? InvoiceDueInDays = null,
    string? ContactEmail = null,
    string? ContactPhone = null,
    IReadOnlyList<string>? RefundNotificationEmails = null,
    IReadOnlyList<string>? IndustrySubCategoryIds = null,
    IReadOnlyList<CdnImageFile>? FeatureImages = null,
    ListingMetadata? MarketplaceListingMetadata = null,
    OrganizationPhysicalAddress? PhysicalAddress = null)
{
    public IReadOnlyList<string> RefundNotificationEmails { get; init; } = RefundNotificationEmails ?? [];
    public IReadOnlyList<string> IndustrySubCategoryIds { get; init; } = IndustrySubCategoryIds ?? [];
    public IReadOnlyList<CdnImageFile> FeatureImages { get; init; } = FeatureImages ?? [];
}

public record OrganizationPatchResult(Shared.Models.Organization Organization);
