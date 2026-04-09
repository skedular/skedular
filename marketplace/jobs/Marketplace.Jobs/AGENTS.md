# Marketplace Jobs Project Notes

This file applies to `marketplace/jobs/Marketplace.Jobs`.

## Purpose

- This is the runnable job host for the marketplace domain.
- Read the parent `marketplace/jobs/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `marketplace/shared/Marketplace.Shared`.
