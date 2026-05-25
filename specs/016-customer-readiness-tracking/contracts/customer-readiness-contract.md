# Contract: Customer Readiness

## Scope

Defines the public customer-owned readiness event contract, the participating-domain publishing contract, the
customer-domain consumption contract, the central readiness lookup contract, and the manual synchronisation/backfill
contract.

## Event topic

- Topic name: `customer_readiness`
- Owner: customer domain
- Visibility: public cross-domain topic; participating non-customer domains may publish
- Version: `v1`
- Source definition files:
  - `api-definitions/events/skedular/customer_readiness_v1_key.proto`
  - `api-definitions/events/skedular/customer_readiness_v1_value.proto`
- Handwritten metadata companion:
  - `shared/Api.Shared.Clients/Events/Skedular/CustomerReadiness/V1/CustomerReadinessMetadata.cs`
- Required generation:
  - Run `api-definitions/events/generate.sh`
  - Do not check in protobuf-generated `*V1Key.g.cs` or `*V1Value.g.cs` outputs

## Protobuf shape

Illustrative shape; final field names and enum spelling should follow the repository's protobuf style.

```proto
syntax = "proto3";
package customer_readiness;

import "google/protobuf/timestamp.proto";

option csharp_namespace = "Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1";

message Key {
  string customerId = 1;
}

enum Type {
  Type_CustomerIdentityProvisioned = 0;
}

enum Domain {
  Domain_Booking      = 0;
  Domain_Organization = 1;
  Domain_Team         = 2;
  Domain_Marketplace  = 3;
  Domain_Location     = 4;
  Domain_Core         = 5;
  Domain_Slack        = 6;
  Domain_MsTeams      = 7;
}

message Event {
  Metadata metadata = 1;
  Data     data     = 2;
}

message Metadata {
  string                    id            = 1;
  string                    domainSource  = 2;
  string                    appSource     = 3;
  Type                      type          = 4;
  google.protobuf.Timestamp time          = 5;
  string                    correlationId = 6;
}

message Data {
  CustomerIdentityProvisioned customerIdentityProvisioned = 1;
}

message CustomerIdentityProvisioned {
  string customerId = 1;
  Domain domain     = 2;
}
```

The payload for `CustomerIdentityProvisioned` must remain limited to `customerId` and `domain`. Do not add
organisation, tenant, status, failure, occurred-at, correlation, or causation fields to the payload unless already
covered by the standard metadata envelope.

## Participating-domain publishing contract

Current participating publishers:

| Domain       | Source subscriber                                                                   |
| ------------ | ----------------------------------------------------------------------------------- |
| Booking      | `booking/processors/Booking.Processors/Subscribers/CustomerSubscriber.cs`           |
| Organisation | `organization/processors/Organization.Processors/Subscribers/CustomerSubscriber.cs` |
| Team         | `team/processors/Team.Processors/Subscribers/CustomerSubscriber.cs`                 |
| Marketplace  | `marketplace/processors/Marketplace.Processors/Subscribers/CustomerSubscriber.cs`   |
| Location     | `location/processors/Location.Processors/Subscribers/CustomerSubscriber.cs`         |
| Core         | `core/processors/Core.Processors/Subscribers/CustomerSubscriber.cs`                 |
| Slack        | `slack/processors/Slack.Processors/Subscribers/CustomerSubscriber.cs`               |
| MsTeams      | `msteams/processors/MsTeams.Processors/Subscribers/CustomerSubscriber.cs`           |

Publishing rules:

- Publish only for `CustomerUpserted`/provisioning source events that successfully ensure local customer identity.
- Publish after local persistence and any required cache invalidation are complete enough for authenticated/federated
  execution to recognise the customer.
- Do not publish on source event receipt alone.
- Do not publish for `CustomerDeleted`.
- Do not publish if the domain cannot map itself to the readiness `Domain` enum.
- Replayed source events must be safe: ensure local state idempotently, then publish readiness again when mapped.
- Emit structured logs for local provisioning decisions, skipped publish due to unmappable domain, publish completion,
  and publish failure.

## Customer-domain consumption contract

Consumer:

- Customer processors subscribe to `customer_readiness`.
- `CustomerIdentityProvisioned` marks only the reported domain as provisioned for the reported customer.
- Unknown future readiness event types are ignored or logged without failing known processing.

Persistence rules:

- Upsert one central readiness aggregate per customer.
- Upsert one domain state per `(customerId, domain)`.
- Missing aggregate or missing domain state counts as pending.
- Derive overall status from the central required-domain set.
- Set `activatedAt` only on the first transition to active.
- Do not regress an active customer because of duplicate/replayed success.
- Emit structured logs for consumption start/completion, duplicate/replay outcomes, per-domain state changes,
  activation, and failures.

## Central readiness lookup contract

Backend readiness/auth checks call one customer-domain readiness service/repository. The lookup result is:

| State                                           | Meaning                                        | Access result                               |
| ----------------------------------------------- | ---------------------------------------------- | ------------------------------------------- |
| Missing aggregate                               | No central readiness state exists              | Block as activating/pending                 |
| Aggregate exists, missing required domain state | At least one required domain has not reported  | Block as activating/pending                 |
| Aggregate exists, required domain pending       | At least one required domain is pending        | Block as activating/pending                 |
| Aggregate active                                | All required domains have reported provisioned | Allow normal authenticated/federated access |

The hot path must not call booking, organisation, team, marketplace, or location to recompute readiness.

## Manual synchronisation/backfill contract

Operators can manually trigger customer synchronisation/backfill through the customer-domain republish path. Expected
flow:

1. Customer source events are republished or reprocessed for one or all customers.
2. Participating domains idempotently ensure local identity exists.
3. Participating domains republish `CustomerIdentityProvisioned`.
4. Customer processors update central readiness.
5. Customers without central readiness remain blocked until all required domain reports arrive.

Existing active customers are not grandfathered. Temporary downtime is acceptable after the central gate is enabled.

## Required tests

- Event contract compiles after generation and metadata companion reports the `customer_readiness` topic names.
- Domain enum excludes unspecified, unknown, none, and customer values.
- Each participating subscriber publishes after durable local provisioning succeeds.
- Participating subscriber does not publish when domain mapping fails.
- Replayed customer source events republish readiness safely.
- Customer readiness consumer marks only the reported domain provisioned.
- Duplicate readiness events do not create duplicate domain states or duplicate activation transitions.
- Missing readiness state and missing domain states block access.
- Active customers do not regress on duplicate success.
- Hot-path readiness check uses the customer-domain lookup and does not call participating domains.
