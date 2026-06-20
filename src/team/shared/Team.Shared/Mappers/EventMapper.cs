using Api.Shared.Clients.Events.Skedular.Team.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using CdnFile = Api.Shared.Clients.Events.Skedular.Team.V1.CdnFile;
using CdnImageFile = Api.Shared.Clients.Events.Skedular.Team.V1.CdnImageFile;
using OrganizationMember = Api.Shared.Clients.Events.Skedular.Team.V1.OrganizationMember;
using TeamMember = Api.Shared.Clients.Events.Skedular.Team.V1.TeamMember;

namespace Team.Shared.Mappers;

public interface IEventMapper
{
    Api.Shared.Clients.Events.Skedular.Team.V1.Team MapTo(Models.Team src);
}

public class EventMapper : IEventMapper
{
    public Api.Shared.Clients.Events.Skedular.Team.V1.Team MapTo(Models.Team src)
    {
        var team = new Api.Shared.Clients.Events.Skedular.Team.V1.Team
        {
            Id = src.Id,
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            OrganizationId = src.Organization.Id,
            PrimaryLocationId = src.PrimaryLocation is null ? string.Empty : src.PrimaryLocation.Id
        };

        team.Members.AddRange(src.TeamMembers.Select(item => new TeamMember
        {
            Id = item.Id,
            CustomerId = item.Customer.Id,
            Role = item.Role switch
            {
                TeamMemberRole.Owner => Role.Owner,
                TeamMemberRole.Administrator => Role.Administrator,
                TeamMemberRole.Member => Role.Member,
                _ => throw new ArgumentOutOfRangeException(nameof(item.Role), item.Role,
                    $"Unexpected value for {nameof(item.Role)}: {item.Role}. Update enum mapping or caller input.")
            },
            Status = item.Status switch
            {
                TeamMemberStatus.Active => Status.Active,
                TeamMemberStatus.Inactive => Status.Inactive,
                _ => throw new ArgumentOutOfRangeException(nameof(item.Status), item.Status,
                    $"Unexpected value for {nameof(item.Status)}: {item.Status}. Update enum mapping or caller input.")
            },
            OrganizationMember = item.OrganizationMember is null
                ? null
                : new OrganizationMember
                {
                    OrganizationMemberId = item.OrganizationMember.Id,
                    CustomerId = item.OrganizationMember.Customer.Id,
                    OrganizationId = item.OrganizationMember.Organization.Id
                }
        }));

        team.FeatureImages.AddRange(MapTo(src.FeatureImages));

        return team;
    }

    private static IEnumerable<CdnImageFile> MapTo(IEnumerable<Api.Shared.Services.Models.CdnImageFile> src) =>
        src.Select(MapTo);

    private static CdnImageFile MapTo(Api.Shared.Services.Models.CdnImageFile src) =>
        new() { Original = MapTo(src.Original), Thumbnail = MapTo(src.Thumbnail) };

    private static CdnFile? MapTo(Api.Shared.Services.Models.CdnFile? src) =>
        src is null ? null : new CdnFile { Url = src.Url.ToSafeString(), Height = src.Height.ToNullInt(), Width = src.Width.ToNullInt() };
}
