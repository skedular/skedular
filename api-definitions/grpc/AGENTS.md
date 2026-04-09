# gRPC Proto Definitions Agent Notes

This file applies to `api-definitions/grpc/`.

## Source Of Truth

- gRPC service contract source files live under `skedular/*.proto`.
- One `.proto` file per domain service, e.g. `booking_v1.proto`, `organization_v1.proto`.

## Generation Flow

- Generated C# gRPC stubs are **not** checked in.
- Each consuming `.csproj` file declares a `<Protobuf>` item group referencing these `.proto` files.
- The C# stubs are generated automatically at build time by the `Grpc.Tools` MSBuild tooling.
- Do not check in generated gRPC stub files; do not hand-edit generated stubs.

## When To Edit

- To add or change a gRPC service method: edit the `.proto` file, then rebuild the consuming project.
- To add a new gRPC service entirely: create a new `.proto` file and add the `<Protobuf>` reference to the consuming `.csproj`.
- Internal/test gRPC services (e.g. `infrastructure_test_v1.proto`) follow the same pattern.

## Agent Rule

- Change `.proto` files first; let the build regenerate C# stubs.
- Do not patch generated gRPC stubs directly.
- After changing a `.proto`, rebuild the consuming project to validate the generated surface before implementing.
