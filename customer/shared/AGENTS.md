# Customer Shared Agent Notes

This file covers `customer/shared/`.

## Agent Rule

- Preserve stable customer identity semantics and replicated-state assumptions.
- Other domains intentionally replicate customer and identity data to support local authorization and membership-aware access checks.
- Do not propose removing those replicas unless the downstream authorization model is changing too.

## Workflow ID Rule

- Customer Temporal workflow IDs belong in `customer/shared/Customer.Shared/Services/WorkflowIdService.cs`.
- Keep client-secret and event-driven workflow ID rules centralized there instead of repeating `ToId(...)` at call sites.

## Workflow ID Test Shape

- Keep customer workflow ID unit tests split one class/file per `WorkflowIdService` method under
  `Customer.Shared.UnitTests/Services/WorkflowIdServiceTests`.
- In customer unit tests, keep frozen/injected constructor dependencies before `sut`, and keep random inputs after
  `sut`.
