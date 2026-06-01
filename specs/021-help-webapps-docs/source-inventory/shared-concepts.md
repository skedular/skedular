# Shared Concepts Inventory

## Source References

- `specs/009-split-web-products/spec.md` defines the product split:
  - Customer WebApp is the public customer surface.
  - Teams is the private organization surface.
  - Spaces is the marketplace and co-working operator surface.
- `specs/020-customer-landing-cleanup/spec.md` keeps Customer focused on public discovery, booking, subscriptions, and customer self-service.
- Current route trees under `src/web/apps/webapp`, `src/web/apps/webapp-teams`, and `src/web/apps/webapp-spaces` confirm the same split.

## App Boundaries

| Concept | Customer Help | Teams Help | Spaces Help |
| --- | --- | --- | --- |
| Location | A place a customer can browse or book. | A private workplace location an organization manages for internal use. | A marketplace/co-working location an operator can publish and sell. |
| Booking | A customer's reservation or personal booking record. | A private organization booking for internal people and resources. | An operator-managed booking, including marketplace bookings and admin-created bookings. |
| Subscription | A customer's recurring marketplace purchase. | Not a Teams-owned marketplace concept. | A marketplace subscription that an operator manages and supports. |
| Product | A marketplace offer a customer can book or subscribe to. | Out of scope for Teams. | A sellable marketplace offer owned by the operator. |
| Team | Not customer-owned. | A private group used for internal organization access and scheduling. | Not a Spaces-owned private team-management concept. |
| Member/user | A customer's own account in Customer. | A person inside a private organization. | A customer/operator user record viewed from the marketplace operator side. |
| Refund | Customer help explains what a customer can request and what may depend on policy. | Out of scope for Teams. | Spaces help explains operator review and follow-up without exposing internal accounting steps. |
| Payments | Customer help explains visible payment and booking outcomes. | Out of scope unless private booking policy exposes it later. | Spaces help explains setup and operational meaning at a public-safe level. |
| Integrations | Customer only sees customer-facing install/status pages where present. | Slack and Microsoft Teams help private organization workflows. | Slack and Microsoft Teams help marketplace operators work from collaboration tools. |
| Analytics | Not customer-owned. | Private workplace usage and availability analytics. | Marketplace/co-working usage, availability, and commerce analytics. |

## Navigation Naming

- Use short, direct labels in help navigation.
- Prefer the user's word over the implementation word:
  - "Bookings" instead of "booking records"
  - "Locations and resources" instead of "location/resource domain"
  - "Payments" instead of "Stripe Connect accounts"
- Keep app names clear:
  - "Customer Help"
  - "Teams Help"
  - "Spaces Help"

## Public Writing Rules

- Write as public help, not internal implementation notes.
- Explain what the user can do and what result to expect.
- Avoid API names, database fields, secrets, internal fallback procedures, and security setup details.
- Mark unclear flows as content gaps.
- Use American spelling and grammar.
