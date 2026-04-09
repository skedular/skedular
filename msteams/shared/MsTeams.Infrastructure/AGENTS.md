# MsTeams Infrastructure Agent Notes

This file applies to `msteams/shared/MsTeams.Infrastructure`.

## Purpose

- `MsTeams.Infrastructure` is the database migration runner for the msteams domain.
- It runs EF Core migrations against the msteams database on startup via a background job.
- It is a short-lived Aspire process: it starts, applies pending migrations, and exits.

## Agent Rule

- Do not add feature logic or application endpoints here.
- Database schema changes are driven by EF Core migrations in `MsTeams.Shared/Database/`.
- If a migration is needed, add it to the domain shared library and the migration runner picks it up automatically.
