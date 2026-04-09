# Location Jobs Project Notes

This file applies to `location/jobs/Location.Jobs`.

## Purpose

- This is the runnable job host for the location domain.
- Read the parent `location/jobs/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `location/shared/Location.Shared`.
