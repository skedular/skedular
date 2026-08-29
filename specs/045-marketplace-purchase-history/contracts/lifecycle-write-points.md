# Lifecycle Write Points

The implementation must append one idempotent event at the authoritative commit point for each transition:

| Transition | Owning write point |
|---|---|
| Purchase creation | Subscription purchase creation or entitlement purchase creation service. |
| Subscription start | Subscription activation/start service or activity. |
| Renewal | Renewal workflow/activity after the renewed cycle is accepted. |
| Cancellation scheduled | Explicit period-end cancellation decision. |
| Cancellation completed | Immediate cancellation or cycle-boundary completion. |
| Entitlement creation | Entitlement grant creation after the purchase is accepted. |
| Entitlement expiration | Entitlement expiry service. |
| Credit consumption | Entitlement ledger consumption transaction. |
| Payment state | Payment confirmation/rejection/expiry/reconciliation transition. |
| Refund state | Refund aggregate transition after its local state is authoritative. |

Each write point calls the shared history service/repository with a deterministic idempotency key derived from the source transition identity. Retries and duplicate processor/workflow deliveries must be safe. Event append and current-snapshot update occur in the same local transaction where the source transition is persisted; downstream provider actions do not create speculative events before local authority exists.
