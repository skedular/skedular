# Organization Processors Project Notes

This file applies to `organization/processors/Organization.Processors`.

## Purpose

- This is the runnable processor host for the organization domain.
- Read the parent `organization/processors/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `organization/shared/Organization.Shared`.
