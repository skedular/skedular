# Organization Infrastructure Agent Notes

This file applies to `organization/shared/Organization.Infrastructure`.

## Purpose

- `Organization.Infrastructure` is the database migration runner for the organization domain.
- It runs EF Core migrations against the organization database on startup via a background job.
- It is a short-lived Aspire process: it starts, applies pending migrations, and exits.

## Agent Rule

- Do not add feature logic or application endpoints here.
- Database schema changes are driven by EF Core migrations in `Organization.Shared/Database/`.
- If a migration is needed, add it to the domain shared library and the migration runner picks it up automatically.
