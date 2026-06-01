# Research: App Switcher

## Decision: Keep App Identities in `@skedular/shared`

**Decision**: Extend the existing `ProductAppId` and product definition model in `@skedular/shared` as the canonical source for Skedular, Skedular Teams, and Skedular Spaces identities.

**Rationale**: The shared package already owns cross-product runtime modules and contains app-product definitions consumed across the split web products. Keeping app identity there avoids duplicated enum-like values in each app.

**Alternatives considered**:

- Define app switcher identities separately in each product app. Rejected because it creates drift risk.
- Put app identity in `@skedular/ui`. Rejected because UI should own presentation primitives, not runtime product semantics.

## Decision: Put Visual Switcher in `@skedular/ui`

**Decision**: Implement the reusable app switcher presentation in `@skedular/ui`, rendered by existing authenticated navigation/menu components in each product app.

**Rationale**: The constitution assigns visual primitives and app-shell UI to `@skedular/ui`. A single shared component keeps styling, accessibility, responsive behavior, and typography consistent across products while allowing each app to place the shortcut in its existing low-priority navigation surface.

**Alternatives considered**:

- Build separate switcher components in each app. Rejected because the switcher must be available consistently from every app.
- Put JSX UI in `@skedular/shared`. Rejected because `@skedular/shared` owns runtime modules and may depend on `@skedular/ui`, while `@skedular/ui` owns visual primitives.

## Decision: Use Product-App Environment Configuration

**Decision**: Each product app supplies the configured base URLs for all three destinations from its runtime environment, with a shared parser/model filtering missing or invalid values before rendering.

**Rationale**: The spec requires URLs to come from configuration. Product apps already own product-specific configuration such as analytics IDs, app URLs, and auth settings.

**Alternatives considered**:

- Hardcode production URLs. Rejected by FR-003 and because environments differ.
- Fetch switcher destinations from a backend service. Rejected because the feature does not require persisted state or backend contract changes.

## Decision: Show All Valid Configured Destinations

**Decision**: The switcher shows all valid configured app destinations even if the current app cannot determine whether the user has access to the destination.

**Rationale**: This captures the clarification decision. Destination apps remain responsible for sign-in, authorization, and denied access flows after navigation.

**Alternatives considered**:

- Hide destinations based on current-user access. Rejected because the selected behavior is to expose configured app destinations and defer access handling.
- Disable inaccessible destinations. Rejected because the current app is not required to evaluate destination access.

## Decision: Navigate to Configured Base URL Only

**Decision**: Switcher links navigate to each destination's configured base URL and do not preserve current page, organization, tenant, or workflow context.

**Rationale**: This captures the clarification decision and keeps the first app-switcher release focused on cross-app discovery rather than context mapping.

**Alternatives considered**:

- Preserve organization or tenant context. Rejected as out of scope.
- Preserve destination page intent where supported. Rejected as out of scope.

## Decision: Validate Destination URLs Before Rendering

**Decision**: Treat empty, malformed, unsupported, or same-current-app destination URLs as unavailable choices for rendering, while keeping the current app visible as context.

**Rationale**: The spec requires missing and invalid URLs not to appear as active navigation choices. Validation in shared code gives each product app the same behavior.

**Alternatives considered**:

- Render all configured strings as links. Rejected because malformed values create broken navigation paths.
- Fail the whole shell when one URL is invalid. Rejected because partial configuration should still allow valid destinations.

## Decision: Log Switcher Decisions Without Sensitive Data

**Decision**: Add structured client-side logging for app-switcher configuration filtering and user switch selection, including current app id, destination app id, availability result, and correlation context where available.

**Rationale**: The constitution requires operational logging for new feature behavior. These events help operators diagnose broken configuration and app-switch usage without exposing user-entered content or tokens.

**Alternatives considered**:

- Rely only on browser navigation analytics. Rejected because analytics does not capture invalid/missing configuration decisions reliably.
- Log full destination URLs. Rejected to reduce sensitive or environment-specific leakage; app id and availability are enough for feature diagnostics.
