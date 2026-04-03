namespace Api.Shared.Services.Models;

/// <summary>
/// Controls whether Skedular should route invoice creation and delivery through Xero for an organization.
/// Stripe still remains the card payment processor when card payments are used.
/// The database stores the string constants, while the application, GraphQL, and UI use this enum.
/// </summary>
public enum OrganizationXeroBillingMode
{
    /// <summary>
    /// Xero is connected but not used for invoicing. Skedular keeps using its local invoice flow.
    /// </summary>
    Disabled,

    /// <summary>
    /// Xero owns invoicing for all supported invoiceable flows. This includes bank-transfer invoices
    /// and invoices for card-paid flows, while Stripe still handles the card payment collection itself.
    /// </summary>
    Enabled
}

public static class XeroBillingModeConstants
{
    public const string Disabled = "DISABLED";
    public const string Enabled = "ENABLED";
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
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToOrganizationXeroBillingModeName() =>
            src switch
            {
                OrganizationXeroBillingMode.Disabled => "Disabled",
                OrganizationXeroBillingMode.Enabled => "Enabled",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string src)
    {
        public OrganizationXeroBillingMode ToOrganizationXeroBillingMode() =>
            src switch
            {
                XeroBillingModeConstants.Disabled => OrganizationXeroBillingMode.Disabled,
                XeroBillingModeConstants.Enabled => OrganizationXeroBillingMode.Enabled,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
