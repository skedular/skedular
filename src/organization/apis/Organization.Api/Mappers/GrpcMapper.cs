using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Api.Shared.Grpc.Skedular.Organization.Core.V1;
using Api.Shared.Grpc.Skedular.Organization.Tags.V1;
using Api.Shared.Grpc.Skedular.Organization.Zones.V1;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using HotChocolate.Types.Pagination;
using Organization.Api.Models;
using Organization.Shared.Models;
using AddZoneInput = Api.Shared.Grpc.Skedular.Organization.Zones.V1.AddZoneInput;
using BankAccount = Api.Shared.Grpc.Skedular.Organization.Billing.V1.BankAccount;
using CdnFile = Api.Shared.Services.Models.CdnFile;
using CdnImageFile = Api.Shared.Services.Models.CdnImageFile;
using Coordinates = Api.Shared.Grpc.Skedular.Organization.Core.V1.Coordinates;
using Currency = Api.Shared.Services.Models.Currency;
using Customer = Organization.Shared.Models.Customer;
using IndustrySubCategory = Organization.Shared.Models.IndustrySubCategory;
using ListingMetadata = Api.Shared.Services.Models.ListingMetadata;
using OrganizationMember = Organization.Shared.Models.OrganizationMember;
using OrganizationMemberStatus = Api.Shared.Services.Models.OrganizationMemberStatus;
using Tag = Organization.Shared.Models.Tag;
using Member = Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationMember;
using Offering = Api.Shared.Grpc.Skedular.Organization.Core.V1.Offering;
using OrganizationBillingCycle = Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationBillingCycle;
using OrganizationMemberRole = Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationMemberRole;
using OrganizationTaxDetails = Organization.Shared.Models.OrganizationTaxDetails;
using OrganizationXeroConnection = Organization.Shared.Models.OrganizationXeroConnection;
using OrganizationType = Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationType;
using TermsOfUse = Api.Shared.Grpc.Skedular.Organization.Core.V1.TermsOfUse;


namespace Organization.Api.Mappers;

public interface IGrpcMapper
{
    TermsOfUse MapToGrpcResponse(Shared.Models.TermsOfUse src);
    Shared.Models.Organization MapTo(Admin_AddInput src);
    global::Api.Shared.Grpc.Skedular.Organization.Core.V1.Organization MapToGrpcResponse(Shared.Models.Organization src);
    XeroConnection? MapToGrpcResponse(OrganizationXeroConnection? src);
    OrganizationMember MapTo(Admin_AddMemberInput src);
    MemberEdge MapToGrpcResponse(Edge<OrganizationMember> src);
    global::Api.Shared.Grpc.Skedular.Organization.Core.V1.Tag MapToGrpcResponseTag(Tag src);
    TagEdge MapToGrpcResponseTag(Edge<Tag> src);
    CustomTag MapToGrpcResponseCustomTag(Tag src);
    CustomTagEdge MapToGrpcResponseCustomTag(Edge<Tag> src);
    Tag MapTo(AddCustomTagInput src);
    OrganizationTagPatchRequest MapTo(UpdateTagInput src, OrganizationTagType type);
    Zone MapToGrpcResponseZone(Tag src);
    ZoneEdge MapToGrpcResponseZone(Edge<Tag> src);
    Tag MapTo(AddZoneInput src);
    OrganizationTagPatchRequest MapTo(UpdateZoneInput src);
    ProductTag MapToGrpcResponseProductTag(Tag src);
    ProductTagEdge MapToGrpcResponseProductTag(Edge<Tag> src);
    Tag MapTo(AddProductTagInput src);
    BillingDetails MapToGrpcResponse(OrganizationBillingDetails? src);
    StripeConnectAccountEdge MapToGrpcResponse(Edge<OrganizationStripeConnectAccount> src);
    BankAccountEdge MapToGrpcResponse(Edge<OrganizationBankAccount> src);
    Tag MapTo(AddTagInput src);
    OrganizationBillingDetails MapTo(AddBillingDetailsInput src);
    OrganizationBillingDetailsPatchRequest MapTo(UpdateBillingDetailsInput src);
}

