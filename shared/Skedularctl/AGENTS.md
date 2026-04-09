# Skedularctl Notes

This file applies to `shared/Skedularctl`.

## Purpose

- `Skedularctl` is the repo-level CLI maintenance tool for Skedular.
- It provides commands for administrative tasks such as database management, data migration, and operational tooling.

## Scope Boundary

- `Skedularctl` does not own event metadata companions anymore.
- It does not own the normal protobuf C# class generation for event key/value messages either. That generation is
  handled by `shared/Api.Shared.Clients/Api.Shared.Clients.csproj`.
- Do not add domain business logic to `Skedularctl`; keep it as an operational/maintenance tool.

## Agent Rule

- Keep commands in `Skedularctl` focused on maintenance and operational concerns, not domain feature logic.
- If a new admin operation is needed, prefer adding a protected API endpoint or a Temporal workflow; use `Skedularctl`
  only for tasks that cannot be driven through the normal API surface.
- Do not check in protobuf-generated event classes in this project.
