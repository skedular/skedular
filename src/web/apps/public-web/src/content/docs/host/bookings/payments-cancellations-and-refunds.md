---
id: host-payments
title: "Payments and refunds"
description: "Set up how you receive payments, understand payment status, and manage refunds when they are required."
product: host
category: payments-and-refunds
slug: payments-and-refunds
articleKind: guide
publicationState: published
evidenceRefs:
  - doc-resources/booking.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds: []
updatedAt: 2026-07-24
---

Host payment setup supports paid Bookings through Stripe Connect. A place can be online without a completed payment connection, but renters cannot complete a paid checkout until the connected account is ready to accept charges.

## Set up payments

Connect a Stripe account from the Host payment setup area. Stripe onboarding collects the business and payout information required for the account. Host shows **Onboarding complete** or **Onboarding required**, along with whether **Charges** and **Payouts** are enabled and whether the account details have been submitted. Complete the Stripe onboarding flow before relying on paid Bookings.

## Understand how renters pay

When a renter makes a paid Booking, Host creates the checkout through Stripe. The Booking shows the resulting payment status and can expose **View Invoice** when an invoice is available. A payment connection is therefore separate from the place being visible online: it controls readiness for paid checkout, not whether the place exists or can be viewed.

## Understand payment status

Host can show these payment statuses on a Booking:

- **Pending**: payment still needs to complete or requires operator attention.
- **Confirmed**: the payment has been accepted for the Booking.
- **Rejected**: the payment was rejected.

Other payment statuses may appear when a checkout expires or when payment is no longer required. Review the Booking details for the current status rather than treating a Booking's lifecycle and payment status as the same thing.

If required payment expires or fails before confirmation, Host releases the affected place capacity and retains the payment outcome. The renter can start a new Booking request, which checks current availability again.

## Payment actions requiring attention

When a Booking requires payment and the current state allows an operator decision, the Booking actions include:

- Successful Stripe checkouts are confirmed automatically; you do not need to use **Confirm Payment** for a completed Stripe checkout.
- Use **Confirm Payment** when a payment remains pending and the Host workflow requires you to confirm that the payment has been received or handled.
- **Reject Payment** records that the pending payment is not accepted; it does not reverse a completed Stripe charge.
- **Make Payment Not Required** removes the payment requirement so the Booking can proceed without a Stripe payment. It does not charge the renter, and it does not mean that money is collected elsewhere by Skedular.

These actions change the Booking's payment state. They do not replace the separate cancellation or refund workflow.

## View invoices

Open **View Invoice** from a Booking when Host has an invoice link available. Invoice availability depends on the Booking and payment workflow, so it is not guaranteed for every Booking.

## Understand cancellations and refund eligibility

Cancellation changes the Booking. The cancellation policy configured for the pricing option determines whether the renter is eligible for a refund and, for a tiered policy, what percentage is refundable. **No Refunds**, **Full Refund Before Cutoff**, and **Tiered Refunds** are configured with the pricing option in [Pricing and availability](/docs/host/pricing-and-availability).

Refund eligibility does not mean that money has already been returned. Cancellation and refund are separate outcomes: cancellation changes the reservation, while a refund records and processes the financial reversal when the supported workflow allows it.

## Process a refund

When a cancelled Booking is eligible for a refund and its payment is confirmed, Skedular creates a refund record and begins the refund workflow. Hosts can review that record in the Host refunds area and use **Queue refund** when the record requires approval of the refundable amount. The amount is calculated from the Booking's cancellation policy. You can reduce the refund amount before queuing it, but you cannot exceed the amount calculated from the cancellation policy.

For a Stripe-paid Booking, Skedular submits the refund through the connected Stripe account. Stripe can leave the refund pending or report it as failed; Host shows the refund's current status. A refund may show **Requested**, **Under review**, **Approved**, **Processing**, **Provider pending**, **Completed**, **Rejected**, **Cancelled**, **Failed**, or **Reconciliation required**. Stripe updates normally arrive within minutes of the provider webhook; Xero processing can take up to one business day. Do not treat an eligible refund as complete until it shows **Completed** or **Completed manually**.

If the refund cannot be completed automatically, Host exposes the relevant reconciliation or bank-transfer follow-up actions in the refund record. A reconciliation resolution records the operator's confirmed outcome and provider reference; it does not silently create a financial correction. Bank-transfer actions record an externally completed transfer and require an approval, sent-transfer reference, and receipt-confirmation sequence.

For Booking details and cancellation or removal actions, continue to [Bookings and renters](/docs/host/bookings-and-renters). For pricing and cancellation-policy setup, use [Pricing and availability](/docs/host/pricing-and-availability).

## Check payment and refund activity

When reviewing a Booking, check its payment status, invoice link when present, whether an operator payment action is available, and any refund status shown in the Host refunds area. Keep the Booking workflow separate from payment and refund status.

## Next step

After understanding payments and refunds, continue to [Managing your listing](/docs/host/managing-your-listing) to keep your place information and operational setup current over time.