public class GrpcMapper : IGrpcMapper
{
    public TermsOfUse MapToGrpcResponse(Shared.Models.TermsOfUse src) => new()
    {
        Id = src.Id,
        Terms = src.Terms,
    };

    public Shared.Models.Organization MapTo(Admin_AddInput src) =>
        new()
        {
            Id = src.Id,
            CustomDomain = src.CustomDomain,
            Name = src.Name,
            MarketplaceListingMetadata = MapTo(src.MarketplaceListingMetadata),
            Website = src.Website,
            CustomerFacingTermsAndConditionsUrl = src.CustomerFacingTermsAndConditionsUrl.ToSafeString(),
            Type = src.Type switch
            {
                OrganizationType.Private => global::Api.Shared.Services.Models.OrganizationType.Private,
                OrganizationType.Marketplace => global::Api.Shared.Services.Models.OrganizationType.Marketplace,
                OrganizationType.Host => global::Api.Shared.Services.Models.OrganizationType.Host,
                _ => throw new ArgumentOutOfRangeException(nameof(src.Type), src.Type,
                    $"Unexpected value for {nameof(src.Type)}: {src.Type}. Update enum mapping or caller input."),
            },
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            TermsOfUse = string.IsNullOrWhiteSpace(src.TermsOfUseId)
                ? null
                : new Shared.Models.TermsOfUse
                {
                    Id = src.TermsOfUseId,
                },
            LogoUrl = src.LogoUrl,
            IndustrySubCategories =
            [
                .. src.IndustrySubCategoryIds.Select(item => new IndustrySubCategory
                {
                    Id = item,
                }),
            ],
            FeatureImages = [.. MapTo(src.FeatureImages)],
            BillingCycle = src.BillingCycle switch
            {
                OrganizationBillingCycle.Weekly => global::Api.Shared.Services.Models.OrganizationBillingCycle.Weekly,
                OrganizationBillingCycle.Fortnightly => global::Api.Shared.Services.Models.OrganizationBillingCycle.Fortnightly,
                OrganizationBillingCycle.Monthly => global::Api.Shared.Services.Models.OrganizationBillingCycle.Monthly,
                _ => throw new ArgumentOutOfRangeException(nameof(src.BillingCycle), src.BillingCycle,
                    $"Unexpected value for {nameof(src.BillingCycle)}: {src.BillingCycle}. Update enum mapping or caller input."),
            },
            InvoiceDueInDays = src.InvoiceDueInDays,
        };


