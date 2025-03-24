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
    DateTimeOffset until,
    InitiationSource initiationSource,
    string? customerId,
    string? locationId,
    string? teamId)
{
    public PageContext PageContext { get; set; } = pageContext;
    public DateTimeOffset From { get; set; } = from;
    public DateTimeOffset Until { get; set; } = until;
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

public record RemovePreferredTeamContext(PageContext PageContext, string TeamId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static RemovePreferredTeamContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<RemovePreferredTeamContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record AddAsPreferredTeamContext(PageContext PageContext, string TeamId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static AddAsPreferredTeamContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<AddAsPreferredTeamContext>(value);
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

public record ClearPreferredLocationContext(PageContext PageContext, string LocationId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static ClearPreferredLocationContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<ClearPreferredLocationContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record AddAsPreferredLocationContext(PageContext PageContext, string LocationId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static AddAsPreferredLocationContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<AddAsPreferredLocationContext>(value);
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

public class EditResourceContext(PageContext pageContext, string locationId, string resourceId)
{
    public PageContext PageContext { get; } = pageContext;
    public string LocationId { get; set; } = locationId;
    public string ResourceId { get; set; } = resourceId;
    public string Serialize() => JsonSerializer.Serialize(this);

    public static EditResourceContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<EditResourceContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record RemovePreferredResourceContext(PageContext PageContext, string ResourceId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static RemovePreferredResourceContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<RemovePreferredResourceContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record SetPreferredResourceContext(PageContext PageContext, string ResourceId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static SetPreferredResourceContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<SetPreferredResourceContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public class RemoveResourceContext(PageContext pageContext, string locationId, string resourceId)
{
    public PageContext PageContext { get; } = pageContext;
    public string ResourceId { get; set; } = resourceId;
    public string LocationId { get; set; } = locationId;
    public string Serialize() => JsonSerializer.Serialize(this);

    public static RemoveResourceContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<RemoveResourceContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record AddZoneContext(PageContext PageContext)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static AddZoneContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<AddZoneContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record AddResourceContext(PageContext PageContext, string LocationId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static AddResourceContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<AddResourceContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record AddCustomTagContext(PageContext PageContext)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static AddCustomTagContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<AddCustomTagContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record RemovePreferredCustomTagContext(PageContext PageContext, string CustomTagId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static RemovePreferredCustomTagContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<RemovePreferredCustomTagContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public record SetPreferredCustomTagContext(PageContext PageContext, string CustomTagId)
{
    public string Serialize() => JsonSerializer.Serialize(this);

    public static SetPreferredCustomTagContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<SetPreferredCustomTagContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public class EditCustomTagContext(PageContext pageContext, string customTagId)
{
    public PageContext PageContext { get; } = pageContext;
    public string CustomTagId { get; set; } = customTagId;
    public string Serialize() => JsonSerializer.Serialize(this);

    public static EditCustomTagContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<EditCustomTagContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}

public class RemoveCustomTagContext(PageContext pageContext, string customTagId)
{
    public PageContext PageContext { get; } = pageContext;
    public string CustomTagId { get; set; } = customTagId;
    public string Serialize() => JsonSerializer.Serialize(this);

    public static RemoveCustomTagContext Deserialize(string value)
    {
        var context = JsonSerializer.Deserialize<RemoveCustomTagContext>(value);
        ArgumentNullException.ThrowIfNull(context);
        return context;
    }
}
