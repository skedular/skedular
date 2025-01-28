using Api.Shared.Clients.Events.Skedular.Location.V1.Value;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Location.Shared.Models;
using Desk = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Desk;
using Room = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Room;
using LocationMember = Api.Shared.Clients.Events.Skedular.Location.V1.Value.LocationMember;

namespace Location.Shared.Mappers;

public interface IMapper
{
    Api.Shared.Clients.Events.Skedular.Location.V1.Value.Location MapTo(Models.Location src);
    public InvitationToJoinLocation MapTo(JoinInvitation src, string? inviteeIdToOverride);
}

public class Mapper : IMapper
{
    public Api.Shared.Clients.Events.Skedular.Location.V1.Value.Location MapTo(Models.Location src)
    {
        var location = new Api.Shared.Clients.Events.Skedular.Location.V1.Value.Location
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            OrganizationId = src.Organization is null ? string.Empty : src.Organization.Id
        };

        location.Members.AddRange(src.LocationMembers.Select(item => new LocationMember
        {
            Id = item.Id,
            CustomerId = item.Customer.Id,
            Role = item.Role switch
            {
                LocationMemberRole.Owner => Role.Owner,
                LocationMemberRole.Administrator => Role.Administrator,
                LocationMemberRole.Member => Role.Member,
                _ => throw new ArgumentOutOfRangeException()
            }
        }));

        location.Desks.AddRange(src.Desks.Select(item =>
        {
            var desk = new Desk
            {
                Id = item.Id,
                Name = item.Name.ToSafeString(),
                Deactivated = item.Deactivated,
                RequireBookingApproval = item.RequireBookingApproval,
                Color = item.Color.ToSafeString()
            };

            desk.CustomTagIds.AddRange(item.CustomTags.Select(tag => tag.Id));
            desk.ZoneIds.AddRange(item.Zones.Select(tag => tag.Id));

            return desk;
        }));

        location.Rooms.AddRange(src.Rooms.Select(item =>
        {
            var room = new Room
            {
                Id = item.Id,
                Name = item.Name.ToSafeString(),
                Deactivated = item.Deactivated,
                RequireBookingApproval = item.RequireBookingApproval,
                Color = item.Color.ToSafeString()
            };

            room.CustomTagIds.AddRange(item.CustomTags.Select(tag => tag.Id));
            room.ZoneIds.AddRange(item.Zones.Select(tag => tag.Id));

            return room;
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
