# Marketplace Processors Project Notes

This file applies to `marketplace/processors/Marketplace.Processors`.

## Purpose

- This is the runnable processor host for the marketplace domain.
- Read the parent `marketplace/processors/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `marketplace/shared/Marketplace.Shared`.
