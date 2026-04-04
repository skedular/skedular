# Event Definitions Notes

This file applies to `api-definitions/events`.

## Source Of Truth

- Event contract source files live under `skedular/*.proto`.
- Change these `.proto` files first. Do not hand-edit generated event code to change event contracts.

## Generation Flow

- Run `./generate.sh` from this directory after changing an event `.proto`.
- That flow now does two separate things:
  - builds `shared/Api.Shared.Clients/Api.Shared.Clients.csproj` so protobuf event classes are regenerated into `obj`
  - runs `scripts/generate-event-metadata.sh` so checked-in metadata companions are regenerated under
    `shared/Api.Shared.Clients/Events`

## Output Expectations

- Protobuf-generated event key/value classes are transient build outputs only.
- Only metadata companion files are expected to remain checked in under `shared/Api.Shared.Clients/Events`.
- Keep versioned metadata under `.../V1/...` directories.
