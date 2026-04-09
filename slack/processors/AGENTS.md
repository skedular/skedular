# Slack Processors Agent Notes

This file applies to `slack/processors/`.

## Purpose

- `Slack.Processors` is the Kafka event consumer host for the Slack integration domain.
- It subscribes to Kafka topics published by other domains to react to platform events and post the appropriate Slack
  notifications to the correct workspaces and channels.

## Kafka Subscriptions

| Subscriber class          | Kafka topic / event source | Responsibility                                                                   |
|---------------------------|----------------------------|----------------------------------------------------------------------------------|
| `CustomerSubscriber`      | `Customer.V1`              | Keeps replicated customer/identity state current for Slack workspace routing     |
| `LocationSubscriber`      | `Location.V1`              | Keeps replicated location state current for channel routing and update targeting |
| `OrganizationSubscriber`  | `Organization.V1`          | Keeps replicated org state current for workspace routing                         |
| `TeamSubscriber`          | `Team.V1`                  | Keeps replicated team state current for team channel routing                     |

## Important Behavior Notes

- Slack routing is driven by replicated organization, location, and team state held locally.
- Do not remove these replication handlers unless the Slack workspace/channel routing model is redesigned.
- Workflow-driven rebuilds are only candidates for derived projections that are neither auth-critical nor
  routing-critical.

## Relationship To Other Slack Hosts

- Does not drain outboxes (that is `Slack.Jobs`).
- Does not serve HTTP/GraphQL (that is `Slack.Api`).
- All shared Slack logic lives in `slack/shared/Slack.Shared/`.

## Agent Rule

- Keep subscribers thin; delegate to shared services for any non-trivial logic.
- Do not remove routing-critical replicated state without redesigning the Slack routing model first.
- Do not add outbox drain logic here; that belongs in `slack/jobs/`.
- Keep Aspire dependency readiness (`WaitFor`) in the Slack domain app host.