    public global::Api.Shared.Grpc.Skedular.Organization.Core.V1.Organization MapToGrpcResponse(Shared.Models.Organization src)
    {
        var organizationOffering = src.OrganizationOfferings.Where(item => !item.DeletedAt.HasValue)
            .OrderByDescending(item => item.End).First();
        var offering = new Offering
        {
            Id = organizationOffering.Id,
            OrganizationId = src.Id,
            Code = organizationOffering.Code.ToOfferingCode(),
            Start = organizationOffering.Start.ToTimestamp(),
            End = organizationOffering.End.ToTimestamp(),
            AutoRenew = organizationOffering.AutoRenew,
            Currency = organizationOffering.Currency switch
            {
                Currency.Nzd => global::Api.Shared.Grpc.Skedular.Organization.Core.V1.Currency.Nzd,
                Currency.Usd => global::Api.Shared.Grpc.Skedular.Organization.Core.V1.Currency.Usd,
                _ => throw new ArgumentOutOfRangeException(nameof(organizationOffering.Currency), organizationOffering.Currency,
                    $"Unexpected value for {nameof(organizationOffering.Currency)}: {organizationOffering.Currency}. Update enum mapping or caller input."),
            },
        };
        if (organizationOffering.UnitPrice.HasValue)
        {
            offering.UnitPrice = organizationOffering.UnitPrice.Value;
        }

        if (organizationOffering.FixedPrice.HasValue)
        {
            offering.FixedPrice = organizationOffering.FixedPrice.Value;
        }

        if (organizationOffering.PurchasedUserCapacity.HasValue)
        {
            offering.PurchasedUserCapacity = organizationOffering.PurchasedUserCapacity.Value;
        }

        if (organizationOffering.PurchasedLocationCapacity.HasValue)
        {
            offering.PurchasedLocationCapacity = organizationOffering.PurchasedLocationCapacity.Value;
        }

        if (organizationOffering.PurchasedTeamCapacity.HasValue)
        {
            offering.PurchasedTeamCapacity = organizationOffering.PurchasedTeamCapacity.Value;
        }

        offering.HostCommissionPercentage = decimal.ToDouble(organizationOffering.HostCommissionPercentage);

        var organization = new global::Api.Shared.Grpc.Skedular.Organization.Core.V1.Organization
        {
            Id = src.Id,
            CustomDomain = src.CustomDomain.ToSafeString(),
            Name = src.Name.ToSafeString(),
            MarketplaceListingMetadata = MapTo(src.MarketplaceListingMetadata),
            Website = src.Website.ToSafeString(),
            CustomerFacingTermsAndConditionsUrl = src.CustomerFacingTermsAndConditionsUrl.ToSafeString(),
            Type = src.Type switch
            {
                global::Api.Shared.Services.Models.OrganizationType.Private => OrganizationType.Private,
                global::Api.Shared.Services.Models.OrganizationType.Marketplace => OrganizationType.Marketplace,
                global::Api.Shared.Services.Models.OrganizationType.Host => OrganizationType.Host,
                _ => throw new ArgumentOutOfRangeException(nameof(src.Type), src.Type,
                    $"Unexpected value for {nameof(src.Type)}: {src.Type}. Update enum mapping or caller input."),
            },
            ContactEmail = src.ContactEmail.ToSafeString(),
            ContactPhone = src.ContactPhone.ToSafeString(),
            IsOwnershipVerified = src.IsOwnershipVerified ?? false,
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            LogoUrl = src.LogoUrl.ToSafeString(),
            Offering = offering,
            HasAttachedPaymentMethod = src.HasAttachedPaymentMethod,
            TaxDetails = MapToGrpcResponse(src.OrganizationTaxDetails),
            PhysicalAddress = MapToGrpcResponse(src.PhysicalAddress),
        };

        organization.Tags.AddRange(MapToGrpcResponse(src.Tags));
        organization.ResourceTypes.AddRange(MapToGrpcResponseResourceType(src.Tags));

        organization.Offering.ActiveCustomerIds.AddRange(
            organizationOffering.OrganizationOfferingActiveMembers.Select(item => item.OrganizationMember.Customer.Id));

        organization.IndustrySubCategories.AddRange(src.IndustrySubCategories.Select(item =>
            new global::Api.Shared.Grpc.Skedular.Organization.Core.V1.IndustrySubCategory
            {
                Id = item.Id,
                Name = item.Name,
                MainCategoryName = item.IndustryMainCategory.Name,
            }));

        organization.Members.AddRange(MapToGrpcResponse(src.OrganizationMembers));
        organization.FeatureImages.AddRange(MapTo(src.FeatureImages));

        return organization;
    }


    public XeroConnection? MapToGrpcResponse(OrganizationXeroConnection? src) =>
        src is null
            ? null
            : new XeroConnection
            {
                Id = src.Id,
                TenantId = src.TenantId,
                TenantName = src.TenantName,
                BillingMode = src.BillingMode.ToOrganizationXeroBillingMode(),
                Scopes = src.Scopes.ToSafeString(),
                IsActive = src.IsActive,
                SendInvoicesViaXero = src.SendInvoicesViaXero,
                AutoReconcilePayments = src.AutoReconcilePayments,
                DefaultSalesAccountCode = src.DefaultSalesAccountCode.ToSafeString(),
                DefaultReceivablesAccountCode = src.DefaultReceivablesAccountCode.ToSafeString(),
                DefaultTrackingCategory1 = src.DefaultTrackingCategory1.ToSafeString(),
                DefaultTrackingCategory2 = src.DefaultTrackingCategory2.ToSafeString(),
                DefaultBrandingThemeId = src.DefaultBrandingThemeId.ToSafeString(),
                DefaultReferencePrefix = src.DefaultReferencePrefix.ToSafeString(),
                AccessTokenExpiresAt = src.AccessTokenExpiresAt?.ToTimestamp(),
                RefreshTokenExpiresAt = src.RefreshTokenExpiresAt?.ToTimestamp(),
                LastSuccessfulSyncAt = src.LastSuccessfulSyncAt?.ToTimestamp(),
                LastError = src.LastError.ToSafeString(),
                AccessTokenEncrypted = src.AccessTokenEncrypted.ToSafeString(),
                RefreshTokenEncrypted = src.RefreshTokenEncrypted.ToSafeString(),
                HasAccessToken = src.HasAccessToken,
                HasRefreshToken = src.HasRefreshToken,
            };


