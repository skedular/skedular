# NATS Module

Register connections through `AddNats` and consumers through `AddNatsReliableEventConsumer` or
`AddNatsNonReliableEventConsumer`. Keep subject names and retry metadata in `Api.Shared.Events.Nats`.

## Delivery

- Acknowledge only after handling succeeds or the next retry/dead-letter publish succeeds.
- Retry subjects advance in order; a retry consumer must not restart at the first retry.
- A retry consumer's acknowledgment lease includes its scheduled delay plus the ordinary processing window.
- With zero retries, failed reliable messages go directly to the dead-letter subject.
- Failed forwarding, non-reliable handling, and dead-letter handling are negatively acknowledged for redelivery.
  A dead-letter handler failure must not forward back into the retry chain or stop the host.
- `Nats:RedeliveryDelay` defaults to five seconds and must be nonnegative. Zero requests immediate redelivery.
- Propagate shutdown cancellation without forwarding or acknowledging the interrupted message.
