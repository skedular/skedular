using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using Organization.Shared.Models;
using Stripe;
using Address = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Address;
using Offering = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Offering;
using OrganizationMember = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.OrganizationMember;
using OrganizationSsoSetting = Organization.Shared.Models.OrganizationSsoSetting;
using StripePaymentMethod = Organization.Shared.Database.Entities.StripePaymentMethod;
using Tag = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Tag;

namespace Organization.Shared.Mappers;

public interface IMapper
{
    Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Organization MapTo(Models.Organization src);
    InvitationToJoinOrganization MapTo(JoinInvitation src, string? inviteeIdToOverride);
    CustomerCreateOptions MapTo(Database.Entities.Organization src);
    CustomerUpdateOptions MergeTo(Database.Entities.Organization src);
    StripePaymentMethod MapTo(PaymentMethod paymentMethod, string setupIntentId, Database.Entities.Organization organization);
}

public class Mapper : IMapper
{
    public Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Organization MapTo(Models.Organization src)
    {
        var organizationOffering = src.OrganizationOfferings.Where(item => !item.DeletedAt.HasValue).OrderByDescending(item => item.End).First();
        var organization = new Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Organization
        {
            Id = src.Id,
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Website = src.Website.ToSafeString(),
            LogoUrl = src.LogoUrl.ToSafeString(),
            Type = src.Type.ToOrganizationType(),
            ContactEmail = src.ContactEmail.ToSafeString(),
            ContactPhone = src.ContactPhone.ToSafeString(),
            MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy(),
            Offering = new Offering
            {
                Id = organizationOffering.Id,
                OrganizationId = src.Id,
                Code = organizationOffering.Code.ToOfferingCode(),
                Start = organizationOffering.Start.ToTimestamp(),
                End = organizationOffering.End.ToTimestamp(),
                AutoRenew = organizationOffering.AutoRenew,
                UnitPrice = organizationOffering.UnitPrice
            },
            SsoSettings = MapTo(src.OrganizationSsoSettings),
            PhysicalAddress = MapTo(src.PhysicalAddress)
        };

        organization.AzureTenantIds.AddRange(src.AzureTenants.Select(item => item.Id));

        organization.Tags.AddRange(src.Tags.Select(item => new Tag
        {
            Id = item.Id,
            Name = item.Name.ToSafeString(),
            Description = item.Description.ToSafeString(),
            Type = item.Type.ToOrganizationTagType(),
            Color = item.Color.ToSafeString()
        }));

        organization.Offering.ActiveCustomerIds.AddRange(
            organizationOffering.OrganizationOfferingActiveMembers.Select(item => item.OrganizationMember.Customer.Id));

        organization.Members.AddRange(src.OrganizationMembers.Select(item => new OrganizationMember
        {
            Id = item.Id,
            CustomerId = item.Customer.Id,
            Role = item.Role switch
            {
                OrganizationMemberRole.Owner => Role.Owner,
                OrganizationMemberRole.Administrator => Role.Administrator,
                OrganizationMemberRole.Member => Role.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = item.Status switch
            {
                OrganizationMemberStatus.Active => Status.Active,
                OrganizationMemberStatus.Inactive => Status.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            }
        }));

        return organization;
    }

    public InvitationToJoinOrganization MapTo(JoinInvitation src, string? inviteeIdToOverride) =>
        new()
        {
            Id = src.Id,
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            OrganizationId = src.Organization.Id,
            InvitedById = src.CreatedBy.Id,
            InviteeId = inviteeIdToOverride ?? (src.Invitee is null ? string.Empty : src.Invitee.Id)
        };

    public CustomerCreateOptions MapTo(Database.Entities.Organization src) =>
        new()
        {
            Name = src.Name,
            Email = string.IsNullOrWhiteSpace(src.ContactEmail) ? null : src.ContactEmail,
            Phone = string.IsNullOrWhiteSpace(src.ContactPhone) ? null : src.ContactPhone,
            Metadata = new Dictionary<string, string> { { "type", "organization" }, { "organizationId", src.Id } }
        };

    public CustomerUpdateOptions MergeTo(Database.Entities.Organization src) =>
        new()
        {
            Name = src.Name,
            Email = string.IsNullOrWhiteSpace(src.ContactEmail) ? null : src.ContactEmail,
            Phone = string.IsNullOrWhiteSpace(src.ContactPhone) ? null : src.ContactPhone,
            Metadata = new Dictionary<string, string> { { "type", "organization" }, { "organizationId", src.Id } }
        };

    public StripePaymentMethod MapTo(PaymentMethod paymentMethod, string setupIntentId, Database.Entities.Organization organization) =>
        new()
        {
            SetupIntentId = setupIntentId,
            PaymentMethodId = paymentMethod.Id,
            CardBrand = paymentMethod.Card?.Brand,
            CardCountry = paymentMethod.Card?.Country,
            CardDescription = paymentMethod.Card?.Description,
            CardExpiryMonth = paymentMethod.Card is null ? null : (byte)paymentMethod.Card.ExpMonth,
            CardExpiryYear = paymentMethod.Card is null ? null : (short)paymentMethod.Card.ExpYear,
            CardFingerprint = paymentMethod.Card?.Fingerprint,
            CardFunding = paymentMethod.Card?.Funding,
            CardIssuer = paymentMethod.Card?.Issuer,
            CardLastFourDigit = paymentMethod.Card?.Last4,
            Organization = organization
        };

    private static OrganizationSsoSettings? MapTo(OrganizationSsoSetting? src) =>
        src is null
            ? null
            : new OrganizationSsoSettings
            {
                Id = src.Id,
                EntityId = src.EntityId.ToSafeString(),
                LoginUrl = src.LoginUrl.ToSafeString(),
                AppFederationMetadataUrl = src.AppFederationMetadataUrl.ToSafeString(),
                IsActive = src.IsActive
            };

    private static Address? MapTo(Models.Address? src) =>
        src is null
            ? null
            : new Address
            {
                Id = src.Id,
                AddressLine1 = src.AddressLine1.ToSafeString(),
                AddressLine2 = src.AddressLine2.ToSafeString(),
                Suburb = src.Suburb.ToSafeString(),
                City = src.City.ToSafeString(),
                Province = src.Province.ToSafeString(),
                Zipcode = src.Zipcode.ToSafeString(),
                Country = src.Country.ToSafeString()
            };
}
