# Contract: Webapp Capability Inventory

## Purpose

Defines the planning and review contract for classifying existing webapp routes, navigation items, and major workflows before cleanup removes or hides administration functionality.

## Inventory Coverage

The inventory MUST include:

- Root and public discovery routes.
- Existing marketplace routes for locations, products, bookings, and subscriptions.
- Custom-subdomain customer-facing entry points.
- Private organization, MS Teams, coworking-owner, resource, booking-management, subscription-management, user-management, and admin routes currently present in webapp.
- Shared account, authentication, callback, notification, upload, and integration entry points that may remain cross-product dependencies.

## Required Record Shape

Each inventory item MUST contain:

| Field                  | Required | Description                                                                                                                         |
| ---------------------- | -------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| `id`                   | Yes      | Stable identifier for review and tasks.                                                                                             |
| `pathPattern`          | Yes      | Route or workflow pattern.                                                                                                          |
| `label`                | Yes      | Human-readable capability name.                                                                                                     |
| `currentSurface`       | Yes      | Current webapp route, navigation area, or entry point.                                                                              |
| `audience`             | Yes      | Visitor, customer, administrator, coworking owner, or shared account audience.                                                      |
| `workflowType`         | Yes      | Discovery, marketplace purchase, customer self-service, private booking, resource management, admin, account, or other.             |
| `ownerApp`             | Yes      | `webapp`, `webapp-teams`, `webapp-spaces`, `shared-entry-point`, or `undecided`.                                                    |
| `disposition`          | Yes      | `keep`, `move`, `remove-from-navigation`, `preserve-shared`, `protect-unchanged`, or `defer`.                                       |
| `customerImpact`       | Yes      | Plain-language impact on visitors and customers.                                                                                    |
| `adminImpact`          | Yes      | Plain-language impact on administrators or coworking owners.                                                                        |
| `hasCustomerOwnedData` | Yes      | Whether hiding/removing the capability affects customer bookings, subscriptions, invoices, refunds, or profile state.               |
| `urlHandling`          | Yes      | `serve-in-place`, `unavailable-in-place`, `preserve-existing`, or `not-applicable`; redirect handling is not allowed in this phase. |
| `rationale`            | Yes      | Why this owner/disposition is correct.                                                                                              |
| `approvalStatus`       | Yes      | `draft`, `reviewed`, `approved`, or `blocked`.                                                                                      |

## Ownership Rules

- Webapp owns no-subdomain aggregate marketplace discovery, customer-facing marketplace purchase paths, customer booking/subscription self-service, shared account entry points, and customer-safe unsupported path states.
- Webapp-teams owns private organization booking creation, coworking-space owner booking management, subscription management, resource management, and private organization administration.
- Webapp-spaces owns co-working space administration workflows that are not customer marketplace purchase flows.
- Existing custom-subdomain owner-specific customer marketplace behavior is protected and must not be changed by this feature.

## URL Handling Rules

- No webapp URL redirects are allowed in this phase.
- Removed or unsupported webapp paths MUST resolve in place.
- In-place states MUST be customer-safe and must not expose private administration controls.
- Owner-specific marketplace paths opened in webapp MUST NOT redirect to custom-subdomain URLs during this phase.

## Review Gate

Cleanup implementation MUST NOT remove, hide, or change route behavior until the affected inventory records are approved by product and engineering stakeholders.

## Acceptance Checks

- Every current webapp route and major navigation item has one inventory record.
- Every record has exactly one owner app and one disposition.
- Every private/admin workflow has a non-webapp owner unless explicitly preserved as a shared account entry point.
- Every customer-owned data path has a preservation note.
- Every URL handling decision avoids redirects.
