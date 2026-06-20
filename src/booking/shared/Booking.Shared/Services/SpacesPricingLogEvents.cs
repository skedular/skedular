namespace Booking.Shared.Services;

/// <summary>
///     Structured log event IDs for Spaces pricing and quota decisions.
///     Used with ILogger to provide consistent logging across the feature.
/// </summary>
public static class SpacesPricingLogEvents
{
    // Quota decision events (1000-1099)
    public const int QuotaCheckStarted = 1000;
    public const int QuotaDecisionWithinQuota = 1001;
    public const int QuotaDecisionFreeTierExceeded = 1002;
    public const int QuotaDecisionPaidTierExceeded = 1003;
    public const int QuotaDecisionCustomCapacityExceeded = 1004;
    public const int QuotaDecisionNoOfferingState = 1005;
    public const int QuotaCheckOutOfPeriodExcluded = 1006;

    // Usage increment events (1100-1199)
    public const int UsageIncrementStarted = 1100;
    public const int UsageIncrementSuccess = 1101;
    public const int UsageIncrementFailedQuotaExceeded = 1102;
    public const int UsageDecrementStarted = 1103;
    public const int UsageDecrementSuccess = 1104;

    // Subscription events (1200-1299)
    public const int SubscriptionCreated = 1200;
    public const int SubscriptionUpdated = 1201;
    public const int OfferingRolloverStarted = 1202;
    public const int OfferingRolloverCompleted = 1203;
    public const int OfferingRolloverFailed = 1204;

    // Recurring booking events (1300-1399)
    public const int RecurringBookingQuotaCheckStarted = 1300;
    public const int RecurringBookingInstanceAllowed = 1301;
    public const int RecurringBookingInstanceBlocked = 1302;

    // Error events (1400-1499)
    public const int QuotaServiceError = 1400;
    public const int UsageRepositoryError = 1401;
    public const int OfferingServiceError = 1402;
}
