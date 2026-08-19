# Data Model: Web App Component Cleanup

This feature has no persisted business data. These documentation entities define its audit evidence.

## Application Scope

`application`, `rootPath`, `routeRoots`, and `validationCommands`. The application must be one of `webapp`, `webapp-spaces`, `webapp-teams`, or `webapp-host`; route roots must be captured before deletion; validation must include lint, test, and build.

## Reachability Record

`path`, optional `symbol`, `classification`, `evidence`, `consumers`, and `deletionEligible`. Classification is `used`, `conditionally-used`, `unused`, or `unresolved`. Evidence must identify route/import/export/dynamic-load/test/workspace usage or explain the unresolved condition. Only confirmed unused app-owned records with no retained consumers are eligible.

## Cleanup Candidate

`path`, `reason`, `dependents`, and `protectedReferences`. Every dependent must independently have no retained consumer; any shared, ambiguous, route, or route-level-test reference blocks deletion.

## Validation Record

`application`, `routeComparison`, `lint`, `test`, `build`, relevant `e2e`, and `unresolved`. Protected routes must remain present and all cleanup-introduced validation failures must be resolved or documented.
