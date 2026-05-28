# Team Processors Project Notes

This file applies to `team/processors/Team.Processors`.

## Purpose

- This is the runnable processor host for the team domain.
- Read the parent `team/processors/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `team/shared/Team.Shared`.
