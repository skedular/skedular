# Customer Processors Project Notes

This file applies to `customer/processors/Customer.Processors`.

## Purpose

- This is the runnable processor host for the customer domain.
- Read the parent `customer/processors/AGENTS.md` for all rules governing this host.

## Agent Rule

- Keep `Program.cs` / `Extensions.cs` focused on host wiring.
- Do not add business logic directly here; delegate to `customer/shared/Customer.Shared`.
