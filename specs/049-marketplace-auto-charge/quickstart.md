# Quickstart: Validate Stripe-Billed Marketplace Auto-Renewal

For an auto-renewable purchase, verify that its recipient connected account has one reusable Stripe Product for the offer version and a recurring Stripe Price mapped to the exact purchased pricing snapshot. Intermediate pricing edits that are never purchased must not create a Stripe Price.

1. Create an eligible AutoRenew reservation or entitlement offer with a connected-account recurring Stripe Price for its membership term.
2. Complete initial hosted Checkout in subscription mode for the recipient connected account. Confirm purchase-specific Stripe correlation fields and a history event exist; confirm no Customer-profile payment method was consulted.
3. Deliver a connected-account `invoice.paid` webhook for the initial/renewal period. Confirm exactly one local reservation term or entitlement/credit allocation is created.
4. Replay that webhook and race it with the applicable reconciliation workflow. Confirm no duplicate grant.
5. Deliver `invoice.payment_action_required`, `invoice.payment_failed`, and disconnected-account events. Confirm no grant, persisted history, notification/recovery action, and no local retry PaymentIntent.
6. Complete Stripe recovery and deliver the later `invoice.paid`; confirm one grant.
7. Disable AutoRenew, test immediate cancellation and period-end cancellation, and confirm Stripe/local future-grant behavior matches the selected policy.
8. Test legacy AutoRenew without purchase-specific Stripe Subscription correlation. Confirm explicit migration checkout is required and the Customer-profile card is not used.
