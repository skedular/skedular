# Location Processors Project Notes

This file applies to `location/processors/Location.Processors`.

## Purpose

- This is the runnable processor host for the location domain.
- Read the parent `location/processors/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `location/shared/Location.Shared`.
