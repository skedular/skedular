using System.Reflection;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.XeroInvoiceServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ShouldPreserveStandardInvoiceTransitionStateShould
{
    [Fact]
    public void Return_True_When_Standard_Invoice_Link_Is_Already_Marked_As_Transition_Required()
    {
        var accountingInvoiceLink = new AccountingInvoiceExportLink
        {
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.StandardInvoice,
            ExportConfigurationState = AccountingInvoiceExportConfigurationStateConstants.TransitionRequired
        };

        Invoke(accountingInvoiceLink).ShouldBeTrue();
    }

    [Fact]
    public void Return_False_When_Transition_State_Is_Not_Transition_Required()
    {
        var accountingInvoiceLink = new AccountingInvoiceExportLink
        {
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.StandardInvoice,
            ExportConfigurationState = AccountingInvoiceExportConfigurationStateConstants.Active
        };

        Invoke(accountingInvoiceLink).ShouldBeFalse();
    }

    [Fact]
    public void Return_False_When_Link_Is_Not_A_Standard_Invoice()
    {
        var accountingInvoiceLink = new AccountingInvoiceExportLink
        {
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            ExportConfigurationState = AccountingInvoiceExportConfigurationStateConstants.TransitionRequired
        };

        Invoke(accountingInvoiceLink).ShouldBeFalse();
    }

    private static bool Invoke(AccountingInvoiceExportLink accountingInvoiceLink) =>
        (bool)(typeof(XeroInvoiceService)
            .GetMethod("ShouldPreserveStandardInvoiceTransitionState", BindingFlags.Static | BindingFlags.NonPublic)!
            .Invoke(null, [accountingInvoiceLink]) ?? false);
}
