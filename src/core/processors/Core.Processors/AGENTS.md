# Core Processors Project Notes

This file applies to `core/processors/Core.Processors`.

## Purpose

- This is the runnable processor host for the core domain.
- Read the parent `core/processors/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `core/shared/Core.Shared`.
