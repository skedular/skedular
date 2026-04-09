# Team Jobs Project Notes

This file applies to `team/jobs/Team.Jobs`.

## Purpose

- This is the runnable job host for the team domain.
- Read the parent `team/jobs/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `team/shared/Team.Shared`.
