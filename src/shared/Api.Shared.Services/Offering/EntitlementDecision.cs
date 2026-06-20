namespace Api.Shared.Services.Offering;

public class EntitlementDecision(bool isAllowed, EntitlementReasonCode reasonCode, string? message = null)
{
    public bool IsAllowed { get; set; } = isAllowed;
    public EntitlementReasonCode ReasonCode { get; set; } = reasonCode;
    public string? Message { get; set; } = message;
}
