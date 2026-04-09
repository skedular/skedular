# MsTeams Processors Agent Notes

This file applies to `msteams/processors/`.

## Purpose

- `MsTeams.Processors` is the Kafka event consumer host for the Microsoft Teams integration domain.
- It subscribes to Kafka topics published by other domains to react to platform events and deliver the appropriate
  Microsoft Teams notifications to the correct Azure tenants and Teams channels.

## Kafka Subscriptions

| Subscriber class          | Kafka topic / event source | Responsibility                                                                         |
|---------------------------|----------------------------|----------------------------------------------------------------------------------------|
| `CustomerSubscriber`      | `Customer.V1`              | Keeps replicated customer/identity state current for Teams routing                     |
| `LocationSubscriber`      | `Location.V1`              | Keeps replicated location state current for Teams channel routing and update targeting |
| `OrganizationSubscriber`  | `Organization.V1`          | Keeps replicated org/Azure-tenant mapping current for workspace routing                |
| `TeamSubscriber`          | `Team.V1`                  | Keeps replicated team state current for team channel routing                           |

## Important Behavior Notes

- MsTeams routing is driven by replicated organization, location, and team state held locally.
- Do not remove these replication handlers unless the Azure-tenant and Teams-channel routing model is redesigned.
- Workflow-driven rebuilds are only candidates for derived projections that are neither auth-critical nor
  routing-critical.

## Relationship To Other MsTeams Hosts

- Does not drain outboxes (that is `MsTeams.Jobs`).
- Does not serve HTTP/GraphQL (that is `MsTeams.Api`).
- All shared MsTeams logic lives in `msteams/shared/MsTeams.Shared/`.

## Agent Rule

- Keep subscribers thin; delegate to shared services for any non-trivial logic.
- External Microsoft Teams API contracts can break production integrations quickly — prefer behavior-preserving changes.
- Do not remove routing-critical replicated state without redesigning the MsTeams routing model first.
- Do not add outbox drain logic here; that belongs in `msteams/jobs/`.
- Keep Aspire dependency readiness (`WaitFor`) in the MsTeams domain app host.
