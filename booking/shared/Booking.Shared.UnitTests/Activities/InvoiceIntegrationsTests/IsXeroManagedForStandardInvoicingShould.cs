using System.Reflection;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using Booking.Shared.Activities;

namespace Booking.Shared.UnitTests.Activities.InvoiceIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class IsXeroManagedForStandardInvoicingShould
{
    [Fact]
    public void Return_True_When_Billing_Mode_Is_Repeating_Invoices()
    {
        var xeroConnection = new XeroConnection
        {
            IsActive = true, HasRefreshToken = true, TenantId = "tenant-1", BillingMode = XeroBillingModeConstants.RepeatingInvoices
        };

        Invoke(xeroConnection).ShouldBeTrue();
    }

    private static bool Invoke(XeroConnection xeroConnection) =>
        (bool)(typeof(InvoiceIntegrations)
            .GetMethod("IsXeroManagedForStandardInvoicing", BindingFlags.Static | BindingFlags.NonPublic)!
            .Invoke(null, [xeroConnection]) ?? false);
}
