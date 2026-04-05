# MsTeams Shared Agent Notes

This file covers `msteams/shared/`.

## Agent Rule

- External contract changes here can break production integrations quickly, so keep edits conservative.
- Replicated organization, location, and team entities are part of Azure-tenant and Teams-channel routing and update
  targeting.
- Do not remove those replicas unless the Microsoft Teams routing model is redesigned.

## Workflow ID Rule

- MsTeams Temporal workflow IDs belong in `msteams/shared/MsTeams.Shared/Services/WorkflowIdService.cs`.
- Keep tenant re-sync workflow ID formatting centralized there.

## Workflow ID Test Shape

- Keep MsTeams workflow ID unit tests split one class/file per `WorkflowIdService` method under
  `MsTeams.Shared.UnitTests/Services/WorkflowIdServiceTests`.
- In MsTeams unit tests, keep frozen/injected constructor dependencies before `sut`, and keep random inputs after
  `sut`.
