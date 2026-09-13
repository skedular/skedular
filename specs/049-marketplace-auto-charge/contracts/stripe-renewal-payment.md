# Stripe Contract: Connected-Account Marketplace Subscription

## Creation

- Request account context: `Stripe-Account: {recipientConnectedAccountId}`.
- Product/Price: reuse one connected-account Product per auto-renewable offer version; lazily create an immutable recurring Price only for an actually-used pricing snapshot and its contract term.
- Initial Checkout: hosted Stripe Checkout with `mode=subscription`, one recurring price, technical local-purchase metadata, and Stripe’s recurring-payment authorization.
- Completion: `checkout.session.completed` supplies the connected-account subscription/customer references used to create/update the local link.

## Ongoing billing

Stripe creates invoices and PaymentIntents and applies its Billing retry/SCA policy. Skedular does not create a separate renewal PaymentIntent.

| Event | Local behavior |
|---|---|
| `invoice.paid` | Deduplicate, record, and grant exactly one reservation term or entitlement. If it precedes Checkout/subscription correlation or local-cycle materialization, resolve correlation from the connected-account Subscription metadata and retry rather than acknowledge an ungranted paid invoice. |
| `invoice.payment_action_required` | Record/notify/recover; no grant. |
| `invoice.payment_failed` | Record/notify/recover; no grant. |
| `invoice.finalization_failed` | Record/block grant and notify appropriate party. |
| `customer.subscription.updated` | Mirror operational status/period/cancellation intent. |
| `customer.subscription.deleted` | End future local grants. |
| `account.application.deauthorized` | Mark link disconnected; no grant. |

Every event is correlated by connected account plus Stripe event/object IDs. The selected Stripe Price is prepared while the prior period is active, before Stripe creates the next invoice. Only a paid invoice controls provisioning.

## Cancellation and fees

AutoRenew changes synchronize to Stripe Subscription cancellation (`cancel_at_period_end` or immediate cancellation as product policy dictates). Direct-charge platform fees require the approved Stripe subscription/invoice fee configuration; this design does not convert to destination charges by default.
