# Team Shared Agent Notes

This file covers `team/shared/`.

## Agent Rule

- Preserve team identity and membership consistency because other domains may rely on these assumptions.
- Do not reintroduce booking replication, booking-derived snapshot tables, or a persisted `HasFutureBooking` flag into team shared state.
- Keep auth-critical replicas such as organization, organization-member, customer, and customer-identity data when team-local authorization depends on them.

## Workflow ID Rule

- Team Temporal workflow IDs belong in `team/shared/Team.Shared/Services/WorkflowIdService.cs`.
- Invitation workflow IDs should not be recreated inline across services and tests.

## Workflow ID Test Shape

- Keep team workflow ID unit tests split one class/file per `WorkflowIdService` method under
  `Team.Shared.UnitTests/Services/WorkflowIdServiceTests`.
- In team unit tests, keep frozen/injected constructor dependencies before `sut`, and keep random inputs after `sut`.
