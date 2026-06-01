# Contract: Observability

## Required Structured Log Events

Implementation must plan structured logs for:

| Event                                    | Trigger                                     | Required Context                                                                              |
| ---------------------------------------- | ------------------------------------------- | --------------------------------------------------------------------------------------------- |
| `AggregateMarketplaceDiscoveryStarted`   | No-subdomain discovery loads                | request/correlation id, signed-in state, filter presence                                      |
| `AggregateMarketplaceDiscoveryCompleted` | Discovery query resolves                    | request/correlation id, eligible location count, empty-state flag                             |
| `AggregateMarketplaceLocationSelected`   | Customer opens an aggregate location        | request/correlation id, location id, organization id                                          |
| `CustomerPurchaseHubLoaded`              | Customer bookings/subscriptions hub loads   | request/correlation id, customer id hash or safe surrogate, booking count, subscription count |
| `CustomerSelfServiceActionStarted`       | Customer attempts cancel/change/refund      | request/correlation id, action type, purchase type, purchase id safe surrogate                |
| `CustomerSelfServiceActionRejected`      | Action is unavailable or policy-blocked     | request/correlation id, action type, purchase type, safe reason code                          |
| `UnsupportedWebappPathHandled`           | Removed/unsupported path resolves in place  | request/correlation id, path category, owner classification if known                          |
| `OwnerSpecificMarketplaceEntryResolved`  | Custom-subdomain marketplace entry resolves | request/correlation id, custom-domain flag, entry point type                                  |

## Privacy Rules

- Do not log raw customer names, email addresses, full addresses, payment details, invoice contents, or refund amounts unless an existing domain logging convention explicitly permits a safe form.
- Prefer stable safe identifiers, counts, state names, and reason codes.
- Include correlation context for request and customer action troubleshooting.

## Verification

- Tests should verify key logging side effects for route resolution, unsupported path handling, and customer self-service action decisions when implementation changes behavior.
- Manual QA should confirm user-facing error states pair with actionable warning/error logs where failures occur.
