# Data Model: App Switcher

This feature introduces no persisted data. The model is runtime configuration and derived UI state.

## Product App Identity

Represents one of the fixed Skedular web products.

### Fields

- `id`: Stable product app id. Allowed values: `webapp`, `webapp-teams`, `webapp-spaces`.
- `displayName`: User-facing name. Allowed values: `Skedular`, `Skedular Teams`, `Skedular Spaces`.
- `shortName`: Compact label for constrained layouts.
- `purpose`: Existing product description used outside the switcher where applicable.

### Validation Rules

- `id` must be one of the fixed product app ids.
- `displayName` must use the canonical app names from the spec.
- User-facing names must use American spelling where relevant.

## Configured App URL

Represents an environment-provided base URL for a destination app.

### Fields

- `appId`: Product app identity the URL belongs to.
- `rawUrl`: Raw configured string supplied by the product app environment.
- `source`: Configuration source name, used for diagnostics without exposing secrets.

### Validation Rules

- `rawUrl` must be present to produce an active destination.
- `rawUrl` must parse as an absolute HTTP or HTTPS URL.
- The normalized URL is the configured base URL. Current organization, tenant, page, and workflow context are not appended.

## App Switcher Destination

Represents one item considered for the switcher.

### Fields

- `appId`: Destination product app id.
- `displayName`: Canonical destination name.
- `href`: Validated destination base URL when available.
- `isCurrent`: Whether this destination is the app currently rendering the shell.
- `availability`: `available`, `current`, `missing-url`, or `invalid-url`.

### Validation Rules

- Destinations with `missing-url` or `invalid-url` are not rendered as active navigation choices.
- The current app is clearly identified when the switcher is opened.
- All valid configured non-current destinations are rendered even when destination access is unknown.

## App Switcher Model

Represents the complete derived state passed to the visual switcher.

### Fields

- `currentAppId`: Product app id for the rendering app.
- `destinations`: Ordered list of app switcher destinations for Skedular, Skedular Teams, and Skedular Spaces.
- `availableDestinationCount`: Count of valid configured non-current destinations.
- `hasSwitchTargets`: Whether at least one active switch target exists.

### Relationships

- `App Switcher Model` contains three `App Switcher Destination` records.
- Each destination is derived from one `Product App Identity` and zero or one `Configured App URL`.

### State Transitions

1. Product app supplies current app id and configured URLs.
2. Shared model validates configured URLs.
3. Shared model marks destinations as current, available, missing, or invalid.
4. UI renders the current app context and active switch targets.
5. User selects an active destination and navigates to its configured base URL.
