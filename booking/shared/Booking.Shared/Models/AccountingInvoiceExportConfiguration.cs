namespace Booking.Shared.Models;

public static class AccountingInvoiceExportModeConstants
{
    public const string StandardInvoice = "StandardInvoice";
    public const string RepeatingInvoice = "RepeatingInvoice";
}

public static class AccountingInvoiceExportConfigurationStateConstants
{
    public const string Active = "Active";
    public const string TransitionRequired = "TransitionRequired";
}

public static class XeroRepeatingInvoiceScheduleSourceConstants
{
    public const string OrganizationBillingCycle = "OrganizationBillingCycle";
    public const string PurchaseCadence = "PurchaseCadence";
}
