# OpenAPI Contract: Desk Availability Analytics

**Phase**: 1 — Design  
**Date**: 2026-04-29  
**Source file to modify**: `api-definitions/openapi/skedular/location/location_analytics_v1.yaml`  
**Regeneration**: run `api-definitions/openapi/generate.sh` after adding endpoint

---

## New Endpoint: Backfill Resource Availability Snapshots by Date Range

**Source of truth**: The endpoint below is to be added to `location_analytics_v1.yaml`.

```yaml
/v1/location/analytics/{locationId}/regenerate-resource-availability-snapshots:
  put:
    tags:
      - v1
      - location
      - analytics
    summary: regenerate resource availability snapshots for a location over a date range
    operationId: regenerateResourceAvailabilitySnapshots
    parameters:
      - in: path
        name: locationId
        schema:
          type: string
        required: true
    requestBody:
      required: true
      content:
        application/json:
          schema:
            $ref: "#/components/schemas/RegenerateResourceAvailabilitySnapshotsInput"
    responses:
      "200":
        description: resource availability snapshots regeneration triggered
      default:
        description: unexpected error
        content:
          application/json:
            schema:
              $ref: "#/components/schemas/ProblemDetails"
```

New schema component to add under `components/schemas`:

```yaml
RegenerateResourceAvailabilitySnapshotsInput:
  type: object
  required:
    - from
    - until
  properties:
    from:
      type: string
      format: date-time
      description: Start of the date range (inclusive, UTC)
    until:
      type: string
      format: date-time
      description: End of the date range (inclusive, UTC)
```

---

## Existing Endpoints (unchanged)

- `PUT /v1/location/analytics/regenerate-all-daily-analytics` — triggers existing `GenerateLocationDailyAnalytics` workflow for all locations (no date range — regenerates from today). Not changed.
- `PUT /v1/location/analytics/{locationId}/regenerate-daily-analytics` — triggers for one location (no date range). Not changed.

The new endpoint adds date-range scoped backfill for desk availability snapshots only. It does not retrigger the existing desk/room count activities.

---

## Controller

Controller method generated from OpenAPI base (`LocationAnalyticsController.cs`). Implementation delegates to `IWorkaroundService` (existing pattern), which triggers the snapshot activity for each day in `[from, until]`.

---

## Breaking Changes

None. New endpoint only.
