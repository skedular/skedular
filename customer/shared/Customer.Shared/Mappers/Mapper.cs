using Api.Shared.Clients.Events.Skedular.Customer.V1.Value;
using Customer.Shared.Database.Entities;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using Stripe;
using Identity = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Identity;
using Location = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Location;
using OrganizationTag = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.OrganizationTag;
using Resource = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Resource;
using Team = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Team;

namespace Customer.Shared.Mappers;

public interface IMapper
{
    Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Customer MapTo(Models.Customer src);
    StripePaymentMethod MapTo(PaymentMethod paymentMethod, string setupIntentId, Database.Entities.Customer customer);
}

public class Mapper : IMapper
{
    public Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Customer MapTo(Models.Customer src)
    {
        var customer = new Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Customer
        {
            Id = src.Id,
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            Title = src.Title.ToSafeString(),
            Designation = src.Designation.ToSafeString(),
            Name = src.Name.ToSafeString(),
            GivenName = src.GivenName.ToSafeString(),
            MiddleName = src.MiddleName.ToSafeString(),
            FamilyName = src.FamilyName.ToSafeString(),
            PhotoUrl = src.PhotoUrl.ToSafeString(),
            PhotoUrl24 = src.PhotoUrl24.ToSafeString(),
            PhotoUrl32 = src.PhotoUrl32.ToSafeString(),
            PhotoUrl48 = src.PhotoUrl48.ToSafeString(),
            PhotoUrl72 = src.PhotoUrl72.ToSafeString(),
            PhotoUrl192 = src.PhotoUrl192.ToSafeString(),
            PhotoUrl512 = src.PhotoUrl512.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            Locale = src.Locale.ToSafeString(),
            PhoneNumber = src.PhoneNumber.ToSafeString(),
            Settings = new Settings
            {
                IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone ?? false,
                IsLocationOnboardingDone = src.IsLocationOnboardingDone ?? false,
                IsTeamOnboardingDone = src.IsTeamOnboardingDone ?? false,
                IsDefaultOrganizationOnboardingDone = src.IsDefaultOrganizationOnboardingDone ?? false,
                IsPreferredLocationOnboardingDone = src.IsPreferredLocationOnboardingDone ?? false,
                IsPreferredZoneOnboardingDone = src.IsPreferredZoneOnboardingDone ?? false
            },
            PreferredOrganizationId = src.DefaultOrganization is null ? string.Empty : src.DefaultOrganization.Id
        };

        customer.Identities.AddRange(MapTo(src.Identities));
        customer.PreferredLocations.AddRange(
            src.PreferredLocations.Select(item =>
                new Location { Id = item.Id, OrganizationId = item.Organization is null ? string.Empty : item.Organization.Id })
        );
        customer.PreferredResources.AddRange(
            src.PreferredResources.Select(item =>
                new Resource { Id = item.Id, LocationId = item.Location is null ? string.Empty : item.Location.Id }));
        customer.PreferredTeams.AddRange(
            src.PreferredTeams.Select(item =>
                new Team { Id = item.Id, OrganizationId = item.Organization is null ? string.Empty : item.Organization.Id }));
        customer.PreferredOrganizationTags.AddRange(
            src.PreferredOrganizationTags.Select(item => new OrganizationTag { Id = item.Id, OrganizationId = item.Organization.Id })
        );

        return customer;
    }

    public StripePaymentMethod MapTo(PaymentMethod paymentMethod, string setupIntentId, Database.Entities.Customer customer) =>
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
            Customer = customer
        };

    private static IEnumerable<Identity> MapTo(IEnumerable<Models.Identity> src) => src.Select(MapTo);

    private static Identity MapTo(Models.Identity src) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = src.EmailVerified ?? false };
}
