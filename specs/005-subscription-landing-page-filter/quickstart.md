# Quickstart: Subscription Landing Page Filtering

**Feature**: `005-subscription-landing-page-filter`  
**Date**: 2026-04-27

---

## Prerequisites

- Local services running: booking API, PostgreSQL (via Docker Compose or Aspire)
- `pnpm install` complete in `web/`
- .NET SDK for .NET 10

---

## Validate Backend Filter Option Queries

Run the booking API locally and send these GraphQL queries to verify the new endpoints:

```graphql
query ValidateFilterOptions {
  marketplaceBookingSubscriptionStatuses {
    type
    name
  }
  marketplaceBookingPaymentStatuses {
    type
    name
  }
}
```

**Expected**: Both return non-empty arrays of `{ type, name }` objects matching the defined enum values.

---

## Validate Backend Subscription Status Filter

```graphql
query ValidateStatusFilter($orgDomain: String!) {
  marketplaceBookingSubscriptions(first: 10, where: { organizationCustomDomain: $orgDomain, statuses: [ACTIVE] }) {
    edges {
      node {
        id
        status {
          type
        }
      }
    }
    totalCount
  }
}
```

**Expected**: All returned subscriptions have `status.type == "ACTIVE"`.

---

## Validate Backend Payment Status Filter

```graphql
query ValidatePaymentFilter($orgDomain: String!) {
  marketplaceBookingSubscriptions(first: 10, where: { organizationCustomDomain: $orgDomain, paymentStatuses: [PENDING] }) {
    edges {
      node {
        id
        marketplaceBooking {
          paymentStatus {
            type
          }
        }
      }
    }
    totalCount
  }
}
```

**Expected**: All returned subscriptions have `marketplaceBooking.paymentStatus.type == "PENDING"`.

---

## Validate Combined Filters

```graphql
query ValidateCombinedFilters($orgDomain: String!) {
  marketplaceBookingSubscriptions(
    first: 10
    where: { organizationCustomDomain: $orgDomain, statuses: [ACTIVE, PAUSED], paymentStatuses: [PENDING, REJECTED] }
  ) {
    edges {
      node {
        id
        status {
          type
        }
        marketplaceBooking {
          paymentStatus {
            type
          }
        }
      }
    }
    totalCount
  }
}
```

**Expected**: All returned subscriptions have `status.type` in `[ACTIVE, PAUSED]` AND `paymentStatus.type` in `[PENDING, REJECTED]`.

---

## Validate Empty Filters (No Restriction)

```graphql
query ValidateEmptyFilter($orgDomain: String!) {
  marketplaceBookingSubscriptions(first: 50, where: { organizationCustomDomain: $orgDomain, statuses: [], paymentStatuses: [] }) {
    totalCount
  }
}
```

**Expected**: `totalCount` equals the same count as a query with no `statuses`/`paymentStatuses` fields.

---

## Validate Frontend Filter Controls

1. Open the Management Portal subscription list page for an organization.
2. Confirm the **subscription status** multi-select combo box is visible with options populated from the backend.
3. Confirm the **payment status** multi-select combo box is visible with options populated from the backend.
4. Select one subscription status → confirm the URL updates (e.g. `?statuses=ACTIVE`) and the list refreshes.
5. Add a second status → confirm the URL includes both values and the list re-fetches.
6. Deselect all statuses → confirm the URL param is cleared and all subscriptions return.
7. Apply a payment status filter → confirm URL updates and results are filtered.
8. Reload the page with filter params in the URL → confirm filters are pre-populated and results are filtered immediately.

---

## Run Tests

```bash
# Backend unit tests (booking API)
cd booking/apis/Booking.Api.UnitTests
dotnet test

# Backend shared unit tests
cd booking/shared/Booking.Shared.UnitTests
dotnet test

# Integration tests
cd booking/domain/Booking.Domain.IntegrationTests
dotnet test

# Frontend
cd web/apps/webapp
pnpm test
```
