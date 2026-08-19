# Research: Web App Component Cleanup

## Decisions

### Route-rooted reachability

Use page, layout, route-handler, root-page, middleware/proxy, API, dynamic, and custom-domain entry points as roots. Navigation-only scans are insufficient because direct URLs and runtime loading paths may bypass visible links.

### Four-state classification

Classify each file or symbol as `used`, `conditionally-used`, `unused`, or `unresolved`. Unresolved candidates remain protected. This accounts for barrels, aliases, dynamic imports, test setup, route conventions, and workspace references.

### Protected route surface

Preserve route files and route-level tests by default, per clarification. The deletion set is limited to confirmed-unreachable components, component-only tests, and transitively orphaned application-owned support files.

### Transitive cleanup

Follow confirmed dead dependency chains through helpers, hooks, styles, fixtures, configuration, and tests. Delete a dependency only when it has no retained consumer; shared or ambiguous dependencies remain.

### Validation

Use each app’s existing `lint`, `test`, and `build` scripts, relevant Playwright suites, and before/after route comparison. No new cleanup framework or backend contract is required.
