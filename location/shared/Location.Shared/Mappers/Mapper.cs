using Api.Shared.Clients.Events.UnityHub.Location.V1.Value;
using Api.Shared.Models;
using Enterprise.Shared;
using Location.Shared.Models;
using Desk = Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Desk;
using Tag = Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Tag;

namespace Location.Shared.Mappers;

public interface IMapper
{
    Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Location MapTo(Models.Location src);
    public InvitationToJoinLocation MapTo(JoinInvitation src, string? inviteeIdToOverride);
}

public class Mapper : IMapper
{
    public Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Location MapTo(Models.Location src)
    {
        var location = new Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Location
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            OrganizationId = src.Organization is null ? string.Empty : src.Organization.Id
        };

        location.Members.AddRange(src.LocationMembers.Select(item =>
        {
            var membershipType =
                item.MembershipType switch
                {
                    LocationMembershipType.Owner => MembershipType.Owner,
                    LocationMembershipType.Administrator => MembershipType.Administrator,
                    LocationMembershipType.Member => MembershipType.Member,
                    _ => throw new ArgumentOutOfRangeException()
                };

            return new Member { Id = item.Id, CustomerId = item.Customer.Id, MembershipType = membershipType };
        }));

        location.Tags.AddRange(src.Tags.Select(item => new Tag
        {
            Id = item.Id,
            Name = item.Name.ToSafeString(),
            Description = item.Description.ToSafeString(),
            TagType = item.Type.ToSafeString()
        }));

        location.Desks.AddRange(src.Desks.Select(item =>
        {
            var desk = new Desk
            {
                Id = item.Id,
                Name = item.Name.ToSafeString(),
                Deactivated = item.Deactivated,
                RequireBookingApproval = item.RequireBookingApproval
            };

            desk.LocationTagIds.AddRange(item.Tags.Select(tag => tag.Id));
            desk.OrganizationTagIds.AddRange(item.OrganizationTags.Select(tag => tag.Id));

            return desk;
        }));

        return location;
    }

    public InvitationToJoinLocation MapTo(JoinInvitation src, string? inviteeIdToOverride) =>
        new()
        {
            Id = src.Id,
            LocationId = src.Location.Id,
            InvitedById = src.CreatedBy.Id,
            InviteeId = inviteeIdToOverride ?? (src.Invitee is null ? string.Empty : src.Invitee.Id)
        };
}
