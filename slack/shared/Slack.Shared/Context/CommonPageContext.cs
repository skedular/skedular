using System.Text.Json;

namespace Slack.Shared.Context;

public record CommonPageContext(PageContext PageContext)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static CommonPageContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<CommonPageContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record AddBookingContext(
    PageContext PageContext,
    DateTimeOffset? From,
    string? CustomerId,
    string? LocationId,
    string? TeamId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static AddBookingContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<AddBookingContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public class InstantAddBookingContext(
    PageContext pageContext,
    DateTimeOffset from,
    DateTimeOffset to,
    InitiationSource initiationSource,
    string? customerId,
    string? locationId,
    string? teamId)
{
    public PageContext PageContext { get; set; } = pageContext;
    public DateTimeOffset From { get; set; } = from;
    public DateTimeOffset To { get; set; } = to;
    public InitiationSource InitiationSource { get; set; } = initiationSource;
    public string? CustomerId { get; set; } = customerId;
    public string? LocationId { get; set; } = locationId;
    public string? TeamId { get; set; } = teamId;

    public string Serialize() => JsonSerializer.Serialize(this);

    public static InstantAddBookingContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<InstantAddBookingContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record EditBookingContext(PageContext PageContext, string BookingId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static EditBookingContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<EditBookingContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record CancelBookingContext(PageContext PageContext, string BookingId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static CancelBookingContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<CancelBookingContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record JoinBookingContext(PageContext PageContext, string BookingId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static JoinBookingContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<JoinBookingContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record ClearDefaultTeamContext(PageContext PageContext, string TeamId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static ClearDefaultTeamContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<ClearDefaultTeamContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record SetAsDefaultTeamContext(PageContext PageContext, string TeamId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static SetAsDefaultTeamContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<SetAsDefaultTeamContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public class EditTeamContext(PageContext pageContext, string teamId)
{
    public PageContext PageContext { get; } = pageContext;
    public string TeamId { get; set; } = teamId;
    public string Serialize() => JsonSerializer.Serialize(this);

    public static EditTeamContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<EditTeamContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public class RemoveTeamContext(PageContext pageContext, string teamId)
{
    public PageContext PageContext { get; } = pageContext;
    public string TeamId { get; set; } = teamId;
    public string Serialize() => JsonSerializer.Serialize(this);

    public static RemoveTeamContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<RemoveTeamContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record ClearDefaultLocationContext(PageContext PageContext, string LocationId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static ClearDefaultLocationContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<ClearDefaultLocationContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record SetAsDefaultLocationContext(PageContext PageContext, string LocationId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static SetAsDefaultLocationContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<SetAsDefaultLocationContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public class EditLocationContext(PageContext pageContext, string locationId)
{
    public PageContext PageContext { get; } = pageContext;
    public string LocationId { get; set; } = locationId;
    public string Serialize() => JsonSerializer.Serialize(this);

    public static EditLocationContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<EditLocationContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public class RemoveLocationContext(PageContext pageContext, string locationId)
{
    public PageContext PageContext { get; } = pageContext;
    public string LocationId { get; set; } = locationId;
    public string Serialize() => JsonSerializer.Serialize(this);

    public static RemoveLocationContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<RemoveLocationContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public class EditZoneContext(PageContext pageContext, string locationId, string zoneId)
{
    public PageContext PageContext { get; } = pageContext;
    public string LocationId { get; set; } = locationId;
    public string ZoneId { get; set; } = zoneId;
    public string Serialize() => JsonSerializer.Serialize(this);

    public static EditZoneContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<EditZoneContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record RemovePreferredZoneContext(PageContext PageContext, string ZoneId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static RemovePreferredZoneContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<RemovePreferredZoneContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record SetPreferredZoneContext(PageContext PageContext, string ZoneId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static SetPreferredZoneContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<SetPreferredZoneContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public class RemoveZoneContext(PageContext pageContext, string locationId, string zoneId)
{
    public PageContext PageContext { get; } = pageContext;
    public string LocationId { get; set; } = locationId;
    public string ZoneId { get; set; } = zoneId;
    public string Serialize() => JsonSerializer.Serialize(this);

    public static RemoveZoneContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<RemoveZoneContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public class EditDeskContext(PageContext pageContext, string locationId, string deskId)
{
    public PageContext PageContext { get; } = pageContext;
    public string LocationId { get; set; } = locationId;
    public string DeskId { get; set; } = deskId;
    public string Serialize() => JsonSerializer.Serialize(this);

    public static EditDeskContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<EditDeskContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record RemovePreferredDeskContext(PageContext PageContext, string DeskId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static RemovePreferredDeskContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<RemovePreferredDeskContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record SetPreferredDeskContext(PageContext PageContext, string DeskId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static SetPreferredDeskContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<SetPreferredDeskContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public class RemoveDeskContext(PageContext pageContext, string locationId, string deskId)
{
    public PageContext PageContext { get; } = pageContext;
    public string DeskId { get; set; } = deskId;
    public string LocationId { get; set; } = locationId;
    public string Serialize() => JsonSerializer.Serialize(this);

    public static RemoveDeskContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<RemoveDeskContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record AddZoneContext(PageContext PageContext, string? LocationId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static AddZoneContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<AddZoneContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record AddDeskContext(PageContext PageContext, string LocationId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static AddDeskContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<AddDeskContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record BulkAddDesksContext(PageContext PageContext, string LocationId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static BulkAddDesksContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<BulkAddDesksContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record AddDeskTypeContext(PageContext PageContext)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static AddDeskTypeContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<AddDeskTypeContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record RemovePreferredDeskTypeContext(PageContext PageContext, string DeskTypeId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static RemovePreferredDeskTypeContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<RemovePreferredDeskTypeContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record SetPreferredDeskTypeContext(PageContext PageContext, string DeskTypeId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static SetPreferredDeskTypeContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<SetPreferredDeskTypeContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public class EditDeskTypeContext(PageContext pageContext, string deskTypeId)
{
    public PageContext PageContext { get; } = pageContext;
    public string DeskTypeId { get; set; } = deskTypeId;
    public string Serialize() => JsonSerializer.Serialize(this);

    public static EditDeskTypeContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<EditDeskTypeContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public class RemoveDeskTypeContext(PageContext pageContext, string deskTypeId)
{
    public PageContext PageContext { get; } = pageContext;
    public string DeskTypeId { get; set; } = deskTypeId;
    public string Serialize() => JsonSerializer.Serialize(this);

    public static RemoveDeskTypeContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<RemoveDeskTypeContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}
