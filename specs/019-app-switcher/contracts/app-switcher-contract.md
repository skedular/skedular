# Contract: App Switcher

This is a frontend UI/configuration contract. It does not define a backend API and does not require generated code.

## Product Configuration Contract

Each product app must provide the current app id and base URLs for app switcher destinations.

```ts
type ProductAppId = 'webapp' | 'webapp-teams' | 'webapp-spaces';

type AppSwitcherConfiguration = {
  currentAppId: ProductAppId;
  destinations: {
    webapp?: string;
    'webapp-teams'?: string;
    'webapp-spaces'?: string;
  };
};
```

### Rules

- `currentAppId` must match the product app rendering the navigation/menu surface.
- Destination URL values must come from environment configuration.
- Destination URL values must be absolute HTTP or HTTPS base URLs.
- Missing destination URLs are allowed and result in no active switch target for that app.
- The current app may be included in `destinations`; it is used for current-app context, not as a switch target.
- Selection navigates to the configured base URL exactly as normalized by the shared model. No current page, organization, tenant, or workflow context is appended.

## Shared Model Contract

```ts
type AppSwitcherDestinationAvailability = 'available' | 'current' | 'missing-url' | 'invalid-url';

type AppSwitcherDestination = {
  appId: ProductAppId;
  displayName: 'Skedular' | 'Skedular Teams' | 'Skedular Spaces';
  href?: string;
  isCurrent: boolean;
  availability: AppSwitcherDestinationAvailability;
};

type AppSwitcherModel = {
  currentAppId: ProductAppId;
  destinations: readonly AppSwitcherDestination[];
  availableDestinationCount: number;
  hasSwitchTargets: boolean;
};
```

### Rules

- The destination list order is Skedular, Skedular Teams, Skedular Spaces.
- Active switch targets are destinations where `availability` is `available`.
- Inactive or invalid destinations must not render as active links.
- Valid configured destinations must remain visible as switch targets even when user access is unknown.

## Visual Behavior Contract

- The current app must be clearly identified when the switcher is opened.
- Active destinations must be keyboard reachable and have accessible names matching the app display names.
- The control must fit common desktop and mobile widths without clipped app names.
- The control must render as a secondary shortcut inside an existing app navigation/menu surface, not as a separate app bar, prominent header control, or primary page action.
- Customer-facing coworking-space subdomain/storefront surfaces must not render the switcher.
- If no active destinations exist, the navigation/menu surface must not present unusable switch targets.

## Logging Contract

```ts
type AppSwitcherLogEvent =
  | {
      event: 'web_app_switcher_configuration';
      currentAppId: ProductAppId;
      availableDestinationCount: number;
      invalidDestinationAppIds: readonly ProductAppId[];
      missingDestinationAppIds: readonly ProductAppId[];
      correlationId?: string;
    }
  | {
      event: 'web_app_switcher_selection';
      currentAppId: ProductAppId;
      destinationAppId: ProductAppId;
      correlationId?: string;
    };
```

### Rules

- Logs must not include authentication tokens, cookies, user-entered page data, or full sensitive URLs.
- Warning/error logs should be emitted when configured destination values are malformed.
- Selection logs should be emitted before navigation where feasible.
