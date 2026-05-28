# Customer Infrastructure Agent Notes

This file applies to `customer/shared/Customer.Infrastructure`.

## Purpose

- `Customer.Infrastructure` is the database migration runner for the customer domain.
- It runs EF Core migrations against the customer database on startup via a background job.
- It is a short-lived Aspire process: it starts, applies pending migrations, and exits.

## Agent Rule

- Do not add feature logic or application endpoints here.
- Database schema changes are driven by EF Core migrations in `Customer.Shared/Database/`.
- If a migration is needed, add it to the domain shared library and the migration runner picks it up automatically.
