# Marketplace Shared Agent Notes

This file covers `marketplace/shared/`.

## Scope

- `marketplace/shared/` owns the core marketplace domain model: products, product versions, pricing options, listings,
  and marketplace-facing purchase/checkout state.
- It is the library consumed by `Marketplace.Api`, `Marketplace.Jobs`, and `Marketplace.Processors`.

## Product and Pricing Model

- A `Product` is the top-level marketplace offering.
- `ProductVersion` is an immutable snapshot of a product at a point in time; pricing is always loaded from a version.
- `PricingOption` describes the price, billing cadence, and purchase type for a product version.
- Booking loads `ProductVersion → Product → Organization` to resolve pricing context, tax settings, and billing cycle
  for invoicing and Stripe checkout.
- Do not change pricing-option shape or billing-cadence conventions without checking booking's consumption in
  `booking/shared/Booking.Shared/`.

## Replication Boundary

- Auth-critical replicated organization, organization-member, customer, and customer-identity state is kept here to
  support local access checks for marketplace entities.
- Do not remove those replicas unless the marketplace authorization model is explicitly redesigned.
- Product or listing projections that are purely derived (not auth-critical) may be candidates for workflow-driven
  rebuilds, but auth-critical replicas are not.

## Temporal / Workflow ID Rule

- Marketplace Temporal workflow IDs belong in `marketplace/shared/Marketplace.Shared/Services/WorkflowIdService.cs`.
- Do not rebuild marketplace workflow IDs inline in Temporal services, outbox services, or tests.
- Keep workflow ID unit tests split one class/file per method under
  `Marketplace.Shared.UnitTests/Services/WorkflowIdServiceTests`.
- In marketplace unit tests, keep frozen/injected constructor dependencies before `sut`, and keep random inputs after
  `sut`.

## Agent Rule

- Be careful with pricing and listing shape changes because booking often consumes them indirectly.
- Keep auth-critical replicated organization/customer state unless a change explicitly redesigns the authorization or
  ownership model.
- Prefer small, backward-compatible changes here because downstream booking and checkout behavior depends on the
  product/pricing contract.
