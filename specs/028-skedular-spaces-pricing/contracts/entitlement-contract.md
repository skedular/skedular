# Entitlement Contract: Spaces Booking Quota

The Spaces entitlement contract standardizes allow/block decisions for booking-instance creation without sharing private persistence between domains. Organization owns plan assignment and catalog data. Booking owns current booking-instance usage and executes enforcement at every booking creation path.

## Inputs

- `organizationId`
- `actorCustomerId` when available
- `bookingCreationPath`: one-off, multi-slot, recurring, subscription-generated, or admin-created
- `billingPeriodStartUtc`
- `billingPeriodEndUtc`
- `currentUsage`
- `requestedInstanceCount`
- `requestedCurrentPeriodInstanceCount`
- `requestedOutOfPeriodInstanceCount`
- `planCode`
- `monthlyBookingInstanceQuota`
- `catalogVersionCode`
- `contextCorrelationId`

## Outputs

- `allowed`
- `reasonCode`
- `userMessage`
- `operatorMessage`
- `planCode`
- `quotaType`: monthly booking instances
- `quotaLimit`
- `currentUsage`
- `attemptedUsage`
- `excludedOutOfPeriodInstanceCount`
- `remainingQuota`
- `upgradePlans`

## Required Reason Codes

- `ALLOWED`
- `SPACES_SUBSCRIPTION_NOT_FOUND`
- `SPACES_SUBSCRIPTION_NOT_EFFECTIVE`
- `SPACES_BOOKING_INSTANCE_QUOTA_EXCEEDED`
- `SPACES_CONTACT_US_REQUIRED`

## Required Behavior

- Free allows up to 100 created booking instances per UTC monthly billing period.
- Growth allows up to 500 created booking instances per UTC monthly billing period.
- Business and Contact Us limits come from the catalog or admin-negotiated capacity.
- Updates to existing booking records do not consume quota.
- Each distinct stored booking record created by one-off, multi-slot, recurring, subscription-generated, or admin-created paths consumes one quota unit in the billing period that contains its scheduled start.
- Booking instances scheduled outside the current billing period do not consume current-period quota.
- The decision must use close-to-real-time persisted Booking row counts for the current billing period.
- If quota validation passes but booking creation fails, usage remains unchanged because no booking row is created.
- Minor concurrent overage is acceptable; this contract does not require an atomic usage counter.
- Canceled booking instances remain counted for the monthly period.
- Downgrades below current usage do not delete existing usage; additional creation is blocked until rollover or upgrade.

## Logging

Each decision must log:

- correlation context
- organization id
- booking creation path
- plan code and catalog version
- reason code
- current usage, attempted usage, and quota limit
- out-of-period instance count excluded from the current-period quota
- upgrade/contact plan count when blocked

Logs must not include sensitive booking payloads.
