# Core Processors Agent Notes

This file applies to `core/processors/`.

## Purpose

- `Core.Processors` is the Kafka event consumer host for the core domain.
- It subscribes to Kafka topics published by other domains to keep core-domain state consistent, particularly
  replicated auth-critical state used for platform-wide authorization decisions.

## Kafka Subscriptions

| Subscriber class          | Kafka topic / event source | Responsibility                                                                     |
|---------------------------|----------------------------|------------------------------------------------------------------------------------|
| `CustomerSubscriber`      | `Customer.V1`              | Keeps replicated customer/identity state in the core domain current                |
| `OrganizationSubscriber`  | `Organization.V1`          | Keeps replicated organization state current for core authorization                 |

## Important Behavior Notes

- `core/` performs cross-cutting authorization decisions that many other domains rely on.
- Auth-critical replicated state (organization, customer) must be kept fresh; removing these subscriber handlers can
  silently break authorization flows across the platform.
- File storage is registered in this host because core activities can produce files.

## Relationship To Other Core Hosts

- Does not drain outboxes (that is `Core.Jobs`).
- Does not serve HTTP/GraphQL (that is `Core.Api`).
- All shared core logic lives in `core/shared/Core.Shared/`.

## Agent Rule

- Do not remove or disable the customer/organization replication handlers without a full authorization audit.
- Keep subscribers thin; delegate to shared services for any non-trivial logic.
- Changes here can affect platform-wide authorization — treat with extra care.
- Do not add outbox drain logic here; that belongs in `core/jobs/`.
- Keep Aspire dependency readiness (`WaitFor`) in the core domain app host.
