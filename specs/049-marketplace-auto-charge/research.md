# Research: Stripe-Billed Marketplace Auto-Renewal

## Decision: Stripe Billing, not local off-session PaymentIntents

Use a Stripe Subscription. Stripe Billing creates the invoice and its PaymentIntent every term, attempts collection, handles SCA/3DS and configured recovery/retries. Skedular consumes webhooks and grants only on `invoice.paid`. This removes the need for local payment-attempt/cycle/retry models.

## Decision: recipient connected account, direct charge

Create the Customer, recurring Price, Checkout Session, and Subscription authenticated as the connected account that receives marketplace money. Stripe documents this as the direct-charge Connect subscription model and identifies `invoice.paid` as the provisioning event. The platform must confirm liability/account-type compatibility before enablement. Percentage platform fees can use `application_fee_percent`; a dynamic fee needs an explicit invoice-level policy.

## Decision: marketplace-only Stripe payment relationship

The customer authorizes Stripe during the initial subscription Checkout. Its Stripe Customer/default payment method is a technical Billing relationship in the connected account. The Customer-domain `StripePaymentMethod` is not read or reused.

## Decision: catalog pricing

Use connected-account recurring Product/Price catalog records. Stripe Prices are immutable. Reuse one Stripe Product per auto-renewable offer version and connected account, and lazily create one `StripePrice` row for each actual recurring Price when that exact local pricing snapshot is first purchased or adopted at renewal. Map `MembershipTerm` to recurring `interval` and `interval_count`; keep one-time and recurring Prices in the same local catalog table. Do not create per-invoice prices or invoice line-item workarounds where a supported recurring Price exists.

Implemented mapping: Daily → `day` × 1; Weekly → `week` × 1; Fortnightly → `week` × 2; Monthly, TwoMonths, Quarterly, FourMonths, FiveMonths, and SixMonths → `month` × 1–6; Yearly → `year` × 1. `NotSet` is rejected and cannot create an AutoRenew Stripe Price.

## Decision: provisioning and failure

`invoice.paid` is the sole grant gate. `invoice.payment_action_required`, `invoice.payment_failed`, finalization failures, and account disconnection create history/notifications/recovery actions but never grant. Webhook event/account/invoice-period identities provide idempotency.

## Decision: recovery surface

Use Stripe Customer Portal for customer recovery. The portal session is created against the purchase-specific connected-account Stripe Customer recorded on the reservation or entitlement aggregate; the Customer-domain `StripePaymentMethod` remains excluded. The portal is a recovery action for payment method/invoice state and does not create a local retry PaymentIntent. Stripe Dashboard portal configuration must prevent customer subscription changes or cancellation unless product policy later approves those actions.

Legacy AutoRenew purchases without purchase-specific subscription correlation do not silently fall back to a Customer-profile payment method. They require an explicit migration checkout that creates the connected-account Billing relationship.

Option B is selected: when the current marketplace price or term changes, the next renewal updates the connected-account Stripe Subscription item with the current recurring Price and applies the renewed marketplace pricing without customer confirmation. If Stripe cannot apply the update, the renewal does not advance locally.

## Open research gates

1. Check installed Stripe.NET API fields for subscription Checkout, recurring price mapping, and invoice event objects.
2. Inventory `MembershipTerm` values and validate exact Stripe interval/count support.
3. Confirm recipient Connect account types and marketplace fee policy.
4. Changed-price policy resolved as option B; recovery surface is Customer Portal.

## Confirmed commercial policy

- Skedular Spaces marketplace subscriptions use direct connected-account charges with no platform commission.
- Skedular Host marketplace subscriptions retain the existing configured commission, currently 5%, using Stripe Billing `application_fee_percent` on the subscription created by Checkout.
- The Host commission is taken from the persisted marketplace booking commission rate; Customer-profile payment methods are not involved.
