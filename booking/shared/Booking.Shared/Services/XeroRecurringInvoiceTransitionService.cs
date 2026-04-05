using Booking.Shared.Database.Entities;
using Booking.Shared.Models;

namespace Booking.Shared.Services;

public enum XeroRecurringInvoiceExportPath
{
    StandardInvoice,
    RepeatingInvoice,
    FreezeExistingRepeatingInvoice
}

public record XeroRecurringInvoiceTransitionDecision(
    XeroRecurringInvoiceExportPath Path,
    string ConfigurationState,
    string? ConfigurationMessage);

public interface IXeroRecurringInvoiceTransitionService
{
    XeroRecurringInvoiceTransitionDecision Decide(
        AccountingInvoiceLink? existingLink,
        bool useRepeatingInvoices,
        XeroRepeatingInvoiceScheduleDefinition? desiredSchedule);
}

public class XeroRecurringInvoiceTransitionService : IXeroRecurringInvoiceTransitionService
{
    public XeroRecurringInvoiceTransitionDecision Decide(
        AccountingInvoiceLink? existingLink,
        bool useRepeatingInvoices,
        XeroRepeatingInvoiceScheduleDefinition? desiredSchedule)
    {
        var hasExistingExternalInvoice = existingLink is not null && !string.IsNullOrWhiteSpace(existingLink.ExternalInvoiceId);
        var existingMode = existingLink?.ExternalInvoiceMode;
        if (existingMode == AccountingInvoiceExportModeConstants.RepeatingInvoice && hasExistingExternalInvoice)
        {
            if (!useRepeatingInvoices)
            {
                return new XeroRecurringInvoiceTransitionDecision(
                    XeroRecurringInvoiceExportPath.FreezeExistingRepeatingInvoice,
                    AccountingInvoiceExportConfigurationStateConstants.TransitionRequired,
                    "Existing recurring Xero repeating invoice remains active until it is migrated manually.");
            }

            if (desiredSchedule is null)
            {
                return new XeroRecurringInvoiceTransitionDecision(
                    XeroRecurringInvoiceExportPath.FreezeExistingRepeatingInvoice,
                    AccountingInvoiceExportConfigurationStateConstants.TransitionRequired,
                    "Existing recurring Xero repeating invoice remains active because the current cadence can no longer be represented as a repeating template.");
            }

            if (!string.Equals(existingLink!.RepeatingScheduleSource, desiredSchedule.Source, StringComparison.Ordinal) ||
                !string.Equals(existingLink.RepeatingScheduleUnit, desiredSchedule.Unit.ToString(), StringComparison.Ordinal) ||
                existingLink.RepeatingSchedulePeriod != desiredSchedule.Period)
            {
                return new XeroRecurringInvoiceTransitionDecision(
                    XeroRecurringInvoiceExportPath.FreezeExistingRepeatingInvoice,
                    AccountingInvoiceExportConfigurationStateConstants.TransitionRequired,
                    "Existing recurring Xero repeating invoice schedule differs from the current settings and requires manual migration.");
            }

            return new XeroRecurringInvoiceTransitionDecision(
                XeroRecurringInvoiceExportPath.RepeatingInvoice,
                AccountingInvoiceExportConfigurationStateConstants.Active,
                null);
        }

        return useRepeatingInvoices switch
        {
            true when hasExistingExternalInvoice => new XeroRecurringInvoiceTransitionDecision(
                XeroRecurringInvoiceExportPath.StandardInvoice,
                AccountingInvoiceExportConfigurationStateConstants.TransitionRequired,
                "Existing recurring invoice export remains on standard Xero invoices until it is migrated manually."),
            true when desiredSchedule is not null => new XeroRecurringInvoiceTransitionDecision(
                XeroRecurringInvoiceExportPath.RepeatingInvoice,
                AccountingInvoiceExportConfigurationStateConstants.Active,
                null),
            _ => new XeroRecurringInvoiceTransitionDecision(
                XeroRecurringInvoiceExportPath.StandardInvoice,
                AccountingInvoiceExportConfigurationStateConstants.Active,
                null)
        };
    }
}
