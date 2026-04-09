# Team Processors Agent Notes

This file applies to `team/processors/`.

## Purpose

- `Team.Processors` is the Kafka event consumer host for the team domain.
- It subscribes to Kafka topics published by other domains to keep team-local state consistent.

## Kafka Subscriptions

| Subscriber class          | Kafka topic / event source | Responsibility                                                               |
|---------------------------|----------------------------|------------------------------------------------------------------------------|
| `CustomerSubscriber`      | `Customer.V1`              | Keeps replicated customer/identity state in the team domain current          |
| `LocationSubscriber`      | `Location.V1`              | Keeps replicated location state current for team-location associations       |
| `OrganizationSubscriber`  | `Organization.V1`          | Keeps replicated org/membership state current for team authorization         |

## Important Behavior Notes

- Team identity and membership semantics are shared contracts with other domains.
- Do not replicate booking-derived state into team processors; team booking questions belong to the booking domain.
- Auth-critical replicas (org, customer) must be kept current.

## Relationship To Other Team Hosts

- Does not drain outboxes (that is `Team.Jobs`).
- Does not serve HTTP/GraphQL (that is `Team.Api`).
- All shared team logic lives in `team/shared/Team.Shared/`.

## Agent Rule

- Keep subscribers thin; delegate to shared services for any non-trivial logic.
- Do not add outbox drain logic here; that belongs in `team/jobs/`.
- Keep Aspire dependency readiness (`WaitFor`) in the team domain app host.
