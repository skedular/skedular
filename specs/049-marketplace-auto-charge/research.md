# Research: Automatic Marketplace Renewal Charging

## Decision: Keep payment authorization purchase-specific

**Decision**: Capture consent and a Stripe-side reusable credential during the initial or fallback checkout for that marketplace purchase. Persist its non-sensitive Stripe references and consent/audit state against that purchase. Never read, select, replace, or fall back to the existing customer-profile payment method.

**Rationale**: It honors the clarified product boundary while leaving card handling to Stripe. A Stripe Customer/PaymentMethod association can exist as a technical Stripe requirement for off-session collection, but it must not be represented as, or sourced from, Skedular’s customer-profile payment-method feature.

**Alternatives considered**:

- Reuse customer-profile cards: rejected; violates purchase-specific consent.
- Create a local card vault: rejected; Stripe owns card data and authorization.

## Decision: Use one off-session PaymentIntent per renewal cycle

**Decision**: A renewal cycle owns one idempotent PaymentIntent identity. Its success/failure/cancellation/action-required outcome is reconciled through local authoritative state and append-only events before reservation access or credit allocation occurs.

**Rationale**: PaymentIntents model the actual charge and asynchronous outcomes without transferring Skedular’s renewal, entitlement, refund, or history ownership to Stripe.

**Alternatives considered**:

- Repeated hosted Checkout: retained as fallback, rejected as the primary renewal path.
- Stripe Billing subscriptions: not selected by default; custom membership terms, current pricing, resource materialization, entitlement validity, cancellation/refund, and Connect behavior would need proof before delegating lifecycle ownership.

## Decision: Use Stripe catalog records only for exact matches

**Decision**: Reuse the existing Stripe Product/Price mapping for a pricing option only when it exactly represents the renewed commercial amount, currency, tax behavior, and charge context. Use a calculated PaymentIntent amount for dynamic/arrears/tax/Connect cases. Do not create a Product or Price per renewal.

**Rationale**: The existing repository already uses persisted Price IDs for standard reservation and entitlement Checkout and inline `price_data` for arrears/host paths. Stripe describes Products/Prices as catalog resources and supports inline transaction-specific pricing; an inline or dynamic price is not a reusable catalog record.

**Alternatives considered**:

- New catalog objects per renewal: rejected; catalog noise with no idempotency benefit.
- Force all dynamic values through a static Price: rejected; incorrect for calculated tax/arrears/current-price/Connect cases.

## Decision: Preserve Connect charge type and validate ownership

**Decision**: Keep direct and destination charge behavior as currently selected. Before automatic charging, validate account context, credential scope, customer ownership, tax semantics, and refund/webhook lookup for the selected path. Any ambiguity results in no charge/no grant plus fallback/manual recovery.

**Rationale**: Stripe scopes direct charges to the connected account and destination charges to the platform. Credential reuse cannot be assumed across account contexts.

## Decision: Keep local lifecycle history authoritative

**Decision**: Persist cycles, attempts, authorization status, fallback recovery, and append-only lifecycle events locally. Stripe IDs/events reconcile state but cannot manufacture paid access, credits, history, cancellation, or refund outcomes.

**Rationale**: This satisfies the repository’s authoritative-history and domain-ownership rules, while allowing retries, delayed webhooks, and manual recovery to remain auditable.

## Sources

- [Stripe Products and Prices](https://docs.stripe.com/products-prices/how-products-and-prices-work)
- [Stripe Checkout future payments](https://docs.stripe.com/payments/checkout/save-and-reuse)
- [Stripe save details during payment](https://docs.stripe.com/payments/save-during-payment)
- [Stripe Setup Intents](https://docs.stripe.com/payments/setup-intents)
- [Stripe destination charges](https://docs.stripe.com/connect/destination-charges)
- [Stripe Connect charge types](https://docs.stripe.com/connect/charges)
