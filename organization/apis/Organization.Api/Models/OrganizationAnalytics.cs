using Organization.Shared.Models;

namespace Organization.Api.Models;

public record OrganizationAnalytics(
    string Id,
    ICollection<OrganizationMemberAttendancePercentage> MemberAttendancePercentage,
    ICollection<OrganizationDailyBookingsTotal> DailyBookingsTotal);
