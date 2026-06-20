using Enterprise.Shared.Temporal;
using Location.Shared.Workflows;

namespace Location.Shared.Services;

public interface IWorkflowIdService
{
    string GenerateLocationDailyAnalytics(string locationId);
    string GenerateLocationResourceAvailabilitySnapshot(string locationId, DateTimeOffset date);
    string RecomputeLocationBookingDerivedState(string locationId);
    string ComputeOrganizationLocationsAndProductsRelationships(string organizationId);
    string NewLocationJoined(string locationId);
    string ProvisionHostLocation(string locationId);
    string DeprovisionHostLocation(string locationId);
}

public class WorkflowIdService(ITemporalHelperService temporalHelperService) : IWorkflowIdService
{
    public string GenerateLocationDailyAnalytics(string locationId) =>
        temporalHelperService.ToId($"{Constants.GenerateLocationDailyAnalyticsPrefix}-{locationId}");

    public string GenerateLocationResourceAvailabilitySnapshot(string locationId, DateTimeOffset date) =>
        temporalHelperService.ToId($"{Constants.GenerateLocationDailyAnalyticsPrefix}-snapshot-{locationId}-{date:yyyyMMdd}");

    public string RecomputeLocationBookingDerivedState(string locationId) =>
        temporalHelperService.ToId($"{Constants.RecomputeLocationBookingDerivedStatePrefix}-{locationId}");

    public string ComputeOrganizationLocationsAndProductsRelationships(string organizationId) =>
        temporalHelperService.ToId($"{Constants.ComputeLocationProductRelationshipsPrefix}-{organizationId}");

    public string NewLocationJoined(string locationId) =>
        temporalHelperService.ToId($"{Constants.NewLocationJoinedPrefix}-{locationId}");

    public string ProvisionHostLocation(string locationId) =>
        temporalHelperService.ToId($"{Constants.ProvisionHostLocationPrefix}-{locationId}");

    public string DeprovisionHostLocation(string locationId) =>
        temporalHelperService.ToId($"{Constants.DeprovisionHostLocationPrefix}-{locationId}");
}
