using Api.Shared.Clients.Events.UnityHub.Customer.V1.Value;
using Enterprise.Shared;

namespace Customer.Shared.Mappers;

public interface IMapper
{
    Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Customer MapTo(Models.Customer src);
}

public class Mapper : IMapper
{
    public Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Customer MapTo(Models.Customer src)
    {
        var customer = new Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Customer
        {
            Id = src.Id,
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
                IsDefaultLocationOnboardingDone = src.IsDefaultLocationOnboardingDone ?? false,
                IsPreferredZoneOnboardingDone = src.IsPreferredZoneOnboardingDone ?? false,
                IsPreferredDeskOnboardingDone = src.IsPreferredDeskOnboardingDone ?? false
            },
            DefaultOrganizationId = src.DefaultOrganization is null ? string.Empty : src.DefaultOrganization.Id
        };

        customer.Identities.AddRange(MapTo(src.Identities));
        customer.DefaultLocations.AddRange(
            src.DefaultLocations.Select(item =>
                new Location
                {
                    Id = item.Id, OrganizationId = item.Organization is null ? string.Empty : item.Organization.Id
                })
        );
        customer.DefaultDesks.AddRange(
            src.PreferredDesks.Select(item =>
                new Desk { Id = item.Id, LocationId = item.Location.Id })
        );
        customer.DefaultTeams.AddRange(
            src.DefaultTeams.Select(item =>
                new Team
                {
                    Id = item.Id, OrganizationId = item.Organization is null ? string.Empty : item.Organization.Id
                })
        );
        customer.DefaultOrganizationTags.AddRange(
            src.PreferredOrganizationTags.Select(item =>
                new OrganizationTag { Id = item.Id, OrganizationId = item.Organization.Id })
        );

        return customer;
    }

    private static IEnumerable<Identity> MapTo(
        IEnumerable<Models.Identity> src) =>
        src.Select(MapTo);

    private static Identity MapTo(Models.Identity src) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = src.EmailVerified ?? false };
}
