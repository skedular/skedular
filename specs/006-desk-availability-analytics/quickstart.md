# Quickstart: Desk Availability Analytics

**Phase**: 1 — Design  
**Date**: 2026-04-30  
**Branch**: `006-desk-availability-analytics`

## What This Feature Does

Adds per-desk daily availability snapshots to the location analytics system. For every location,
at UTC midnight each day, the system records which desks were available, unavailable (inactive),
or booked on that day. Results are queryable via the existing `locationsAnalytics` GraphQL query
by adding the `deskAvailabilitySnapshots` field. Historical snapshots can be backfilled or
regenerated via a new REST endpoint.

Also fixes three bugs in existing location analytics:

- Cancelled bookings were counted in occupancy (now excluded; the booking gRPC server already filters soft-deleted bookings server-side)
- Dual-tagged desk+room resources were double-counted; they are now counted as desks only in `RecordLocationDesksCountAsync` (rooms count excludes them), and classified as `Unavailable` in the availability snapshot
- Zero-capacity locations returned misleading 0% occupancy (now omitted from the `desksOccupancyPercentage` list entirely)

---

## Running Locally

1. Start the full stack:

   ```sh
   docker-compose -f docker-compose-min.yml up -d
   ```

2. Run the `Location.Api` service and `Location.Jobs` (Temporal worker):

   ```sh
   cd location/apis/Location.Api && dotnet run
   cd location/jobs/Location.Jobs && dotnet run
   ```

3. Run the database migration (after applying the new migration):
   ```sh
   cd location/shared/Location.Shared && dotnet ef database update
   ```

---

## Triggering a Snapshot Manually

Use the existing REST endpoint to regenerate analytics for all locations (which now includes
the desk availability snapshot step):

```http
PUT http://localhost:10600/v1/location/analytics/regenerate-all-daily-analytics
```

Or trigger for a specific location:

```http
PUT http://localhost:10600/v1/location/analytics/{locationId}/regenerate-daily-analytics
```

---

## Querying Resource Availability Analytics

```graphql
query {
  locationsAnalytics(from: "2025-11-01T00:00:00Z", until: "2026-04-29T23:59:59Z", where: { locationIds: ["<locationId>"] }) {
    name
    resourceAvailabilitySnapshots {
      date
      availableCount
      unavailableCount
      bookedCount
      availableResourceNames
      unavailableResourceNames
      bookedResourceNames
    }
  }
}
```

Expected response shape:

```json
{
  "locationsAnalytics": [
    {
      "name": "City Office",
      "resourceAvailabilitySnapshots": [
        {
          "date": "2026-04-28T00:00:00Z",
          "availableCount": 6,
          "unavailableCount": 2,
          "bookedCount": 3,
          "availableResourceNames": ["Desk A", "Desk B", "Desk C", "Desk D", "Desk E", "Desk F"],
          "unavailableResourceNames": ["Desk G", "Desk H"],
          "bookedResourceNames": ["Desk I", "Desk J", "Desk K"]
        }
      ]
    }
  ]
}
```

Days with no snapshot recorded (before the job was enabled, or failed days) are omitted from
the `resourceAvailabilitySnapshots` array.

---

## Running Tests

```sh
# Unit tests (location shared)
dotnet test location/shared/Location.Shared.UnitTests/Location.Shared.UnitTests.csproj

# Unit tests (location api)
dotnet test location/apis/Location.Api.UnitTests/Location.Api.UnitTests.csproj

# Integration tests (location api)
dotnet test location/apis/Location.Api.IntegrationTests/Location.Api.IntegrationTests.csproj
```

---

## Regenerating GraphQL Schema

After modifying the GraphQL resolver or type:

```sh
bash scripts/generate-graphql.sh
```

After modifying the OpenAPI YAML:

```sh
bash api-definitions/openapi/generate.sh
```
