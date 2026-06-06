# Data Model: UI Test Automation

## Test Suite Entity

Represents a collection of related UI tests for a single web application.

| Field | Type | Description |
|-------|------|-------------|
| `appId` | string | Web app identifier (`webapp`, `webapp-spaces`, `webapp-teams`) |
| `suiteName` | string | Name of the test suite (e.g., "authentication", "spaces") |
| `testFiles` | string[] | Array of test file paths relative to app's test directory |
| `mockDataPath` | string | Path to mock data configuration for this suite |
| `enabled` | boolean | Whether tests in this suite are active |

## Mock Response Entity

Defines a predefined API response used during testing.

| Field | Type | Description |
|-------|------|-------------|
| `id` | string | Unique identifier for the mock |
| `pathPattern` | string | URL pattern to match (supports glob patterns) |
| `method` | string | HTTP method (`GET`, `POST`, etc.) or `*` for all |
| `responseBody` | object | JSON response body to return |
| `statusCode` | number | HTTP status code (default: 200) |
| `contentType` | string | Response content type (default: `application/json`) |

## Media Asset Entity

Represents a captured video or screenshot.

| Field | Type | Description |
|-------|------|-------------|
| `id` | string | Unique identifier |
| `type` | 'video' \| 'screenshot' | Asset type |
| `testName` | string | Name of the test that produced this asset |
| `filePath` | string | Absolute path to the media file |
| `webApp` | string | Source web application name |
| `timestamp` | string | ISO 8601 timestamp of capture |

## Test Run Entity

Records a single execution of the test suite.

| Field | Type | Description |
|-------|------|-------------|
| `id` | string | Unique identifier |
| `appId` | string | Web app being tested |
| `startTime` | string | ISO 8601 timestamp |
| `endTime` | string \| null | Completion timestamp (null if running) |
| `status` | 'pending' \| 'running' \| 'passed' \| 'failed' \| 'cancelled' | Final status |
| `testCount` | number | Total tests in the run |
| `passedCount` | number | Tests that passed |
| `failedCount` | number | Tests that failed |
| `skippedCount` | number | Tests that were skipped |
| `videoPath` | string \| null | Path to recorded video (if captured) |

## State Transitions

```
Test Run: pending → running → [passed | failed | cancelled]
Media Asset: created (during test execution)
Mock Response: loaded (at test start), used (during test)
```

## Validation Rules

- Test suite name must be alphanumeric with hyphens/underscores only
- Mock path patterns must be valid glob expressions
- Media output paths must be valid filesystem paths
- Test run timestamps must be ISO 8601 compliant
