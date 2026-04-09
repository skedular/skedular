# Location Infrastructure Agent Notes

This file applies to `location/shared/Location.Infrastructure`.

## Purpose

- `Location.Infrastructure` is the database migration runner for the location domain.
- It runs EF Core migrations against the location database on startup via a background job.
- It is a short-lived Aspire process: it starts, applies pending migrations, and exits.

## Agent Rule

- Do not add feature logic or application endpoints here.
- Database schema changes are driven by EF Core migrations in `Location.Shared/Database/`.
- If a migration is needed, add it to the domain shared library and the migration runner picks it up automatically.
