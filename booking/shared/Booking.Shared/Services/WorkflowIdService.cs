using Booking.Shared.Workflows;
using Enterprise.Shared.Temporal;

namespace Booking.Shared.Services;

public interface IWorkflowIdService
{
    string GenerateLocationResourcesSlots(string locationId);
    string GenerateResourcesSlots(string locationId);
    string PayRecurringBookingViaCard(string recurringBookingId);
    string PayRecurringBookingViaBankTransfer(string recurringBookingId);
    string GenerateInitialArrearsRecurringBookingInvoice(string recurringBookingId);
    string PayBookingViaCard(string bookingId);
    string PayBookingViaBankTransfer(string bookingId);
    string GenerateInitialArrearsBookingInvoice(string bookingId);
    string BookPrivateRecurringResources(string recurringBookingId);
    string BookMarketplaceBookingSubscriptionResources(string marketplaceBookingSubscriptionId);
    string RunOrganizationArrearsBilling(string organizationId);
    string MaintainOrganizationArrearsInvoiceAccountingState(string organizationArrearsInvoiceId);
    string MaintainAccountingInvoiceState(string localEntityType, string localEntityId);
}

public class WorkflowIdService(ITemporalHelperService temporalHelperService) : IWorkflowIdService
{
    public string GenerateLocationResourcesSlots(string locationId) =>
        temporalHelperService.ToId($"{Constants.GenerateLocationResourcesSlotsPrefix}-{locationId}");

    public string GenerateResourcesSlots(string locationId) =>
        temporalHelperService.ToId($"{Constants.GenerateResourcesSlotsPrefix}-{locationId}");

    public string PayRecurringBookingViaCard(string recurringBookingId) =>
        temporalHelperService.ToId($"{Constants.PaidRecurringBookingViaCardPrefix}-{recurringBookingId}");

    public string PayRecurringBookingViaBankTransfer(string recurringBookingId) =>
        temporalHelperService.ToId($"{Constants.PaidRecurringBookingViaBankTransferPrefix}-{recurringBookingId}");

    public string GenerateInitialArrearsRecurringBookingInvoice(string recurringBookingId) =>
        temporalHelperService.ToId($"{Constants.InitialArrearsRecurringBookingInvoicePrefix}-{recurringBookingId}");

    public string PayBookingViaCard(string bookingId) =>
        temporalHelperService.ToId($"{Constants.PaidViaCardPrefix}-{bookingId}");

    public string PayBookingViaBankTransfer(string bookingId) =>
        temporalHelperService.ToId($"{Constants.PaidViaBankTransferPrefix}-{bookingId}");

    public string GenerateInitialArrearsBookingInvoice(string bookingId) =>
        temporalHelperService.ToId($"{Constants.InitialArrearsBookingInvoicePrefix}-{bookingId}");

    public string BookPrivateRecurringResources(string recurringBookingId) =>
        temporalHelperService.ToId(recurringBookingId);

    public string BookMarketplaceBookingSubscriptionResources(string marketplaceBookingSubscriptionId) =>
        temporalHelperService.ToId(marketplaceBookingSubscriptionId);

    public string RunOrganizationArrearsBilling(string organizationId) =>
        temporalHelperService.ToId($"{Constants.OrganizationArrearsBillingPrefix}-{organizationId}");

    public string MaintainOrganizationArrearsInvoiceAccountingState(string organizationArrearsInvoiceId) =>
        temporalHelperService.ToId(
            $"{Constants.MaintainOrganizationArrearsInvoiceAccountingStatePrefix}-{organizationArrearsInvoiceId}");

    public string MaintainAccountingInvoiceState(string localEntityType, string localEntityId) =>
        temporalHelperService.ToId($"{Constants.MaintainAccountingInvoiceStatePrefix}-{localEntityType}-{localEntityId}");
}
