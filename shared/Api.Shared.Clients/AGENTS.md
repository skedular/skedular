# Api.Shared.Clients Notes

This file applies to `shared/Api.Shared.Clients`.

## Framework Target

- Targets **netstandard2.0** to enable broader cross-platform consumption (.NET Framework, .NET Core, .NET 5+).
- Depends on `Api.Shared` for shared event contract interfaces and helpers.

## Event Code Ownership

- Shared event contracts (`IEvent`, `IEventExtensions`, `KafkaTopicAttribute`, `KafkaTopicHelper`,
  `EventMetadataFactory`) are owned by `api-definitions/events/skedular/*.proto` and `shared/Api.Shared/`.
- Do not hand-edit protobuf-generated event key/value classes.
- Event metadata companions under `Api.Shared.Clients/Events/Skedular/**` are handwritten implementations of `IEvent`
  and `IEventMetadata<T>` that layer on top of protobuf-generated partial classes.

## Generated Event Files

- Event protobuf classes are generated into this project's `obj` directory during build.
- The project explicitly includes those transient `obj` outputs so checked-in metadata companions can compile against
  them.
- Do not reintroduce checked-in protobuf-generated files such as `*V1Key.g.cs` or `*V1Value.g.cs` under `Events/`.

## Checked-In Event Files

- The checked-in files under `Events/Skedular/**` are handwritten metadata companions layered onto generated protobuf
  partial classes.
- Prefer one handwritten metadata file per event namespace/version and keep shared event-topic logic in common helper
  types (use `IEventExtensions` for cross-event behavior).
- Keep those metadata files versioned under directories like `Events/Skedular/Booking/V1/`.
- Import event contracts from `Api.Shared.Events` namespace.

## Dependency Rule

- `Api.Shared.Clients` depends on `Api.Shared` for the core event contract interfaces.
- Do not redefine `IEvent`, `IEventExtensions`, `KafkaTopicAttribute`, or related types here; import them from
  `Api.Shared`.
- If shared event contract behavior needs to change, update `Api.Shared` and then rebuild both `Api.Shared.Clients` and
  all downstream domains.
