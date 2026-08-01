# Refund Operations Metrics

The refund operations dashboard consumes the existing `Skedular.Booking.Refunds` OpenTelemetry meter and the refund operations queue. No second persistence or dashboard-specific data store is required.

| Dashboard panel | Source | Metric/query | Alert guidance |
|---|---|---|---|
| Pending refunds | Refund reconciliation service | `refund.queue.provider_pending` and `refund.queue.processing` gauges, plus `refund.reconciliation.result` | Alert when any refund remains beyond the reconciliation threshold or the count grows between two daily runs |
| Failed/reconciliation-required | Refund operations queue | `refund.queue.failed` and `refund.queue.reconciliation_required` gauges | Alert when count is non-zero for one daily run; page when it increases |
| Approved bank transfers not sent | Refund operations service | `refund.queue.overdue_bank_transfer` gauge and `refund.bank_transfer.overdue` counter | Alert after three business days; include refund ID, amount, and organization in the operator queue |
| Cancelled bookings without refund decision | Refund operations repository snapshot | `refund.queue.cancelled_without_decision` gauge; cancelled marketplace bookings with no linked refund decision | Alert when any record is older than the daily reconciliation interval |
| Provider/webhook/reconciliation errors | Structured logs and reconciliation meter | Log events from Stripe/Xero/webhook/reconciliation paths; `refund.reconciliation.result` grouped by provider and status | Alert on error-rate increase or any reconciliation lookup failure; never include credentials or payment secrets |

Required common dimensions are provider, status, and organization where available. Refund IDs, booking IDs, payment references, and correlation IDs remain structured-log fields rather than metric labels to avoid high-cardinality metrics. The existing operations queue is the investigation surface for those identifiers.
