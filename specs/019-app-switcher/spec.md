# Feature Specification: App Switcher

**Feature Branch**: `019-app-switcher`  
**Created**: 2026-06-01  
**Status**: Draft  
**Input**: User description: "now that I have three different apps, I need to have an app switcher to let user switch into different apps, either Skedular, Skedular Teams or Skedular Spaces. The app switcher should be available from each app to switch to other apps, the URL to what the other app URLs are provided through configuration. These apps are separated, but we ned to give them an option from within app to switch to other skedular apps"

## Clarifications

### Session 2026-06-01

- Q: Should the app switcher show only apps the user can access, or all configured apps? → A: Show all configured apps, and let destination apps handle denied access.
- Q: Should switching preserve current organization, tenant, or page context? → A: Navigate to the configured base URL only.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Switch Between Skedular Apps (Priority: P1)

As an authenticated user working in any Skedular app, I can open an app switcher and choose Skedular, Skedular Teams, or Skedular Spaces so that I can move to the right product without manually entering another address.

**Why this priority**: This is the core value of the feature. Users need a consistent way to move between the separated products from within the app they are already using.

**Independent Test**: Can be fully tested by opening each signed-in app, using the low-priority switcher from the existing navigation/menu area, selecting another configured app, and confirming the user is taken to the selected app.

**Acceptance Scenarios**:

1. **Given** a user is in Skedular and the other app URLs are configured, **When** the user opens the app switcher and selects Skedular Teams, **Then** the user is taken to the configured Skedular Teams URL.
2. **Given** a user is in Skedular Teams and all app URLs are configured, **When** the user opens the app switcher and selects Skedular Spaces, **Then** the user is taken to the configured Skedular Spaces URL.
3. **Given** a user is in Skedular Spaces and all app URLs are configured, **When** the user opens the app switcher and selects Skedular, **Then** the user is taken to the configured Skedular URL.
4. **Given** a user is viewing a specific page, organization, or tenant context in the current app, **When** the user selects another app, **Then** the user is taken to that app's configured base URL without attempting to carry over the current context.
5. **Given** a user is in a signed-in app, **When** the page shell renders, **Then** the app switcher is available as a secondary shortcut in the existing navigation/menu surface and is not rendered as a separate app bar, prominent header control, or first-class page action.

---

### User Story 2 - See Current App Context (Priority: P2)

As a user, I can clearly see which Skedular app I am currently using and which other apps are available so that I do not accidentally switch to the same product or the wrong product.

**Why this priority**: Clear app identity reduces navigation mistakes and makes the separated product experience understandable.

**Independent Test**: Can be tested by opening the switcher in each app and confirming the current app is identified while the other configured apps remain available for navigation.

**Acceptance Scenarios**:

1. **Given** a user is in Skedular Teams, **When** the user opens the app switcher, **Then** Skedular Teams is presented as the current app and Skedular and Skedular Spaces are presented as switch targets.
2. **Given** a user is in any Skedular app, **When** the user opens the app switcher, **Then** the app names are presented consistently as Skedular, Skedular Teams, and Skedular Spaces.

---

### User Story 3 - Handle Unavailable App Destinations (Priority: P3)

As a user, I receive a clear outcome when a destination app is not available in configuration so that I am not sent to a broken or unknown location.

**Why this priority**: Configuration may differ between environments or rollout stages, and missing destinations should fail gracefully.

**Independent Test**: Can be tested by running an app with one destination URL missing and confirming the switcher does not offer a broken navigation path.

**Acceptance Scenarios**:

1. **Given** a destination app URL is not configured, **When** the user opens the app switcher, **Then** that destination is not offered as an active switch target.
2. **Given** no other app URLs are configured, **When** the user views the app navigation area, **Then** the app switcher does not present unusable destinations.
3. **Given** a destination app is configured but the user may not have access to it, **When** the user opens the app switcher, **Then** the configured destination remains available and the destination app handles any denied access after navigation.

### Edge Cases

