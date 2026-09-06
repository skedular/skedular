using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Xero.NetStandard.OAuth2.Model.Accounting;

namespace Booking.Shared.Services;

public record XeroRepeatingInvoiceScheduleDefinition(
    string Source,
    Schedule.UnitEnum Unit,
    int Period,
    decimal InvoiceAmount);

public interface IXeroRepeatingInvoiceScheduleService
{
    XeroRepeatingInvoiceScheduleDefinition? GetSchedule(
        RecurringBooking recurringBooking,
        MarketplaceBooking marketplaceBooking,
        OrganizationBillingCycle organizationBillingCycle);
}

public class XeroRepeatingInvoiceScheduleService(IRecurringInvoiceBillingScheduleService recurringInvoiceBillingScheduleService)
    : IXeroRepeatingInvoiceScheduleService
{
    public XeroRepeatingInvoiceScheduleDefinition? GetSchedule(
        RecurringBooking recurringBooking,
        MarketplaceBooking marketplaceBooking,
        OrganizationBillingCycle organizationBillingCycle)
    {
        var billingDefinition = recurringInvoiceBillingScheduleService.GetSchedule(recurringBooking, marketplaceBooking, organizationBillingCycle);
        return billingDefinition.MembershipTerm switch
        {
            MembershipTerm.Weekly => new XeroRepeatingInvoiceScheduleDefinition(
                billingDefinition.Source,
                Schedule.UnitEnum.WEEKLY,
                1,
                billingDefinition.InvoiceAmount),
            MembershipTerm.Fortnightly => new XeroRepeatingInvoiceScheduleDefinition(
                billingDefinition.Source,
                Schedule.UnitEnum.WEEKLY,
                2,
                billingDefinition.InvoiceAmount),
            MembershipTerm.Monthly => new XeroRepeatingInvoiceScheduleDefinition(
                billingDefinition.Source,
                Schedule.UnitEnum.MONTHLY,
                1,
                billingDefinition.InvoiceAmount),
            MembershipTerm.TwoMonths => new XeroRepeatingInvoiceScheduleDefinition(
                billingDefinition.Source,
                Schedule.UnitEnum.MONTHLY,
                2,
                billingDefinition.InvoiceAmount),
            MembershipTerm.Quarterly => new XeroRepeatingInvoiceScheduleDefinition(
                billingDefinition.Source,
                Schedule.UnitEnum.MONTHLY,
                3,
                billingDefinition.InvoiceAmount),
            MembershipTerm.FourMonths => new XeroRepeatingInvoiceScheduleDefinition(
                billingDefinition.Source,
                Schedule.UnitEnum.MONTHLY,
                4,
                billingDefinition.InvoiceAmount),
            MembershipTerm.FiveMonths => new XeroRepeatingInvoiceScheduleDefinition(
                billingDefinition.Source,
                Schedule.UnitEnum.MONTHLY,
                5,
                billingDefinition.InvoiceAmount),
            MembershipTerm.SixMonths => new XeroRepeatingInvoiceScheduleDefinition(
                billingDefinition.Source,
                Schedule.UnitEnum.MONTHLY,
                6,
                billingDefinition.InvoiceAmount),
            MembershipTerm.Yearly => new XeroRepeatingInvoiceScheduleDefinition(
                billingDefinition.Source,
                Schedule.UnitEnum.MONTHLY,
                12,
                billingDefinition.InvoiceAmount),
            _ => null,
        };
    }
}