    public OrganizationMember MapTo(Admin_AddMemberInput src) => MapTo(src.Member, new Shared.Models.Organization
    {
        Id = src.Id,
    });

    public MemberEdge MapToGrpcResponse(Edge<OrganizationMember> src) => new()
    {
        Cursor = src.Cursor,
        Node = MapToGrpcResponse(src.Node),
    };

    public global::Api.Shared.Grpc.Skedular.Organization.Core.V1.Tag MapToGrpcResponseTag(Tag src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            TagType = src.Type.ToOrganizationTagType(),
            Color = src.Color.ToSafeString(),
        };


    public TagEdge MapToGrpcResponseTag(Edge<Tag> src) => new()
    {
        Cursor = src.Cursor,
        Node = MapToGrpcResponseTag(src.Node),
    };

    public CustomTag MapToGrpcResponseCustomTag(Tag src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Color = src.Color.ToSafeString(),
        };

    public CustomTagEdge MapToGrpcResponseCustomTag(Edge<Tag> src) => new()
    {
        Cursor = src.Cursor,
        Node = MapToGrpcResponseCustomTag(src.Node),
    };

    public Tag MapTo(AddCustomTagInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Type = OrganizationTagType.Custom,
            Color = src.Color.ToSafeString(),
            Organization = new Shared.Models.Organization
            {
                Id = src.OrganizationId,
            },
        };

    public OrganizationTagPatchRequest MapTo(UpdateTagInput src, OrganizationTagType type) =>
        new(src.Id, type, src.FieldsToUpdate.Select(MapTo).ToHashSet(), src.Name, src.Description, src.Color);

    public Zone MapToGrpcResponseZone(Tag src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Color = src.Color.ToSafeString(),
        };

    public ZoneEdge MapToGrpcResponseZone(Edge<Tag> src) => new()
    {
        Cursor = src.Cursor,
        Node = MapToGrpcResponseZone(src.Node),
    };

    public Tag MapTo(AddZoneInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Type = OrganizationTagType.Zone,
            Color = src.Color.ToSafeString(),
            Organization = new Shared.Models.Organization
            {
                Id = src.OrganizationId,
            },
        };

    public OrganizationTagPatchRequest MapTo(UpdateZoneInput src) =>
        new(src.Id, OrganizationTagType.Zone, src.FieldsToUpdate.Select(MapTo).ToHashSet(), src.Name, src.Description, src.Color);

    public ProductTag MapToGrpcResponseProductTag(Tag src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Color = src.Color.ToSafeString(),
        };

    public ProductTagEdge MapToGrpcResponseProductTag(Edge<Tag> src) => new()
    {
        Cursor = src.Cursor,
        Node = MapToGrpcResponseProductTag(src.Node),
    };

    public Tag MapTo(AddProductTagInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Type = OrganizationTagType.Product,
            Color = src.Color.ToSafeString(),
            Organization = new Shared.Models.Organization
            {
                Id = src.OrganizationId,
            },
        };

