namespace Api.Shared.Services.Models;

/// <summary>
///     Controls whether Skedular should route invoice creation and delivery through Xero for an organization.
///     Stripe still remains the card payment processor when card payments are used.
///     The database stores the string constants, while the application, GraphQL, and UI use this enum.
/// </summary>
public enum OrganizationXeroBillingMode
{
    /// <summary>
    ///     Xero is connected but not used for invoicing. Skedular keeps using its local invoice flow.
    /// </summary>
    Disabled,

    /// <summary>
    ///     Xero owns invoicing for all supported invoiceable flows. This includes bank-transfer invoices
    ///     and invoices for card-paid flows, while Stripe still handles the card payment collection itself.
    /// </summary>
    Enabled,

    /// <summary>
    ///     Xero owns recurring invoice template creation for supported recurring booking flows. For recurring
    ///     in-arrears bookings, the repeating schedule follows the organization's billing cycle. For other recurring
    ///     bookings, the repeating schedule follows the recurring purchase cadence.
    /// </summary>
    RepeatingInvoices
}

public static class XeroBillingModeConstants
{
    public const string Disabled = "DISABLED";
    public const string Enabled = "ENABLED";
    public const string RepeatingInvoices = "REPEATING_INVOICES";
}

public static class OrganizationXeroBillingModeExtensions
{
    extension(OrganizationXeroBillingMode src)
    {
        public string ToOrganizationXeroBillingMode() =>
            src switch
            {
                OrganizationXeroBillingMode.Disabled => XeroBillingModeConstants.Disabled,
                OrganizationXeroBillingMode.Enabled => XeroBillingModeConstants.Enabled,
                OrganizationXeroBillingMode.RepeatingInvoices => XeroBillingModeConstants.RepeatingInvoices,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case.")
            };

        public string ToOrganizationXeroBillingModeName() =>
            src switch
            {
                OrganizationXeroBillingMode.Disabled => "Disabled",
                OrganizationXeroBillingMode.Enabled => "Enabled",
                OrganizationXeroBillingMode.RepeatingInvoices => "Repeating Invoices",
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case.")
            };
    }

    extension(string src)
    {
        public OrganizationXeroBillingMode ToOrganizationXeroBillingMode() =>
            src switch
            {
                XeroBillingModeConstants.Disabled => OrganizationXeroBillingMode.Disabled,
                XeroBillingModeConstants.Enabled => OrganizationXeroBillingMode.Enabled,
                XeroBillingModeConstants.RepeatingInvoices => OrganizationXeroBillingMode.RepeatingInvoices,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case.")
            };
    }
}
