# Skedularctl Notes

This file applies to `shared/Skedularctl`.

## Scope Boundary

- `Skedularctl` does not own event metadata companions anymore.
- It does not own the normal protobuf C# class generation for event key/value messages either. That generation is
  handled by `shared/Api.Shared.Clients/Api.Shared.Clients.csproj`.
