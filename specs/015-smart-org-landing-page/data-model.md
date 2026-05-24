# Data Model: Smart Organization Landing Page

**Feature**: 015-smart-org-landing-page

---

## Overview

This feature is **frontend-only**. No backend migrations or new domain entities are required.
All data is read from the existing GraphQL `myOrganizations` query, which returns
`MyOrganizationDetails` objects from the Organization API.

---

## Existing Types Used (no backend changes)

### `MyOrganizationDetails`

Returned by `myOrganizations(types: [OrganizationType!])` on the `Query` root.

| Field                | Type       | Nullable | Notes                                             |
| -------------------- | ---------- | -------- | ------------------------------------------------- |
| `name`               | `String!`  | no       | Display name of the organization                  |
| `uniqueId`           | `String!`  | no       | Stable ID — used to build the org navigation link |
| `customDomain`       | `String`   | yes      | Subdomain — used to build the org navigation link |
| `logoUrl`            | `String`   | yes      | Logo URL for `OrganizationAvatar`                 |
| `isMyOnboardingDone` | `Boolean!` | no       | Used to guard landing page rendering              |

---

## New Relay Fragments (per-app)

No new backend types. New Relay query/fragment shapes per app.

### `noOrganizationLandingPage_rootQuery` (new, per app landing page)

```graphql
query noOrganizationLandingPage_rootQuery {
  me {
    isOnboardingDone
    ...noOrganizationLandingContent_query
  }
}
```

> **Note**: The exact filter `types: [PRIVATE]` or `types: [MARKETPLACE]` is declared inside
> the `noOrganizationLandingContent_query` fragment, not in the page root query, to keep the
> filter co-located with the component that consumes it.

### `noOrganizationLandingContent_query` (new fragment, per app)

```graphql
fragment noOrganizationLandingContent_query on Query {
  myOrganizations(types: [PRIVATE]) {
    # MARKETPLACE for webapp-spaces
    name
    uniqueId
    customDomain
    logoUrl
  }
}
```

---

## UI State Model

The landing page content component renders one of three exclusive states based on
`myOrganizations.length`:

| State        | Condition                      | Rendered UI                                  |
| ------------ | ------------------------------ | -------------------------------------------- |
| `no-orgs`    | `myOrganizations.length === 0` | Create-org prompt with call-to-action button |
| `single-org` | `myOrganizations.length === 1` | Single org card with "Select" action         |
| `multi-org`  | `myOrganizations.length > 1`   | Scrollable list of org cards                 |

**Precondition**: If `me.isOnboardingDone === false`, the root shell redirect fires before this
component renders; guard with a `null` render or early return.

---

## Component Props (TypeScript interfaces)

### `NoOrganizationRootShell` (modified in all three apps)

```typescript
interface NoOrganizationRootShellProps {
  collapsed?: boolean; // existing
  hideSideNav?: boolean; // NEW — omits left nav from DOM entirely
  hideOrganizationSelector?: boolean; // existing (webapp only)
  hideWelcomeMessage?: boolean; // existing
  showBreadcrumps?: boolean; // existing
  breadcrumbs?: ReactNode; // existing
}
```

### `NoOrganizationLandingContent` (new component, per app)

```typescript
interface NoOrganizationLandingContentProps {
  queryRef: PreloadedQuery<noOrganizationLandingPage_rootQuery>;
}
```

The component reads the fragment internally via `usePreloadedQuery` +
`useFragment`.

---

## No Backend Changes

- No new EF Core migrations
- No new protobuf definitions
- No new OpenAPI endpoints
- No new gRPC methods
- `myOrganizations` and `OrganizationTypeDetails` already exist and are served through the
  existing Fusion gateway
