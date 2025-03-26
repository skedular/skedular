using Api.Shared.Clients.Events.Skedular.Location.V1.Value;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using Location.Shared.Models;
using LocationMember = Api.Shared.Clients.Events.Skedular.Location.V1.Value.LocationMember;
using OpeningHours = Api.Shared.Services.Models.OpeningHours;
using OpeningHoursDetails = Api.Shared.Services.Models.OpeningHoursDetails;
using Resource = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Resource;
using WeekOpeningHours = Api.Shared.Services.Models.WeekOpeningHours;

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
            OrganizationId = src.Organization.Id,
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
                Capacity = item.Capacity,
                IsAvailableHoursOverridden = item.IsAvailableHoursOverridden,
                AvailableHours = item.AvailableHours is null ? null : MapTo(item.AvailableHours)
            };

            resource.TagIds.AddRange(item.Tags.Select(tag => tag.Id));

            return resource;
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

    private static Api.Shared.Clients.Events.Skedular.Location.V1.Value.OpeningHours MapTo(OpeningHours? src)
    {
        if (src is null)
        {
            return new Api.Shared.Clients.Events.Skedular.Location.V1.Value.OpeningHours
            {
                WeekOpeningHours = new Api.Shared.Clients.Events.Skedular.Location.V1.Value.WeekOpeningHours
                {
                    Monday = MapToDefault(),
                    Tuesday = MapToDefault(),
                    Wednesday = MapToDefault(),
                    Thursday = MapToDefault(),
                    Friday = MapToDefault(),
                    Saturday = MapToDefault(),
                    Sunday = MapToDefault()
                }
            };
        }

        var openingHours = new Api.Shared.Clients.Events.Skedular.Location.V1.Value.OpeningHours { WeekOpeningHours = MapTo(src.WeekOpeningHours) };
        openingHours.ClosedDates.AddRange(src.ClosedDates.Select(item => item.ToTimestamp()));
        openingHours.DatesWithVariedOpeningHours.AddRange(src.DatesWithVariedOpeningHours.ToList().Select(item => new VariedDateOpeningHours
        {
            Date = item.Key.ToTimestamp(), OpeningHoursDetails = MapTo(item.Value)
        }));

        return openingHours;
    }

    private static Api.Shared.Clients.Events.Skedular.Location.V1.Value.WeekOpeningHours MapTo(WeekOpeningHours src) =>
        new()
        {
            Monday = MapTo(src.Monday),
            Tuesday = MapTo(src.Tuesday),
            Wednesday = MapTo(src.Wednesday),
            Thursday = MapTo(src.Thursday),
            Friday = MapTo(src.Friday),
            Saturday = MapTo(src.Saturday),
            Sunday = MapTo(src.Sunday)
        };

    private static Api.Shared.Clients.Events.Skedular.Location.V1.Value.OpeningHoursDetails MapTo(OpeningHoursDetails src) =>
        new()
        {
            Closed = src.Closed,
            OpenAllDay = src.OpenAllDay,
            From = src.From is null ? string.Empty : $"{src.From.Value.Hour}:{src.From.Value.Minute}",
            Until = src.Until is null ? string.Empty : $"{src.Until.Value.Hour}:{src.Until.Value.Minute}"
        };

    private static Api.Shared.Clients.Events.Skedular.Location.V1.Value.OpeningHoursDetails MapToDefault() =>
        new() { Closed = false, OpenAllDay = true, From = string.Empty, Until = string.Empty };
}
