using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Api.Shared.Services.Offering;

public interface IPricingEntitlementEvaluator
{
    EntitlementDecision EvaluateActiveUser(Models.Offering? offering, string customerId);
    EntitlementDecision EvaluateActiveUserCount(Models.Offering? offering, int currentActiveUserCount);
    EntitlementDecision EvaluateTeamCreation(Models.Offering? offering, int currentTeamCount);
    EntitlementDecision EvaluateLocationCreation(Models.Offering? offering, int currentLocationCount);
}

public class PricingEntitlementEvaluator(ILogger<PricingEntitlementEvaluator>? logger = null) : IPricingEntitlementEvaluator
{
    private readonly ILogger<PricingEntitlementEvaluator> _logger = logger ?? NullLogger<PricingEntitlementEvaluator>.Instance;

    public EntitlementDecision EvaluateActiveUser(Models.Offering? offering, string customerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

        if (offering is null)
        {
            return Denied(EntitlementReasonCode.OfferingNotFound, "No active organization offering found.");
        }

        if (offering.ActiveCustomerIds.Contains(customerId))
        {
            return Allowed("Existing active user remains allowed for the current offering period.");
        }

        var limit = offering.PurchasedUserCapacity ?? offering.Code.GetOffering().MaxUserCount;
        if (limit == -1 || offering.Code.IsPayAsYouGoOffering() || offering.Code.IsEarlyBirdOffering())
        {
            return Allowed("Organization offering allows additional active users.");
        }

        if (offering.ActiveCustomerIds.Count < limit)
        {
            return Allowed("Organization has active-user capacity available.");
        }

        var reasonCode = offering.Code.IsEnterpriseOffering()
            ? EntitlementReasonCode.EnterpriseCapacityReached
            : EntitlementReasonCode.FreeActiveUserLimitReached;

        return Denied(reasonCode, $"Organization has reached the active-user capacity of {limit}.");

    }

    public EntitlementDecision EvaluateActiveUserCount(Models.Offering? offering, int currentActiveUserCount)
    {
        if (offering is null)
        {
            return Denied(EntitlementReasonCode.OfferingNotFound, "No active organization offering found.");
        }

        var limit = offering.PurchasedUserCapacity ?? offering.Code.GetOffering().MaxUserCount;
        if (limit == -1 || offering.Code.IsPayAsYouGoOffering() || offering.Code.IsEarlyBirdOffering())
        {
            return Allowed("Organization offering allows additional active users.");
        }

        if (currentActiveUserCount < limit)
        {
            return Allowed("Organization has active-user capacity available.");
        }

        var reasonCode = offering.Code.IsEnterpriseOffering()
            ? EntitlementReasonCode.EnterpriseCapacityReached
            : EntitlementReasonCode.FreeActiveUserLimitReached;

        return Denied(reasonCode, $"Organization has reached the active-user capacity of {limit}.");

    }

    public EntitlementDecision EvaluateTeamCreation(Models.Offering? offering, int currentTeamCount)
    {
        if (offering is null)
        {
            return Denied(EntitlementReasonCode.OfferingNotFound, "No active organization offering found.");
        }

        var limit = offering.PurchasedTeamCapacity ?? offering.Code.GetOffering().MaxTeamCount;
        if (limit == -1 || currentTeamCount < limit)
        {
            return Allowed("Organization has team capacity available.");
        }

        return Denied(EntitlementReasonCode.FreeTeamLimitReached, $"Organization has reached the team capacity of {limit}.");
    }

    public EntitlementDecision EvaluateLocationCreation(Models.Offering? offering, int currentLocationCount)
    {
        if (offering is null)
        {
            return Denied(EntitlementReasonCode.OfferingNotFound, "No active organization offering found.");
        }

        var limit = offering.PurchasedLocationCapacity ?? offering.Code.GetOffering().MaxLocationCount;
        if (limit == -1 || currentLocationCount < limit)
        {
            return Allowed("Organization has location capacity available.");
        }

        return Denied(EntitlementReasonCode.FreeLocationLimitReached, $"Organization has reached the location capacity of {limit}.");
    }

    private EntitlementDecision Allowed(string message)
    {
        _logger.LogInformation("Pricing entitlement check allowed with reason {ReasonCode}", EntitlementReasonCode.Allowed);
        return new EntitlementDecision(true, EntitlementReasonCode.Allowed, message);
    }

    private EntitlementDecision Denied(EntitlementReasonCode reasonCode, string message)
    {
        _logger.LogWarning("Pricing entitlement check denied with reason {ReasonCode}: {Message}", reasonCode, message);
        return new EntitlementDecision(false, reasonCode, message);
    }
}
