namespace Api.Shared.Services.Offering;

public interface IPricingEntitlementEvaluator
{
    EntitlementDecision EvaluateActiveUser(Models.Offering? offering, string customerId);
    EntitlementDecision EvaluateActiveUserCount(Models.Offering? offering, int currentActiveUserCount);
    EntitlementDecision EvaluateTeamCreation(Models.Offering? offering, int currentTeamCount);
    EntitlementDecision EvaluateLocationCreation(Models.Offering? offering, int currentLocationCount);
}

public class PricingEntitlementEvaluator : IPricingEntitlementEvaluator
{
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
        if (!limit.HasValue || offering.Code.IsPayAsYouGoOffering() || offering.Code.IsEarlyBirdOffering())
        {
            return Allowed("Organization offering allows additional active users.");
        }

        if (offering.ActiveCustomerIds.Count < limit.Value)
        {
            return Allowed("Organization has active-user capacity available.");
        }

        var reasonCode = offering.Code.IsEnterpriseOffering()
            ? EntitlementReasonCode.EnterpriseCapacityReached
            : EntitlementReasonCode.FreeActiveUserLimitReached;

        return Denied(reasonCode, $"Organization has reached the active-user capacity of {limit.Value}.");
    }

    public EntitlementDecision EvaluateActiveUserCount(Models.Offering? offering, int currentActiveUserCount)
    {
        if (offering is null)
        {
            return Denied(EntitlementReasonCode.OfferingNotFound, "No active organization offering found.");
        }

        var limit = offering.PurchasedUserCapacity ?? offering.Code.GetOffering().MaxUserCount;
        if (!limit.HasValue || offering.Code.IsPayAsYouGoOffering() || offering.Code.IsEarlyBirdOffering())
        {
            return Allowed("Organization offering allows additional active users.");
        }

        if (currentActiveUserCount < limit.Value)
        {
            return Allowed("Organization has active-user capacity available.");
        }

        var reasonCode = offering.Code.IsEnterpriseOffering()
            ? EntitlementReasonCode.EnterpriseCapacityReached
            : EntitlementReasonCode.FreeActiveUserLimitReached;

        return Denied(reasonCode, $"Organization has reached the active-user capacity of {limit.Value}.");
    }

    public EntitlementDecision EvaluateTeamCreation(Models.Offering? offering, int currentTeamCount)
    {
        if (offering is null)
        {
            return Denied(EntitlementReasonCode.OfferingNotFound, "No active organization offering found.");
        }

        if (offering.Code.IsEarlyBirdOffering())
        {
            return Allowed("Early Bird organizations have unlimited team capacity.");
        }

        var limit = offering.PurchasedTeamCapacity ?? offering.Code.GetOffering().MaxTeamCount;
        if (!limit.HasValue || currentTeamCount < limit.Value)
        {
            return Allowed("Organization has team capacity available.");
        }

        return Denied(EntitlementReasonCode.FreeTeamLimitReached, $"Organization has reached the team capacity of {limit.Value}.");
    }

    public EntitlementDecision EvaluateLocationCreation(Models.Offering? offering, int currentLocationCount)
    {
        if (offering is null)
        {
            return Denied(EntitlementReasonCode.OfferingNotFound, "No active organization offering found.");
        }

        if (offering.Code.IsEarlyBirdOffering())
        {
            return Allowed("Early Bird organizations have unlimited location capacity.");
        }

        var limit = offering.PurchasedLocationCapacity ?? offering.Code.GetOffering().MaxLocationCount;
        if (!limit.HasValue || currentLocationCount < limit.Value)
        {
            return Allowed("Organization has location capacity available.");
        }

        return Denied(EntitlementReasonCode.FreeLocationLimitReached, $"Organization has reached the location capacity of {limit.Value}.");
    }

    private EntitlementDecision Allowed(string message) => new(true, EntitlementReasonCode.Allowed, message);

    private EntitlementDecision Denied(EntitlementReasonCode reasonCode, string message) => new(false, reasonCode, message);
}
