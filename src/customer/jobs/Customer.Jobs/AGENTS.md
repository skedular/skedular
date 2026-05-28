# Customer Jobs Project Notes

This file applies to `customer/jobs/Customer.Jobs`.

## Purpose

- This is the runnable job host for the customer domain.
- Read the parent `customer/jobs/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `customer/shared/Customer.Shared`.
