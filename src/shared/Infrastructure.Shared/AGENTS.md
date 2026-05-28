# Infrastructure.Shared Agent Notes

This file applies to everything under `shared/Infrastructure.Shared`.

## Purpose

- `Infrastructure.Shared` is a local orchestration and test-support host.
- It is used for integration tests, system tests, and local IDE/Aspire runs.
- It is not treated as a production runtime host.

## Responsibility Boundary

- This project may host reusable local infrastructure needed by tests.
- This project should stay generic and reusable across app hosts.
- Do not put booking-specific fake dependency behavior here.

## Shared Infra Pattern

- This project is the generic local infra/bootstrap host used by multiple app hosts.
- Kafka topic creation and similar shared bootstrapping belong here.
- Domain-specific fake dependencies should live close to the owning domain instead of being accumulated here.

## Startup Expectations

- `Infrastructure.Shared` should be able to start before the domain under test.
- Keep this host generic so multiple domain app hosts can keep referencing it.

## Testing Goal

- The target shape is:
    - start shared local infrastructure needed by app hosts
    - keep domain-specific fake behavior in domain-local fake dependency hosts
