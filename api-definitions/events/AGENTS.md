# Event Definitions Notes

This file applies to `api-definitions/events`.

## Source Of Truth

- Event contract source files live under `skedular/*.proto`.
- Change these `.proto` files first. Do not hand-edit generated event code to change event contracts.

## Generation Flow

- Build `shared/Api.Shared.Clients/Api.Shared.Clients.csproj` after changing an event `.proto`.
- That build regenerates protobuf event classes into `obj`.

## Output Expectations

- Protobuf-generated event key/value classes are transient build outputs only.
- Handwritten metadata companion files are expected to remain checked in under `shared/Api.Shared.Clients/Events`.
- Keep versioned metadata under `.../V1/...` directories.
