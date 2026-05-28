# Slack Shared Agent Notes

This file covers `slack/shared/`.

## Agent Rule

- Preserve stable integration semantics and avoid unnecessary behavioral drift.
- Replicated organization, location, and team entities are part of Slack workspace/channel routing and update targeting.
- Do not remove those replicas unless the Slack routing model is redesigned.

## Workflow ID Rule

- Slack Temporal workflow IDs belong in `slack/shared/Slack.Shared/Services/WorkflowIdService.cs`.
- Keep Slack workspace workflow ID formatting centralized there.

## Workflow ID Test Shape

- Keep Slack workflow ID unit tests split one class/file per `WorkflowIdService` method under
  `Slack.Shared.UnitTests/Services/WorkflowIdServiceTests`.
- In Slack unit tests, keep frozen/injected constructor dependencies before `sut`, and keep random inputs after `sut`.
