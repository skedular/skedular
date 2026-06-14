# Entitlement Contract

The entitlement contract standardizes allow/block decisions across Organization, Booking, Team, and Location without sharing private persistence between domains. Organization publishes pricing/subscription state through existing events; other domains store that projected state locally as JSON/projection data and enforce through shared `Api.Shared.Services` code.

## Inputs

- `organizationId`
- `action`
- `actorCustomerId`
- `targetCustomerId` when different from actor
- `billingPeriod`
- `requestedResourceType`: active user, team, location, booking participation, or booking update
- `requestedQuantity`
- `contextCorrelationId`

## Outputs

- `allowed`
- `reasonCode`
- `userMessage`
- `operatorMessage`
- `planCode`
- `subscriptionId`
- `quotaType`
- `quotaLimit`
- `currentUsage`
- `attemptedUsage`

## Required Reason Codes

- `ALLOWED`
- `FREE_ACTIVE_USER_LIMIT_REACHED`
- `FREE_TEAM_LIMIT_REACHED`
- `FREE_LOCATION_LIMIT_REACHED`
- `ENTERPRISE_CAPACITY_REACHED`
- `SUBSCRIPTION_NOT_FOUND`
- `SUBSCRIPTION_NOT_EFFECTIVE`
- `CONTACT_US_REQUIRED`
- `LEGACY_SUBSCRIPTION_UNCHANGED`

## Required Behavior

- Free allows at most 10 active users, one team, and one location.
- Pay As You Go allows unlimited teams and locations and does not block active users for quota, but active usage must be counted for billing.
- Enterprise Capacity allows unlimited teams and locations and blocks only newly active users beyond purchased capacity.
- Existing active users continue normal work during the billing period after Enterprise Capacity is reached.
- Existing Early Bird remains honored and unchanged.

## Logging

Each decision must log:

- correlation context
- organization id
- action
- subscription plan
- catalog version
- reason code
- current usage and attempted usage where applicable

Logs must not include sensitive customer payloads.
