# MsTeams Processors Project Notes

This file applies to `msteams/processors/MsTeams.Processors`.

## Purpose

- This is the runnable processor host for the msteams domain.
- Read the parent `msteams/processors/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `msteams/shared/MsTeams.Shared`.
