using Organization.Shared.Models;

namespace Organization.Api.Models;

public record OrganizationAnalytics(
    string Id,
    IReadOnlyList<OrganizationMemberAttendancePercentage> MemberAttendancePercentage,
    IReadOnlyList<OrganizationDailyBookingsTotal> DailyBookingsTotal);
