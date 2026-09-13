# Marketplace Renewal Event Contract

Persist an append-only local event for each accepted Stripe/local fact. Required correlation: local purchase, connected account, Stripe event ID where provider-originated, subscription ID, invoice ID/period, and local grant ID where applicable.

| Event family | Grant allowed? |
|---|---:|
| Subscription created/updated/cancelled | No |
| Invoice paid | Yes, exactly once |
| Invoice action required/failed/finalization failed | No |
| Connected account disconnected | No |
| Reservation term granted | Already gated by paid invoice |
| Entitlement/credit allocation granted | Already gated by paid invoice |
| Cancellation/refund/recovery | Never itself grants |

History UI reads these persisted records rather than aggregate subscription/payment fields.
