# Feature Specification: Team Domain Structured Logging

**Feature Branch**: `001-team-domain-logging`  
**Created**: 2026-04-14  
**Status**: Draft  
**Input**: Add structured `ILogger<T>` logging to all service, subscriber, publisher, and activity
components in the team domain, following the established Enterprise.Shared Serilog patterns already
used in `LocationSubscriber` and `OrganizationSubscriber`.

---

## Clarifications

### Session 2026-04-14

- Q: What log level should authorization services use for denied vs granted outcomes? → A: `LogWarning` for denied outcomes, `LogInformation` for granted outcomes.
- Q: What log level should cache services use for cache-miss and cache-eviction events? → A: `LogDebug` for both cache-miss and cache-eviction events.
- Q: Should public read methods be logged? → A: Log read methods only on denial, failure, or empty-result outcomes.
- Q: Should `WorkaroundService` and `TeamGrpcService` be in scope? → A: Include both unconditionally; trivial pass-through methods may use `LogDebug` only.

---

## User Scenarios & Testing _(mandatory)_

### User Story 1 — Operational traceability for team and member mutations (Priority: P1)

A developer or operator investigating a production issue in the team domain can search structured
logs to trace any team creation, update, deletion, or team-member change. Each mutation operation
emits at least one structured log entry recording the outcome and relevant (safe) identifiers.

**Why this priority**: These are the most business-critical operations in the domain. Without log
coverage anyone diagnosing an incident — e.g. why a team was incorrectly deleted or why a member
role change did not apply — has no structured signal to work from.

**Independent Test**: Inject a mock `ILogger<TeamService>` and `ILogger<TeamMemberService>`,
perform an add/update/delete operation, and assert that at least one `LogInformation` or
`LogWarning` call was made carrying expected structured properties (no secret values).

**Acceptance Scenarios**:

1. **Given** a valid team-creation request, **When** `TeamService.AddAsync` completes successfully,
   **Then** a structured `LogInformation` entry is emitted containing the new team ID and customer
   ID (no sensitive data).
2. **Given** a valid team-deletion request, **When** `TeamService.DeleteAsync` completes,
   **Then** a `LogInformation` entry records the deleted team ID.
3. **Given** a batch member-status change, **When** `TeamMemberService.ChangeStatusAsync`
   completes, **Then** a `LogInformation` entry records the count of affected members and the
   requested status (not PII).
4. **Given** a member-role change, **When** `TeamMemberService.ChangeRoleAsync` completes,
   **Then** a `LogInformation` entry records the member ID and the new role.

---

### User Story 2 — Traceability for invitation lifecycle (Priority: P2)

An operator can use structured logs to follow a join-invitation from creation through
acceptance, rejection, or cancellation. Each state transition emits a log entry at the
appropriate level.

**Why this priority**: Invitation workflows span multiple actors (inviter and invitee) and
involve Temporal workflows. Production investigations frequently need to reconstruct the
invitation timeline.

**Independent Test**: Inject a mock `ILogger<InvitationService>` and exercise each lifecycle
method; assert that entries are emitted for each transition with invitation ID and team ID, but
no email address or personal data in the structured properties.

**Acceptance Scenarios**:

1. **Given** a team member sends invitations to new members, **When**
   `InvitationService.InviteMembersByEmailsAsync` completes, **Then** a `LogInformation` entry
   records the team ID and the count of invitations created (not the email addresses themselves).
2. **Given** an invitee accepts an invitation, **When** `InvitationService.AcceptInvitationToJoinAsync`
   completes, **Then** a `LogInformation` entry records the invitation ID and team ID.
3. **Given** an invitation is cancelled or rejected, **When** the corresponding service method
   completes, **Then** a `LogInformation` entry records the invitation ID and outcome.

---

### User Story 3 — Processor subscriber observability (Priority: P2)

Events processed by `CustomerSubscriber` emit structured log entries consistent with the
pattern already present in `LocationSubscriber` and `OrganizationSubscriber`. Stale or
ignored events are logged at `LogInformation` level with a clear message, not silently skipped.

**Why this priority**: All three event subscribers should have uniform log coverage.
`CustomerSubscriber` currently has none, creating a blind spot for customer-event processing.

