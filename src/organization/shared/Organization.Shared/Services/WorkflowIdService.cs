using Enterprise.Shared.Temporal;
using Organization.Shared.Workflows;

namespace Organization.Shared.Services;

public interface IWorkflowIdService
{
    string GenerateOrganizationDailyAnalytics(string organizationId);
    string RecomputeOrganizationBookingDerivedState(string organizationId);
    string ReSyncAzureTenant(string tenantId);
    string AddOrganizationStripePaymentMethod(string clientSecret);
    string ScheduleRenewOrganizationOffering(string organizationOfferingId);
    string InviteToJoin(string joinInvitationId);
    string MaintainOrganizationXeroConnection(string organizationId);
    string NewOrganizationJoined(string? organizationId, string? organizationCustomDomain);
}

public class WorkflowIdService(ITemporalHelperService temporalHelperService) : IWorkflowIdService
{
    public string GenerateOrganizationDailyAnalytics(string organizationId) =>
        temporalHelperService.ToId($"{Constants.GenerateOrganizationDailyAnalyticsPrefix}-{organizationId}");

    public string RecomputeOrganizationBookingDerivedState(string organizationId) =>
        temporalHelperService.ToId($"{Constants.RecomputeOrganizationBookingDerivedStatePrefix}-{organizationId}");

    public string ReSyncAzureTenant(string tenantId) =>
        temporalHelperService.ToId($"{Constants.ReSyncAzureTenantPrefix}-{tenantId}");

    public string AddOrganizationStripePaymentMethod(string clientSecret) =>
        temporalHelperService.ToId(clientSecret);

    public string ScheduleRenewOrganizationOffering(string organizationOfferingId) =>
        temporalHelperService.ToId(organizationOfferingId);

    public string InviteToJoin(string joinInvitationId) =>
        temporalHelperService.ToId(joinInvitationId);

    public string MaintainOrganizationXeroConnection(string organizationId) =>
        temporalHelperService.ToId($"{Constants.MaintainOrganizationXeroConnectionPrefix}-{organizationId}");

    public string NewOrganizationJoined(string? organizationId, string? organizationCustomDomain) =>
        temporalHelperService.ToId(
            $"{Constants.NewOrganizationJoinedPrefix}-{organizationId ?? string.Empty}-{organizationCustomDomain ?? string.Empty}");
}
