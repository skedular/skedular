namespace Api.Shared.Services.Offering;

public class EntitlementDecision
{
    public EntitlementDecision(bool isAllowed, EntitlementReasonCode reasonCode, string? message = null)
    {
        IsAllowed = isAllowed;
        ReasonCode = reasonCode;
        Message = message;
    }

    public bool IsAllowed { get; set; }
    public EntitlementReasonCode ReasonCode { get; set; }
    public string? Message { get; set; }
}