    public BillingDetails MapToGrpcResponse(OrganizationBillingDetails? src) =>
        src is null
            ? new BillingDetails
            {
                Id = string.Empty,
            }
            : new BillingDetails
            {
                Id = src.Id,
                CompanyName = src.CompanyName.ToSafeString(),
                Email = src.Email,
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2.ToSafeString(),
                Suburb = src.Suburb.ToSafeString(),
                City = src.City.ToSafeString(),
                Province = src.Province.ToSafeString(),
                Zipcode = src.Zipcode,
                Country = src.Country,
                CountryCode = src.CountryCode.ToSafeString(),
                FormattedAddress = src.ToFormattedAddress(),
                OsmType = src.OsmType.ToSafeString(),
                OsmId = src.OsmId.ToSafeString(),
                PlaceId = src.PlaceId.ToSafeString(),
                Coordinates = src.Coordinates is null
                    ? null
                    : new Coordinates
                    {
                        Longitude = src.Coordinates.X,
                        Latitude = src.Coordinates.Y,
                    },
            };

    public StripeConnectAccountEdge MapToGrpcResponse(Edge<OrganizationStripeConnectAccount> src) =>
        new()
        {
            Cursor = src.Cursor,
            Node = MapToGrpcResponse(src.Node),
        };

    public BankAccountEdge MapToGrpcResponse(Edge<OrganizationBankAccount> src) =>
        new()
        {
            Cursor = src.Cursor,
            Node = MapToGrpcResponse(src.Node),
        };

    public Tag MapTo(AddTagInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Type = src.Type.ToOrganizationTagType(),
            Color = src.Color.ToSafeString(),
            Organization = new Shared.Models.Organization
            {
                Id = src.OrganizationId,
            },
        };

    public OrganizationBillingDetails MapTo(AddBillingDetailsInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            CompanyName = src.CompanyName,
            Email = src.Email,
            AddressLine1 = src.AddressLine1,
            AddressLine2 = src.AddressLine2,
            Suburb = src.Suburb,
            City = src.City,
            Province = src.Province,
            Zipcode = src.Zipcode,
            Country = src.Country,
            CountryCode = src.CountryCode,
            Organization = new Shared.Models.Organization
            {
                Id = src.OrganizationId,
            },
        };

    public OrganizationBillingDetailsPatchRequest MapTo(UpdateBillingDetailsInput src) =>
        new(
            src.OrganizationId,
            src.OrganizationCustomDomain,
            src.FieldsToUpdate.Select(MapTo).ToHashSet(),
            src.CompanyName,
            src.Email,
            null,
            null,
            null,
            null,
            null,
            null,
            src.AddressLine1,
            src.AddressLine2,
            src.Suburb,
            src.City,
            src.Province,
            src.Zipcode,
            src.Country,
            src.CountryCode);

    private static OrganizationTagPatchField MapTo(TagPatchField src) =>
        src switch
        {
            TagPatchField.Name => OrganizationTagPatchField.Name,
            TagPatchField.Description => OrganizationTagPatchField.Description,
            TagPatchField.Color => OrganizationTagPatchField.Color,
            _ => throw new ArgumentOutOfRangeException(nameof(src), src, "This organisation tag gRPC patch field is not supported."),
        };

    private static OrganizationTagPatchField MapTo(ZonePatchField src) =>
        src switch
        {
            ZonePatchField.Name => OrganizationTagPatchField.Name,
            ZonePatchField.Description => OrganizationTagPatchField.Description,
            ZonePatchField.Color => OrganizationTagPatchField.Color,
            _ => throw new ArgumentOutOfRangeException(nameof(src), src, "This organisation zone gRPC patch field is not supported."),
        };

    private static OrganizationBillingDetailsPatchField MapTo(BillingDetailsPatchField src) =>
        src switch
        {
            BillingDetailsPatchField.CompanyName => OrganizationBillingDetailsPatchField.CompanyName,
            BillingDetailsPatchField.Email => OrganizationBillingDetailsPatchField.Email,
            BillingDetailsPatchField.BillingAddress => OrganizationBillingDetailsPatchField.BillingAddress,
            _ => throw new ArgumentOutOfRangeException(nameof(src), src, "This organisation billing details gRPC patch field is not supported."),
        };

    private static IEnumerable<global::Api.Shared.Grpc.Skedular.Organization.Core.V1.Tag> MapToGrpcResponse(IEnumerable<Tag> src) =>
        src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Grpc.Skedular.Organization.Core.V1.Tag MapToGrpcResponse(Tag src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Color = src.Color.ToSafeString(),
        };