- The user is on a public website or customer-facing coworking-space subdomain, where the app switcher must not be shown.
- The current app URL is configured differently from the address the user is currently visiting.
- A configured destination URL is empty, malformed, or points to an unsupported location.
- Only one of the other Skedular apps is configured for the current environment.
- The user opens the switcher on a small screen or with zoomed text.
- The user chooses a destination while they have unsaved work in the current app.
- The destination app requires the user to sign in again or complete access checks after the user selects it.
- The user switches from a deeply nested page or tenant-specific view in the current app.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: Each Skedular app MUST provide an app switcher from a persistent, discoverable location within the signed-in app experience.
- **FR-002**: The app switcher MUST support the three app identities: Skedular, Skedular Teams, and Skedular Spaces.
- **FR-003**: The app switcher MUST use environment-provided destination URLs for each app rather than hardcoded production addresses.
- **FR-004**: Users MUST be able to switch from any configured app to any other configured Skedular app.
- **FR-005**: The app switcher MUST make the current app identity clear when the switcher is opened.
- **FR-006**: The app switcher MUST avoid presenting missing or invalid destination URLs as active navigation choices.
- **FR-007**: The app switcher MUST preserve a consistent app naming pattern across all three apps.
- **FR-008**: The app switcher MUST be usable with keyboard navigation and assistive technologies.
- **FR-009**: The app switcher MUST work on common desktop and mobile viewport sizes without overlapping or truncated app names.
- **FR-010**: When switching apps would leave unsaved user-entered changes behind, the current app MUST use its existing unsaved-change protection behavior before navigation proceeds.
- **FR-011**: If a destination app requires authentication or authorization, the user MUST follow that destination app's normal access flow after navigation.
- **FR-012**: Administrators or operators MUST be able to configure different app URLs per deployment environment.
- **FR-013**: The app switcher MUST show all configured app destinations even when the current app cannot determine whether the user has access to the destination.
- **FR-014**: The app switcher MUST navigate to the selected app's configured base URL and MUST NOT attempt to preserve the current page, organization, tenant, or workflow context.
- **FR-015**: The app switcher MUST be a secondary shortcut inside an existing app navigation/menu area and MUST NOT introduce a separate app bar or prominent page-level action.
- **FR-016**: The app switcher MUST NOT render in customer-facing coworking-space subdomain experiences or other customer storefront surfaces.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Feature MUST emit structured logs for start/completion of core workflows.
- **LOG-002**: Feature MUST emit structured logs for meaningful state transitions and branch decisions.
- **LOG-003**: Feature MUST emit actionable warning/error logs for failure and recovery paths.
- **LOG-004**: Feature logs MUST include correlation context (for example request/workflow identifiers)
  and MUST avoid sensitive data leakage.

### Key Entities _(include if feature involves data)_

- **Skedular App Destination**: A configured switch target representing one Skedular product, including its user-facing name, app identity, destination URL, and availability state.
- **Current App Context**: The app identity from which the user opens the switcher, used to distinguish the current product from switchable destinations.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 95% of users who open the app switcher can navigate to another configured Skedular app in 10 seconds or less.
- **SC-002**: In usability testing, at least 90% of participants can identify the current app and the available destination apps without guidance.
- **SC-003**: The switcher presents no active navigation choice for missing or invalid destination URLs in 100% of tested deployment configurations.
- **SC-004**: The switcher remains usable without text overlap or clipped app names across the supported desktop and mobile viewport range.
- **SC-005**: Support requests about finding the correct Skedular app decrease by 30% within one month of release.

## Assumptions

- The feature applies to signed-in areas of Skedular, Skedular Teams, and Skedular Spaces.
- Public website product discovery can link users into the three products, but the in-app switcher scope is limited to signed-in app navigation surfaces.
- Customer-facing coworking-space subdomains are excluded from this feature.
- Skedular, Skedular Teams, and Skedular Spaces are separate apps with separate configured base URLs.
- The initial switcher scope is navigation between the three named Skedular apps only.
- Destination apps remain responsible for their own authentication, authorization, and landing behavior.
- Existing app-level unsaved-change protection is reused when navigation away from the current app could lose work.
- Cross-app context preservation is out of scope for this feature.
