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
    string RolloverSpacesBookingUsage();
    string ExpireEntitlements();
    string PrepareEntitlementRenewal(string entitlementId);
    string MaintainOrganizationArrearsInvoiceAccountingState(string organizationArrearsInvoiceId);
    string MaintainAccountingInvoiceState(string localEntityType, string localEntityId);
    string NotifyMarketplaceBookingFailure(string failureId);
    string NotifyMarketplaceBookingModification(string modificationId);
    string ProcessMarketplaceRefund(string refundId);
    string ResolvePartialMarketplaceBooking(string failureId);
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

    public string RolloverSpacesBookingUsage() =>
        temporalHelperService.ToId(Constants.SpacesBookingUsageRolloverPrefix);

    public string ExpireEntitlements() =>
        temporalHelperService.ToId(Constants.ExpireEntitlementsPrefix);

    public string PrepareEntitlementRenewal(string entitlementId) =>
        temporalHelperService.ToId($"{Constants.PrepareEntitlementRenewalPrefix}-{entitlementId}");

    public string MaintainOrganizationArrearsInvoiceAccountingState(string organizationArrearsInvoiceId) =>
        temporalHelperService.ToId(
            $"{Constants.MaintainOrganizationArrearsInvoiceAccountingStatePrefix}-{organizationArrearsInvoiceId}");

    public string MaintainAccountingInvoiceState(string localEntityType, string localEntityId) =>
        temporalHelperService.ToId($"{Constants.MaintainAccountingInvoiceStatePrefix}-{localEntityType}-{localEntityId}");

    public string NotifyMarketplaceBookingFailure(string failureId) =>
        temporalHelperService.ToId($"notify-marketplace-booking-failure-{failureId}");

    public string NotifyMarketplaceBookingModification(string modificationId) =>
        temporalHelperService.ToId($"notify-marketplace-booking-modification-{modificationId}");

    public string ProcessMarketplaceRefund(string refundId) =>
        temporalHelperService.ToId($"process-marketplace-refund-{refundId}");

    public string ResolvePartialMarketplaceBooking(string failureId) =>
        temporalHelperService.ToId($"marketplace-booking-partial-resolution:{failureId}");
}
