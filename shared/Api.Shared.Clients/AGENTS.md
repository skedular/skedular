# Api.Shared.Clients Notes

This file applies to `shared/Api.Shared.Clients`.

## Event Code Ownership

- Event protobuf definitions are owned by `api-definitions/events/skedular/*.proto`.
- Do not hand-edit protobuf-generated event key/value classes.
- Shared event contracts such as `IEvent`, `KafkaTopicAttribute`, and `KafkaTopicHelper` live in `Events/`.

## Generated Event Files

- Event protobuf classes are generated into this project's `obj` directory during build.
- The project explicitly includes those transient `obj` outputs so checked-in metadata companions can compile against
  them.
- Do not reintroduce checked-in protobuf-generated files such as `*V1Key.g.cs` or `*V1Value.g.cs` under
  `Events/`.

## Checked-In Event Files

- The checked-in files under `Events/Skedular/**` are handwritten metadata companions layered onto generated protobuf
  partial classes.
- Prefer one handwritten metadata file per event namespace/version and keep shared event-topic logic in common helper
  types rather than duplicating it across split key/value files.
- Keep those metadata files versioned under directories like `Events/Skedular/Booking/V1/`.
