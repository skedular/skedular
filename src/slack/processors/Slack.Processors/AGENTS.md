# Slack Processors Project Notes

This file applies to `slack/processors/Slack.Processors`.

## Purpose

- This is the runnable processor host for the slack domain.
- Read the parent `slack/processors/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `slack/shared/Slack.Shared`.
