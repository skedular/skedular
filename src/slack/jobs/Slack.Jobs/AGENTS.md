# Slack Jobs Project Notes

This file applies to `slack/jobs/Slack.Jobs`.

## Purpose

- This is the runnable job host for the slack domain.
- Read the parent `slack/jobs/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `slack/shared/Slack.Shared`.
