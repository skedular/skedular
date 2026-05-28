# Organization Jobs Project Notes

This file applies to `organization/jobs/Organization.Jobs`.

## Purpose

- This is the runnable job host for the organization domain.
- Read the parent `organization/jobs/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `organization/shared/Organization.Shared`.