    private static IEnumerable<ResourceType> MapToGrpcResponseResourceType(IEnumerable<Tag> src) =>
        src
            .Where(item => OrganizationTagTypeConstants.ResourceTypes.Any(resourceType => resourceType == item.Type))
            .Select(MapToGrpcResponseResourceType);

    private static ResourceType MapToGrpcResponseResourceType(Tag src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Color = src.Color.ToSafeString(),
            TagType = src.Type.ToOrganizationTagType(),
        };

    private static Member MapToGrpcResponse(OrganizationMember src) =>
        new()
        {
            Id = src.Id,
            Role = src.Role switch
            {
                global::Api.Shared.Services.Models.OrganizationMemberRole.Owner => OrganizationMemberRole.Owner,
                global::Api.Shared.Services.Models.OrganizationMemberRole.Administrator => OrganizationMemberRole.Administrator,
                global::Api.Shared.Services.Models.OrganizationMemberRole.Member => OrganizationMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException(nameof(src.Role), src.Role,
                    $"Unexpected value for {nameof(src.Role)}: {src.Role}. Update enum mapping or caller input."),
            },
            Status = src.Status switch
            {
                OrganizationMemberStatus.Active => global::Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationMemberStatus.Active,
                OrganizationMemberStatus.Inactive => global::Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException(nameof(src.Status), src.Status,
                    $"Unexpected value for {nameof(src.Status)}: {src.Status}. Update enum mapping or caller input."),
            },
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone ?? false,
            CustomerId = src.Customer.Id,
        };

    private static IEnumerable<Member> MapToGrpcResponse(IEnumerable<OrganizationMember> src) => src.Select(MapToGrpcResponse);

    private static OrganizationMember MapTo(Member src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            Role = src.Role switch
            {
                OrganizationMemberRole.Owner => global::Api.Shared.Services.Models.OrganizationMemberRole.Owner,
                OrganizationMemberRole.Administrator => global::Api.Shared.Services.Models.OrganizationMemberRole.Administrator,
                OrganizationMemberRole.Member => global::Api.Shared.Services.Models.OrganizationMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            },
            Status = src.Status switch
            {
                global::Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationMemberStatus.Active => OrganizationMemberStatus.Active,
                global::Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationMemberStatus.Inactive => OrganizationMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            },
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone,
            Customer = new Customer
            {
                Id = src.CustomerId,
            },
            Organization = organization,
        };

    private static StripeConnectAccount MapToGrpcResponse(OrganizationStripeConnectAccount src) =>
        new()
        {
            Id = src.Id,
            IsDefault = src.IsDefault,
            StripeAccountId = src.StripeAccountId.ToSafeString(),
            Name = src.Name.ToSafeString(),
            ChargesEnabled = src.ChargesEnabled,
            PayoutsEnabled = src.PayoutsEnabled,
            Type = src.Type.ToSafeString(),
            Country = src.Country.ToSafeString(),
            DefaultCurrency = src.DefaultCurrency.ToSafeString(),
            BusinessType = src.BusinessType.ToSafeString(),
            CompanyName = src.CompanyName.ToSafeString(),
            Url = src.Url.ToSafeString(),
            SupportUrl = src.SupportUrl.ToSafeString(),
            ContactEmail = src.ContactEmail.ToSafeString(),
            ContactPhone = src.ContactPhone.ToSafeString(),
            DetailsSubmitted = src.DetailsSubmitted,
            CapabilitiesTransfers = src.CapabilitiesTransfers.ToSafeString(),
            CapabilitiesCardPayments = src.CapabilitiesCardPayments.ToSafeString(),
            OnboardingUrl = src.OnboardingUrl.ToSafeString(),
            OnboardingCompleted = src.IsOnboardingCompleted(),
        };

    private static BankAccount MapToGrpcResponse(OrganizationBankAccount src) =>
        new()
        {
            Id = src.Id,
            IsDefault = src.IsDefault,
            Name = src.Name.ToSafeString(),
            BankName = src.BankName.ToSafeString(),
            AccountHolderName = src.AccountHolderName.ToSafeString(),
            AccountNumber = src.AccountNumber.ToSafeString(),
            Country = src.Country.ToSafeString(),
        };

