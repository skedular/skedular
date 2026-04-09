# Slack Domain Test Agent Notes

This file covers `slack/domain/`.

## What This Area Is For

- `Slack.Domain.AppHost` — Aspire app host for the Slack domain.
- `Slack.Domain.FakeDependencies` — Slack-owned fake external dependency host for integration tests.

## Boundary

- Use this area for Slack-domain integration tests covering workspace connection and channel routing.
- For multi-domain scenarios involving booking or org events triggering Slack notifications end to end, use
  `system/Skedular.SystemTests` instead.

## Aspire App Host Rule

- Keep Slack dependency readiness in `Slack.Domain.AppHost/AppHost.cs`.
- All resources referenced by Slack should have matching `WaitFor(...)` or `WaitForCompletion(...)` there rather than
  in test startup.

## Agent Rule

- Use this area for Slack-domain integration tests.
- Use system tests when the scenario spans multiple domains.
- Keep Slack Aspire dependency readiness in `Slack.Domain.AppHost/AppHost.cs`; referenced resources should be paired
  with `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.
