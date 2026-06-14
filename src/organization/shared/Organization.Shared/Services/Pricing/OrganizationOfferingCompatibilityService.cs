using Microsoft.Extensions.Logging;
using Organization.Shared.Mappers;
using Organization.Shared.Models.PricingCatalog;
using OrganizationOffering = Organization.Shared.Database.Entities.OrganizationOffering;

namespace Organization.Shared.Services.Pricing;

public interface IOrganizationOfferingCompatibilityService
{
    Task<OrganizationOfferingPlan?> GetTeamsOfferingPlanAsync(
        string organizationId,
        OrganizationOffering? activeLegacyOffering,
        DateTimeOffset at,
        CancellationToken cancellationToken);
}

public class OrganizationOfferingCompatibilityService(
    ILegacyOfferingCompatibilityMapper legacyOfferingCompatibilityMapper,
    ILogger<OrganizationOfferingCompatibilityService> logger) : IOrganizationOfferingCompatibilityService
{
    public async Task<OrganizationOfferingPlan?> GetTeamsOfferingPlanAsync(
        string organizationId,
        OrganizationOffering? activeLegacyOffering,
        DateTimeOffset at,
        CancellationToken cancellationToken)
    {
        if (activeLegacyOffering is null)
        {
            return null;
        }

        logger.LogInformation(
            "{EventName}: resolved read-only legacy organization Teams offering for {OrganizationId}",
            PricingLogEvents.ExistingOfferingCompatibilityResolved,
            organizationId);

        return legacyOfferingCompatibilityMapper.MapToReadOnlyOfferingPlan(activeLegacyOffering);
    }
}
