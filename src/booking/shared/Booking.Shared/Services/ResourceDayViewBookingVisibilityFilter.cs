using Api.Shared.Services.Models;
using Booking.Shared.Models;

namespace Booking.Shared.Services;

/// <summary>
///     Applies booking detail visibility rules to a <see cref="ResourceDayView" /> based on the
///     requesting user's organization type and roles.
///     Rules:
///     - Private organizations: all booking detail fields are visible to any member.
///     - Marketplace / Individual organizations: only admins and owners see full detail;
///     regular members see <c>BookedByName</c>, <c>BookedByUserId</c>, and <c>Notes</c> set to null.
/// </summary>
public interface IResourceDayViewBookingVisibilityFilter
{
    /// <summary>
    ///     Applies visibility rules to all booking windows in the given views.
    ///     Returns a new list of <see cref="ResourceDayView" /> with detail fields redacted when appropriate.
    /// </summary>
    IReadOnlyList<ResourceDayView> Apply(
        IReadOnlyList<ResourceDayView> views,
        string organizationType,
        IReadOnlyList<string> userRoles);
}

/// <inheritdoc cref="IResourceDayViewBookingVisibilityFilter" />
public class ResourceDayViewBookingVisibilityFilter : IResourceDayViewBookingVisibilityFilter
{
    private const string AdminRole = "ADMIN";
    private const string OwnerRole = "OWNER";

    /// <inheritdoc />
    public IReadOnlyList<ResourceDayView> Apply(IReadOnlyList<ResourceDayView> views, string organizationType, IReadOnlyList<string> userRoles)
    {
        if (IsFullyVisible(organizationType, userRoles))
        {
            return views;
        }

        return views.Select(view => view with
        {
            BookingWindows = view.BookingWindows.Select(Redact).ToList(),
        }).ToList();
    }

    private static bool IsFullyVisible(string organizationType, IReadOnlyList<string> userRoles)
    {
        if (organizationType == OrganizationTypeConstants.Private)
        {
            return true;
        }

        return userRoles.Any(item =>
            string.Equals(item, AdminRole, StringComparison.InvariantCultureIgnoreCase) ||
            string.Equals(item, OwnerRole, StringComparison.InvariantCultureIgnoreCase));
    }

    private static BookingWindow Redact(BookingWindow window) =>
        window with
        {
            BookedByName = null,
            BookedByUserId = null,
            Notes = null,
        };
}
