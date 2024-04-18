using Slack.Shared.Context;

namespace Slack.Api.Services;

public interface IBookingsPageContextService
{
    BookingsPage GetDefaultBookingsPageContext();
}

public class BookingsPageContextService : IBookingsPageContextService
{
    public BookingsPage GetDefaultBookingsPageContext() =>
        new(new PaginationContext(), new DateRange(null, null), true);
}
