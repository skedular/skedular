# Research: Admin Cancellation Policy Override

## Decision 1: Use an explicit server-side cancellation actor

- **Decision**: Resolve cancellation source from authenticated customer identity and product-owning organization permissions. Do not accept a client-provided bypass flag as authority.
- **Rationale**: The current booking path already has a policy-bypass concept, but the cancellation source must be explicit and authoritative for both booking and subscription flows. This prevents customers from impersonating operators and gives audit records a stable actor category.
- **Alternatives considered**: Keep inferring operator behavior from a nullable customer parameter; rejected because it is ambiguous and does not cleanly represent authenticated owner/admin actions.

## Decision 2: Authorize only the product-owning organization

- **Decision**: Grant override authority to the product-owning Spaces or Host organization, limited to owners and administrators with existing booking/subscription management permission.
- **Rationale**: This creates a clear commercial and security boundary and follows least privilege without introducing a new role hierarchy.
- **Alternatives considered**: Any organization involved in the booking; rejected because participation does not imply authority over the product's commercial policy. All organization administrators; rejected because it exceeds existing management permissions.

## Decision 3: Override cancellation eligibility, not refund policy

- **Decision**: An authorized operator can cancel when customer policy conditions fail, but refund eligibility and amount remain separately calculated.
- **Rationale**: Cancellation ends entitlement; refund is a separate financial decision. This preserves the existing refund ownership and payment-state rules.
- **Alternatives considered**: Full or policy-derived automatic refund for every override; rejected because it would conflate operational cancellation authority with financial approval.

## Decision 4: Preserve provider-specific refund processing

- **Decision**: A cancellation creates a refund request when existing payment/refund rules require one. Eligible Stripe refunds continue automatic processing. Bank-transfer refunds remain pending owner/admin approval and transfer confirmation. Xero refunds remain subject to owner/admin approval and the existing Xero processing/reconciliation path.
- **Rationale**: The cancellation override must not bypass established financial controls. Provider behavior is already represented in Booking refund automation and administrative services.
- **Alternatives considered**: Make all override refunds automatic; rejected because it would bypass bank-transfer and Xero approval controls.

## Decision 5: Require an override reason and durable audit outcome

- **Decision**: Require a short reason for every administrative policy override and record actor, organization, mode, policy result, reason, and outcome.
- **Rationale**: Operator cancellations can affect customer entitlement and financial follow-up; support and accounting need an explainable history.
- **Alternatives considered**: Optional or no reason; rejected because it weakens operational accountability.

## Existing code alignment

- Booking cancellation already evaluates policy through `MarketplaceBookingService.DeleteAsync` and passes a policy-bypass decision into refund calculation.
- Subscription cancellation validates customer cancellation eligibility in `MarketplaceBookingSubscriptionService` and separately creates immediate-cancellation refunds.
- GraphQL mutations currently expose booking and subscription deletion paths; operator context and override reason need to be represented at this boundary without leaking GraphQL types into shared services.
- Existing `MarketplaceRefundAdminService`, Stripe refund automation, bank-transfer refund mutations, and Xero refund services provide the provider-specific approval and settlement behavior to preserve.
