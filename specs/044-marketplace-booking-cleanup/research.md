# Research: Reliable Marketplace Booking Cleanup

## Decisions

- **Separate local release from external cleanup.** Existing recurring release invokes accounting before deleting generated bookings; local transaction must commit first. Retrying the combined activity would preserve the provider dependency.
- **Use a shared cleanup Temporal contract.** Existing payment and invoice workflows remain responsible for detecting terminal failure, but they delegate cleanup to one idempotent workflow/activity contract. This avoids replacing all workflow routes while preventing duplicated cleanup ordering and retry behavior.
- **Reuse existing failure and reconciliation infrastructure.** `MarketplaceBookingFailure`, failure events/delivery records, repository filters, and `MarketplaceRefundReconciliationHostedService` provide the established seams. Extend them before creating new storage.
- **Resolve effective payment ownership.** A booking may inherit state from a linked subscription or billed owner; booking-local status alone is insufficient. Record the resolved owner for repeatability and audit.
- **Make missing Stripe setup explicit.** Null product, pricing, customer, or session responses must enter durable failure and cleanup rather than silently returning.
- **Use five local retries plus immediate reconciliation.** Local-release activities use at most five delayed/exponential-backoff Temporal retries. Exhaustion creates an immediate reconciliation candidate and recurring reconciliation repairs worker loss for failures created by the new paths. Provider/accounting cleanup is independently retryable after local release and does not share the local-release limit.
- **Treat durable failure records as cleanup evidence.** A rejected or expired effective payment is eligible, as is an invoice-generation, Xero, or Stripe setup terminal failure record when no payment record was created. Pending, confirmed, and no-payment-required cases remain ineligible, and confirmed entitlement always excludes cleanup.
- **Publish truthful UI state.** “Resources released” is valid only after local commit; provider follow-up is represented separately and Relay updates returned state without browser reload.

## Implementation inventory and remaining abstraction

- **Terminal release callers.** One-time and recurring card and bank-transfer workflows call `ReleaseBookingResourcesAsync` and `ReleaseRecurringBookingResourcesAsync` respectively. Initial arrears invoice workflows use the same activities. The subscription resource workflow invokes the recurring release path when it reaches a terminal payment outcome.
- **Committed release boundary.** `BookingIntegrations` and `MarketplaceBookingSubscriptionIntegrations` now commit local slot/allocation release and failure finalization before accounting invoice cancellation. They persist `ResourceReleaseStatus=Released` after that commit and leave accounting state as `Pending` for the follow-up.
- **Failure source without a payment record.** Null Stripe product, pricing, customer, or checkout-session responses are routed to the same terminal release activities, which write a durable marketplace failure even when the payment record was never created.
- **GraphQL/UI consumers.** Booking, recurring booking, and subscription detail resolvers expose `MarketplaceBookingFailureDetails`. The current web consumer is `marketplace-product-booking-details.tsx`; its Relay operations need fragments for the newly added release/accounting fields.
- **Cleanup reconciliation.** `MarketplaceBookingCleanupReconciliationService` now queries durable failures, atomically claims candidates with a lease, records attempts, and enqueues the idempotent `MarketplaceBookingCleanup` workflow from the daily reconciliation host. It covers failures without payment records. Retry exhaustion also enqueues cleanup immediately, while the scheduled pass repairs worker loss for newly created failure records.
