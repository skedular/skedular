# Slack Domain Test Agent Notes

This file covers `slack/domain/`.

## Agent Rule

- Use this area for Slack-domain integration tests.
- Use system tests when the scenario spans multiple domains.
- Keep Slack Aspire dependency readiness in `Slack.Domain.AppHost/AppHost.cs`; referenced resources should be paired
  with `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.
