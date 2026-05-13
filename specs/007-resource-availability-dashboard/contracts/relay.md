# Relay Contract: Resource Availability Dashboard

**Package**: `web/apps/webapp`  
**Feature**: [spec.md](../spec.md) | [graphql.md](graphql.md)  
**Generation**: `web/apps/webapp/scripts/generate.sh` after backend schema regeneration

---

## Root Query (page-level)

```graphql
# web/apps/webapp/src/components/availabilityDashboard/AvailabilityDashboard.graphql

query AvailabilityDashboardQuery($filter: ResourceAvailabilityDayFilterInput!, $orderBy: ResourceAvailabilityOrderByInput) {
  resourceDayViews(filter: $filter, orderBy: $orderBy) {
    ...ResourceDayViewList_result
    subscriptionKey # opaque backend-generated key — pass to subscription as-is
  }
}

subscription OnResourceAvailabilityChangedSubscription($subscriptionKey: String!) {
  onResourceAvailabilityChanged(subscriptionKey: $subscriptionKey) {
    ...ResourceDayViewCard_resourceDayView
  }
}
```

---

## Fragments

### ResourceDayViewList_connection

```graphql
# Inline graphql tag inside ResourceDayViewList.tsx
fragment ResourceDayViewList_result on ResourceDayViewResult {
  items {
    ...ResourceDayViewCard_resourceDayView
  }
  subscriptionKey
}
```

### ResourceDayViewCard_resourceDayView

```graphql
# Inline graphql tag inside ResourceDayViewCard.tsx
fragment ResourceDayViewCard_resourceDayView on ResourceDayView {
  resourceId
  resourceName
  resourceType
  locationId
  locationName
  floorId
  floorName
  zoneId
  zoneName
  date
  status
  openingFrom
  openingUntil
  totalOpeningMinutes
  bookedMinutes
  bookingWindows {
    ...BookingWindowList_bookingWindow
  }
}
```

### BookingWindowList_bookingWindow

```graphql
# Inline graphql tag inside BookingWindowList.tsx
fragment BookingWindowList_bookingWindow on BookingWindow {
  bookingId
  from
  until
  isRecurring
  isCheckedIn
  bookedByName
  notes
}
```

---

## Component Hierarchy

```
page.tsx  (App Router, loadQuery)
└── AvailabilityDashboard.tsx  (usePreloadedQuery, subscription)
    ├── AvailabilityFilterBar.tsx  (controlled: date picker + filter selects + sort select)
    └── ResourceDayViewList.tsx  (useFragment on ResourceDayViewList_result)
        └── ResourceDayViewCard.tsx  (useFragment)
            ├── AvailabilityStatusBadge.tsx  (status chip, no Relay)
            └── BookingWindowList.tsx  (useFragment on each window)
```

---

## Subscription Integration

```typescript
// AvailabilityDashboard.tsx — subscription wired alongside the query
// subscriptionKey is read from the query result (opaque, do not construct on client)
const subscriptionKey = data.resourceDayViews.subscriptionKey;

useSubscription<OnResourceAvailabilityChangedSubscription>(
  {
    subscription: OnResourceAvailabilityChangedSubscriptionDocument,
    variables: { subscriptionKey },
  },
  (state, responseData) => {
    // Merge incoming updated ResourceDayViews into the Relay store
    return state;
  },
);
```

When the filter changes, the parent query re-runs and returns a new `subscriptionKey`. The subscription re-establishes automatically via the new key.

---

## Filter State

Filter values are synchronised with the URL via `useSearchParams` (Next.js):

| URL param      | Value                                                                              |
| -------------- | ---------------------------------------------------------------------------------- |
| `date`         | ISO date string (`YYYY-MM-DD`) — defaults to today                                 |
| `locationId`   | string or empty                                                                    |
| `floorId`      | string or empty                                                                    |
| `zoneId`       | string or empty                                                                    |
| `resourceType` | string or empty                                                                    |
| `status`       | `ResourceAvailabilityStatus` constant or empty                                     |
| `orderByField` | `ResourceAvailabilityOrderByField` constant or empty (defaults to `RESOURCE_NAME`) |
| `orderByDir`   | `ASC` or `DESC` (defaults to `ASC`)                                                |

This enables deep-linking and browser back-button navigation between filter states, consistent with the existing webapp filter pattern.
