# MsTeams Jobs Project Notes

This file applies to `msteams/jobs/MsTeams.Jobs`.

## Purpose

- This is the runnable job host for the msteams domain.
- Read the parent `msteams/jobs/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `msteams/shared/MsTeams.Shared`.