**Independent Test**: Inject `ILogger<CustomerSubscriber>` and exercise each handled event
type including a stale-event scenario; assert that `LogInformation` is called with a
recognisable message and that no customer PII appears in structured properties.

**Acceptance Scenarios**:

1. **Given** a customer-upserted event with a timestamp older than the stored record, **When**
   `CustomerSubscriber.HandleAsync` is called, **Then** a `LogInformation` entry is emitted
   indicating the event was ignored as stale.
2. **Given** a valid customer-upserted event, **When** handling completes, **Then** at least
   one `LogInformation` entry records the event type and outcome.

---

### User Story 4 — Cache-layer and publisher observability (Priority: P3)

Cache-service and publisher components (`CachedTeamService`, `CachedOrganizationService`,
`CachedCustomerService`, `TeamOutboxPublisher`) log cache misses, evictions, and Kafka publish
outcomes at appropriate levels so operators can diagnose cache-related issues or event-publishing
failures without needing to attach a debugger.

**Why this priority**: Useful for operation, but lower risk than mutation and invitation
traceability. A cache miss or a publish failure may not surface as a user-visible error, making
logs the only signal.

**Independent Test**: Inject `ILogger<CachedTeamService>` and trigger a cache miss followed by
a successful refresh; assert at least one structured log entry is emitted. Inject
`ILogger<TeamOutboxPublisher>` and verify at least one entry per publish call.

**Acceptance Scenarios**:

1. **Given** a cache-miss on team lookup, **When** the value is fetched from the repository and
   stored, **Then** a `LogDebug` or `LogInformation` entry records the team ID and that a cache
   refresh occurred.
2. **Given** `TeamOutboxPublisher.PublishTeams` is called with a collection of teams, **Then**
   a `LogInformation` entry captures the count of events published and their type
   (upserted/deleted).

---

### Edge Cases

- Log entries MUST NOT contain email addresses, full names, tokens, passwords, cookie values,
  or any other credential-bearing or personally identifiable data in structured properties.
  The Serilog `SensitiveDataEnricherOptions` mask applies at transport level, but the code
  itself must not pass such values as structured log arguments.
- If an exception is caught and re-thrown, `LogError` with the exception MUST be the first
  action before the rethrow — no silent swallowing.
- Components that already have `ILogger<T>` (LocationSubscriber, OrganizationSubscriber) MUST
  NOT be changed unless their existing log statements are inconsistent with this spec.

---

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: Every public service class in `Team.Api/Services` (`TeamService`,
  `InvitationService`, `TeamMemberService`, `CustomerService`, `WorkaroundService`) MUST accept
  `ILogger<T>` as a constructor dependency and emit at least one structured log entry per
  mutating public method.

- **FR-001b**: `Team.Api/Grpc/TeamGrpcService` MUST accept `ILogger<T>` as a constructor
  dependency and emit structured logs for request handling outcomes. Trivial pass-through methods
  MAY use `LogDebug` only.

- **FR-001a**: Public read methods in `Team.Api/Services` and `Team.Api/Services/Authorization`
  MUST emit structured logs only for denial, failure, or empty-result outcomes. Routine successful
  reads MUST NOT require informational logging.

- **FR-002**: Every public service class in `Team.Api/Services/Authorization`
  (`OrganizationAuthorizationService`, `OrganizationOfferingService`,
  `OrganizationSsoAuthorizationService`, `TeamAuthorizationService`) MUST accept `ILogger<T>`
  and log authorisation decisions at `LogWarning` level for denied outcomes and `LogInformation`
  level for granted outcomes, without logging raw identity claims or token content.

- **FR-003**: `Team.Processors/Subscribers/CustomerSubscriber` MUST accept `ILogger<T>` and
  follow the same stale-event logging pattern as `LocationSubscriber` and
  `OrganizationSubscriber`.

- **FR-004**: `Team.Shared/Services/Cache` services (`CachedTeamService`,
  `CachedOrganizationService`, `CachedCustomerService`) MUST accept `ILogger<T>` and log
  cache-miss and cache-eviction events at `LogDebug` level.

- **FR-005**: `Team.Shared/Publishers/TeamOutboxPublisher` MUST accept `ILogger<T>` and log
  the count and type of events published per call.

