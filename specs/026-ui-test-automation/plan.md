# Implementation Plan: UI Test Automation with Playwright

**Branch**: `026-ui-test-automation` | **Date**: 2026-06-06 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/026-ui-test-automation/spec.md`

## Summary

Add UI test automation using Playwright for three core web applications (`webapp`, `webapp-spaces`, `webapp-teams`). The automation must:

1. Run tests locally without backend services (using API mocking)
2. Capture videos and screenshots for documentation purposes
3. Support CI/CD pipeline execution via GitHub Actions

This is a frontend-only feature - no backend changes required.

## Technical Context

**Language/Version**: TypeScript 6 / React 19  
**Primary Dependencies**: Playwright (test framework), Vitest (existing test runner)  
**Storage**: N/A (UI tests run against mocks, media artifacts stored locally)  
**Testing**: Playwright for E2E UI tests with route mocking; Vitest for unit tests (existing)  
**Target Platform**: macOS (development), Linux (CI)
**Project Type**: Web application (Next.js App Router monorepo)  
**Performance Goals**: Test suite completes in under 5 minutes per app locally (excluding browser setup); media output at 1920x1080 HD resolution for majority desktop/laptop displays  
**Constraints**: No backend services required for local testing; CI execution within 10 minutes; macOS platform with automatic Playwright browser installation  
**Scale/Scope**: 3 web applications with ~80% core user scenario coverage

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — No API definition changes; UI tests only.
- [x] **II. Domain Boundaries** — Tests are cross-domain but use public HTTP interfaces (mocked).
- [x] **III. Testing** — E2E tier required for UI tests; uses Playwright's route mocking instead of repository layer assertions.
- [x] **IV. Frontend** — Tests follow existing Next.js App Router + Vitest pattern; no Relay changes needed.
- [x] **V. Pattern Consistency** — Uses established Playwright pattern (no new abstractions introduced).
- [x] **VI. Logging** — Tests emit structured logs for execution phases; video/screenshot capture logged.

## Project Structure

### Documentation (this feature)

```text
specs/026-ui-test-automation/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
src/web/
├── apps/
│   ├── webapp/
│   │   └── tests/              # New: Playwright test files
│   │       ├── e2e/            # End-to-end tests
│   │       ├── mocks/          # API response mocks
│   │       └── media/          # Captured videos and screenshots (1920x1080 HD)
│   ├── webapp-spaces/
│   │   └── tests/e2e/          # Playwright tests
│   └── webapp-teams/
│       └── tests/e2e/          # Playwright tests
├── packages/
│   └── ui-test-helpers/        # New: Shared test utilities (if needed)
└── scripts/
    ├── test-ui.ts              # Test runner script with Playwright browser verification
    └── capture-media.ts        # Media capture script (1920x1080 HD output)

.github/workflows/
└── ui-tests.yml                # CI workflow for UI tests
```

**Structure Decision**: Following the existing monorepo pattern, each webapp gets its own `tests/e2e/` directory. This matches the existing `src/test/` layout and keeps test code co-located with app code.

## Complexity Tracking

No violations - this feature follows existing patterns:
- Uses Vitest for unit tests (existing in each app)
- Extends Playwright for E2E tests (complementary to existing test setup)
- Follows monorepo structure already established
- No new packages or frameworks introduced