    private static TaxDetails? MapToGrpcResponse(OrganizationTaxDetails? src) =>
        src is null
            ? null
            : new TaxDetails
            {
                Id = src.Id,
                IsRegistered = src.IsRegistered,
                TaxId = src.TaxId.ToSafeString(),
                TaxRatePercentage = Convert.ToDouble(src.TaxRatePercentage),
            };

    private static PhysicalAddress? MapToGrpcResponse(OrganizationPhysicalAddress? src) =>
        src is null
            ? null
            : new PhysicalAddress
            {
                Id = src.Id,
                AddressLine1 = src.AddressLine1.ToSafeString(),
                AddressLine2 = src.AddressLine2.ToSafeString(),
                Suburb = src.Suburb.ToSafeString(),
                City = src.City.ToSafeString(),
                Province = src.Province.ToSafeString(),
                Zipcode = src.Zipcode.ToSafeString(),
                Country = src.Country.ToSafeString(),
                CountryCode = src.CountryCode.ToSafeString(),
                FormattedAddress = src.ToFormattedAddress(),
                OsmType = src.OsmType.ToSafeString(),
                OsmId = src.OsmId.ToSafeString(),
                PlaceId = src.PlaceId.ToSafeString(),
                Coordinates = src.Coordinates is null
                    ? null
                    : new Coordinates
                    {
                        Longitude = src.Coordinates.X,
                        Latitude = src.Coordinates.Y,
                    },
            };


    private static IEnumerable<CdnImageFile> MapTo(IEnumerable<global::Api.Shared.Grpc.Skedular.Organization.Core.V1.CdnImageFile> src) =>
        src.Select(MapTo);

    private static CdnImageFile MapTo(global::Api.Shared.Grpc.Skedular.Organization.Core.V1.CdnImageFile src) =>
        new(MapTo(src.Original), MapTo(src.Thumbnail));

    private static CdnFile? MapTo(global::Api.Shared.Grpc.Skedular.Organization.Core.V1.CdnFile? src) =>
        src is null ? null : new CdnFile(src.Url, src.Height.FromNullInt(), src.Width.FromNullInt());

    private static IEnumerable<global::Api.Shared.Grpc.Skedular.Organization.Core.V1.CdnImageFile> MapTo(IEnumerable<CdnImageFile> src) =>
        src.Select(MapTo);

    private static global::Api.Shared.Grpc.Skedular.Organization.Core.V1.CdnImageFile MapTo(CdnImageFile src) =>
        new()
        {
            Original = MapTo(src.Original),
            Thumbnail = MapTo(src.Thumbnail),
        };

    private static global::Api.Shared.Grpc.Skedular.Organization.Core.V1.CdnFile? MapTo(CdnFile? src) =>
        src is null
            ? null
            : new global::Api.Shared.Grpc.Skedular.Organization.Core.V1.CdnFile
            {
                Url = src.Url.ToSafeString(),
                Height = src.Height.ToNullInt(),
                Width = src.Width.ToNullInt(),
            };

    private static ListingMetadata MapTo(global::Api.Shared.Grpc.Skedular.Organization.Core.V1.ListingMetadata? src) =>
        src is null
            ? ListingMetadata.Empty
            : new ListingMetadata(src.About.ToSafeString(), src.Title.ToSafeString(), src.SubTitle.ToSafeString(),
                src.IncludedFeatures.ToSafeCollection());

    private static global::Api.Shared.Grpc.Skedular.Organization.Core.V1.ListingMetadata MapTo(ListingMetadata src)
    {
        var listingMetadata = new global::Api.Shared.Grpc.Skedular.Organization.Core.V1.ListingMetadata
        {
            About = src.About.ToSafeString(),
            Title = src.Title.ToSafeString(),
            SubTitle = src.SubTitle.ToSafeString(),
        };

        listingMetadata.IncludedFeatures.AddRange(src.IncludedFeatures.ToSafeCollection().Select(item => item.ToSafeString()));

        return listingMetadata;
    }
}
