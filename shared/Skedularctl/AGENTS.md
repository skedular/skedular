# Skedularctl Notes

This file applies to `shared/Skedularctl`.

## Event Metadata Generator

- `Skedularctl` owns the custom event metadata generation used by `scripts/generate-event-metadata.sh`.
- The liquid templates under `Events/Resources/` are the source of truth for generated metadata companion files.
- If a metadata companion shape is wrong across multiple events, fix the template here rather than patching generated
  files one by one.

## Scope Boundary

- `Skedularctl` generates event metadata companions.
- It does not own the normal protobuf C# class generation for event key/value messages. That generation is handled by
  `shared/Api.Shared.Clients/Api.Shared.Clients.csproj`.
