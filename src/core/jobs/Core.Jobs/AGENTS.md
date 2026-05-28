# Core Jobs Project Notes

This file applies to `core/jobs/Core.Jobs`.

## Purpose

- This is the runnable job host for the core domain.
- Read the parent `core/jobs/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `core/shared/Core.Shared`.