- **FR-006**: `Team.Shared/Activities` (`InvitationIntegrations`, `EmailIntegrations`) and
  `Team.Shared/Services/TemporalOutboxService` MUST accept `ILogger<T>` and log activity
  dispatch and temporal-outbox enqueue events at `LogInformation` level.

- **FR-007**: No log statement anywhere in the team domain MUST pass email addresses, raw
  customer names, tokens, secrets, or credential-bearing values as structured log properties.
  Safe values are: IDs, counts, boolean flags, enum values, and operation outcomes.

- **FR-008**: When a new `ILogger<T>` parameter is added to a class whose unit tests exist, the
  corresponding unit test files MUST be updated to use `[AutoFakeItEasyData]` with SUT injection
  (or `[Frozen]` where a pre-configured dependency is required) consistent with the patterns
  documented in `Enterprise.Shared.UnitTests`.

- **FR-009**: No unit or integration test MUST construct a `NullLogger` or an ad hoc logger
  instance manually; all logger dependencies MUST flow through AutoFixture/FakeItEasy
  auto-mocking unless the test class already uses manual request-delegate construction.

### Key Entities

- **LoggableComponent**: Any non-test, non-generated C# class in the team domain that holds
  business logic and participates in mutations, event processing, caching, publishing, or
  workflow activity dispatch.
- **StructuredLogEntry**: An `ILogger<T>` call with a message template and structured parameters
  that contains only safe identifiers, counts, flags, or outcome strings.
- **SafeLogProperty**: An ID string, a count, a boolean, an enum value, or a descriptive outcome
  phrase — never a secret, token, credential, or PII field.

---

## Success Criteria _(mandatory)_

1. **Full component coverage**: Every loggable component in the team domain (as enumerated
   under FR-001 through FR-006) has at least one `ILogger<T>` log call per mutating or
   event-handling public method after this feature is complete.

2. **Secret-safe logs**: A code-review pass or linting rule can verify that no structured log
   argument in any team-domain file contains an email, token, password, claim value, or
   full-name field. Zero violations at merge time.

3. **Test suite remains green**: All existing unit and integration tests in `Team.Api.UnitTests`,
   `Team.Jobs.UnitTests`, `Team.Processors.UnitTests`, and `Team.Shared.UnitTests` pass without
   modification to test logic — only test setup changes required by the new `ILogger<T>` parameter
   are acceptable.

4. **Consistent pattern**: Every new `ILogger<T>` injection follows the primary-constructor
   parameter pattern already in use in `LocationSubscriber` — no service locator, no static
   logger, no manual `LoggerFactory` creation in production code.

5. **No regression on existing subscribers**: `LocationSubscriber` and `OrganizationSubscriber`
   behaviour and log output are unchanged.

---

## Assumptions

- The Serilog pipeline is already bootstrapped in `Team.Api/Program.cs` via
  `UseSerilogCustom(appName)` from `Enterprise.Shared.Logging`; no pipeline changes are needed.
- The same assumption holds for `Team.Jobs/Program.cs` and `Team.Processors/Program.cs`.
- `ILogger<T>` is available from the ASP.NET Core / .NET Generic Host DI container in all
  host projects without additional registration.
- `Team.Api/Services/WorkaroundService.cs` and `Team.Api/Grpc/TeamGrpcService.cs` are in scope
  for this feature. Trivial pass-through methods may receive `LogDebug` only.
- Workflow ID construction in `WorkflowIdService` is already deterministic and stateless;
  logging there is informational for debugging only (no mutation to log).
- The `TeamPublisher` (direct, non-outbox) is in scope if it is still actively used; if it is
  dead code it is explicitly out of scope.

---

## Out of Scope

- Changes to `Enterprise.Shared.Logging.SerilogExtensions` or the Serilog pipeline
  configuration.
- Changes to migration files, entity classes, or the `TeamDbContext`.
- Adding OpenTelemetry tracing spans (separate concern from structured logging).
- Modifying `LocationSubscriber` or `OrganizationSubscriber` unless a defect in their existing
  log statements is identified.
- Integration or system tests that specifically assert on log output; unit-test coverage of
  logging behaviour is sufficient.
