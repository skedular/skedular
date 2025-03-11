using Api.Shared.Clients.Events.Skedular.Location.V1.Value;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using Location.Shared.Models;
using Desk = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Desk;
using Room = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Room;
using LocationMember = Api.Shared.Clients.Events.Skedular.Location.V1.Value.LocationMember;
using OpeningHours = Api.Shared.Services.Models.OpeningHours;
using OpeningHoursDetails = Api.Shared.Services.Models.OpeningHoursDetails;
using Resource = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Resource;

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
            OrganizationId = src.Organization is null ? string.Empty : src.Organization.Id,
            OpeningHours = MapTo(src.OpeningHours)
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

        location.Resources.AddRange(src.Resources.Select(item =>
        {
            var resource = new Resource
            {
                Id = item.Id,
                Name = item.Name.ToSafeString(),
                Inactive = item.Inactive,
                RequireBookingApproval = item.RequireBookingApproval,
                Color = item.Color.ToSafeString(),
                IsOpeningHoursOverriden = item.IsOpeningHoursOverriden ?? false,
                OpeningHours = MapTo(item.OpeningHours)
            };

            resource.TagIds.AddRange(item.Tags.Select(tag => tag.Id));

            return resource;
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

            desk.TagIds.AddRange(item.Tags.Select(tag => tag.Id));

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

            room.TagIds.AddRange(item.Tags.Select(tag => tag.Id));

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

    private static Api.Shared.Clients.Events.Skedular.Location.V1.Value.OpeningHours? MapTo(OpeningHours? src)
    {
        if (src is null)
        {
            return null;
        }

        var openingHours = new Api.Shared.Clients.Events.Skedular.Location.V1.Value.OpeningHours
        {
            Monday = MapTo(src.Monday),
            Tuesday = MapTo(src.Tuesday),
            Wednesday = MapTo(src.Wednesday),
            Thursday = MapTo(src.Thursday),
            Friday = MapTo(src.Friday),
            Saturday = MapTo(src.Saturday),
            Sunday = MapTo(src.Sunday)
        };

        openingHours.ClosedDates.AddRange(src.ClosedDates.Select(item => item.ToTimestamp()));
        openingHours.DatesWithVariedOpeningHours.AddRange(src.DatesWithVariedOpeningHours.ToList().Select(item => new VariedDateOpeningHours
        {
            Date = item.Key.ToTimestamp(),
            OpeningHoursDetails = MapTo(item.Value)
        }));

        return openingHours;
    }

    private static Api.Shared.Clients.Events.Skedular.Location.V1.Value.OpeningHoursDetails MapTo(OpeningHoursDetails src) =>
        new()
        {
            IsClosed = src.IsClosed,
            Open24 = src.Open24,
            From = src.From is null ? string.Empty : src.From.ToString(),
            Until = src.Until is null ? string.Empty : src.Until.ToString()
        };
}
