# Data Model: Team Domain Structured Logging

## Entity: LoggableComponent

Represents any production Team-domain class that must adopt structured `ILogger<T>` usage.

### Fields

- `component_name` (string, required): Class name.
- `component_path` (string, required): Repository path.
- `component_type` (enum, required): `ApiService | AuthorizationService | GrpcService |
Subscriber | CacheService | Publisher | Activity | WorkflowSupportService`.
- `in_scope` (bool, required): Whether this feature must update it.
- `log_policy` (reference, required): Link to log-level and property-safety rules.

### Validation Rules

- Every in-scope component MUST have constructor-injected `ILogger<TComponent>`.
- Every in-scope component MUST emit at least one structured log event per mutating/event-handling
  method.
- Read methods emit logs only on denial/failure/empty-result outcomes.

## Entity: StructuredLogRule

Defines how operations are logged for each component/method category.

### Fields

- `operation_category` (enum): `Mutation | Read | Authorization | SubscriberEvent |
CacheOperation | PublishOperation | ActivityDispatch | WorkflowOutbox`.
- `success_level` (enum): `Debug | Information`.
- `denied_level` (enum, nullable): `Warning` for authorization denials.
- `failure_level` (enum): `Error`.
- `requires_exception_logging` (bool): If exception is caught/re-thrown, log first.

### Validation Rules

- Authorization denied MUST be `Warning`.
- Authorization granted MUST be `Information`.
- Cache miss/eviction MUST be `Debug`.
- Failure paths MUST log with exception context.

## Entity: SafeLogProperty

Represents allowable structured values.

### Fields

- `property_name` (string)
- `property_value_type` (enum): `Id | Count | Boolean | Enum | Outcome`
- `is_safe` (bool)

### Validation Rules

- `is_safe` MUST be true for every structured property.
- Disallowed categories: `Email | FullName | Token | ClaimValue | Password | Cookie |
Secret | RawPayload`.

### Safe Logging Guidance Examples

- Prefer safe structured properties:
  - `{TeamId}` with a team GUID/string ID
  - `{InvitationCount}` with integer count
  - `{Status}` with enum/string status value
  - `{IsAuthorised}` with boolean result
- Avoid unsafe structured properties:
  - `{Email}` full address
  - `{CustomerName}` full name
  - `{AccessToken}` token or credential
  - `{Claims}` raw identity claims payload

### Message Template Guidance

- Use consistent, outcome-oriented templates:
  - `"Team mutation completed for {TeamId}"`
  - `"Authorisation denied for {CustomerId} on {TeamId}"`
  - `"Cache miss for team {TeamId}"`
- Do not embed sensitive free text or raw payload dumps in templates.

## Entity: LoggingCoverageRecord

Tracks acceptance coverage for planning and task execution.

### Fields

- `component_name` (string)
- `method_name` (string)
- `operation_category` (enum)
- `expected_log_behavior` (string)
- `test_coverage_type` (enum): `Unit | Integration | None`.

### State Transitions

- `Planned` → `Implemented` → `Verified`.
- Transition to `Verified` requires:
  - code change merged in component
  - required tests updated/passing
  - safe-log property rule satisfied
