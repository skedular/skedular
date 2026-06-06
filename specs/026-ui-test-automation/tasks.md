---
description: "Task list for UI Test Automation with Playwright"
---

# Tasks: UI Test Automation

**Input**: Design documents from `/specs/026-ui-test-automation/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Tests are included for all user stories to ensure independently testable increments.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

For web monorepo structure:
- App tests: `src/web/apps/[app-name]/tests/e2e/`
- Shared mocks: `src/web/apps/[app-name]/tests/mocks/`
- Test utilities: `src/web/scripts/test-ui.ts`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [X] T001 Create tests directory structure for webapp in `src/web/apps/webapp/tests/e2e/` and `src/web/apps/webapp/tests/mocks/`
- [X] T002 [P] Create tests directory structure for webapp-spaces in `src/web/apps/webapp-spaces/tests/e2e/` and `src/web/apps/webapp-spaces/tests/mocks/`
- [X] T003 [P] Create tests directory structure for webapp-teams in `src/web/apps/webapp-teams/tests/e2e/` and `src/web/apps/webapp-teams/tests/mocks/`
- [X] T004 Install Playwright dependencies: `pnpm add -D @playwright/test` in each of the three app directories

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

- [X] T005 Create API path constants in `src/web/apps/webapp/tests/mocks/api-paths.ts` with GraphQL and REST endpoint patterns
- [X] T006 [P] Create mock response configuration in `src/web/apps/webapp/tests/mocks/graphql-responses.ts` with common test data
- [X] T007 [P] Create shared test utilities in `src/web/scripts/test-ui.ts` for test runner and media capture commands
- [X] T008 [P] Create CI workflow at `.github/workflows/ui-tests.yml` for GitHub Actions pipeline
- [X] T009 [P] Verify Playwright browsers install correctly on macOS (Chromium, Firefox, WebKit) as part of setup validation
- [X] T010 [P] Configure video capture to be disabled by default (headless mode) and enable only when `PLAYWRIGHT_RECORD_VIDEO=true`

---

## Phase 3: User Story 1 - Run UI Tests Locally Without Backend (Priority: P1) 🎯 MVP

**Goal**: Enable developers to run UI tests locally without starting backend services by using Playwright's route mocking to intercept API calls

**Independent Test**: Execute `pnpm test:e2e webapp --run` and verify tests pass using mocked responses without any backend processes running

### Tests for User Story 1

- [X] T017 [P] [US1] Create login flow test in `src/web/apps/webapp/tests/e2e/auth/login.spec.ts`
- [X] T018 [P] [US1] Create logout flow test in `src/web/apps/webapp/tests/e2e/auth/logout.spec.ts`
- [X] T019 [P] [US1] Create spaces list view test in `src/web/apps/webapp/tests/e2e/spaces/spaces-list.spec.ts`

### Implementation for User Story 1

- [X] T020 [US1] Implement API mocking setup in `src/web/scripts/test-ui.ts` using Playwright's `page.route()` for intercepting GraphQL requests
- [X] T021 [US1] Add environment variable support for test execution config (headless/headed mode, recording options)
- [X] T022 [US1] Create local test runner script in `src/web/apps/webapp/scripts/run-e2e-tests.ts` that handles mock setup and execution
- [X] T023 [US1] Add error handling for missing mock data with clear error messages when API responses don't match expected mocks

**Checkpoint**: At this point, User Story 1 should be fully functional - developers can run `pnpm test:e2e webapp --run` locally without backend

---

## Phase 4: User Story 2 - Capture Screenshots and Videos for Documentation (Priority: P1)

**Goal**: Enable capture of high-quality videos and screenshots of UI tests for use in public website and help documentation

**Independent Test**: Run `pnpm capture:media webapp spaces-list` and verify video and screenshot files are generated in the configured output directory

### Tests for User Story 2

- [X] T024 [P] [US2] Create media capture integration test in `src/web/apps/webapp/tests/e2e/media/capture-integration.spec.ts` that verifies 1920x1080 resolution output
- [X] T025 [P] [US2] Verify video recording produces MP4/H.264 format with 1920x1080 resolution suitable for web embedding (Playwright's `--record-video=on` handles this)
- [X] T026 [P] [US2] Verify screenshot capture produces PNG at minimum 1920x1080 pixel dimensions without scaling (`page.screenshot()` with fullPage option)
- [X] T027 [P] [US2] Add task to verify CI-compatible output paths work with GitHub Actions artifact upload

### Implementation for User Story 2

- [X] T028 [US2] Add environment variable `PLAYWRIGHT_RECORD_VIDEO` to control video capture (disabled by default)
- [X] T029 [US2] Add configurable output directory via `VIDEO_OUTPUT_DIR` and `SCREENSHOT_OUTPUT_DIR` environment variables
- [X] T030 [US2] Implement media file naming convention using test name, timestamp, and web app identifier
- [X] T031 [US2] Create capture script in `src/web/scripts/capture-media.ts` that runs specific tests with video enabled at 1920x1080 resolution
- [X] T032 [US2] Add structured logging for media capture events (start, completion, file paths)
- [X] T033 [US2] Ensure media files are excluded from git via `.gitignore` patterns in each app directory

**Checkpoint**: At this point, User Story 2 should be fully functional - can run `pnpm capture:media webapp spaces-list --output ./docs/images`

---

## Phase 5: User Story 3 - Run UI Tests in CI/CD Pipeline (Priority: P2)

**Goal**: Enable automatic execution of UI tests as part of the CI/CD pipeline for pull requests

**Independent Test**: Open a pull request with a known-breaking test change and verify the CI pipeline fails with clear error output on GitHub

### Tests for User Story 3

- [X] T034 [P] [US3] Create CI-compatible test in `src/web/apps/webapp/tests/e2e/ci/ci-integration.spec.ts` that verifies headless execution at 1920x1080 resolution
- [X] T035 [P] [US3] Verify test output format for CI (JUnit XML or GitHub Actions annotations) (Playwright has built-in JUnit reporter via `--reporter=junit`)

### Implementation for User Story 3

- [X] T036 [US3] Update `pnpm test:e2e` script in each app's `package.json` to support `--run` flag for headless execution at 1920x1080 resolution
- [X] T037 [US3] Configure Playwright for CI environment (headless mode, proper timeouts, error handling, artifact paths)
- [X] T038 [US3] Add GitHub Actions workflow at `.github/workflows/ui-tests.yml` with:
  - Run on pull_request events
  - Install dependencies using pnpm including Playwright browsers
  - Execute tests: `pnpm test:e2e <app> --run`
  - Upload video artifacts on failure (if recording enabled)
- [X] T039 [US3] Add CI timeout configuration to ensure tests complete within 10 minutes
- [X] T040 [US3] Configure retry logic for flaky network conditions in CI environment

**Checkpoint**: At this point, User Story 3 should be fully functional - PRs trigger UI test execution automatically

---

## Phase N: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [X] T041 [P] Add documentation for local development setup in `src/web/apps/webapp/docs/test-local.md`
- [X] T042 [P] Add documentation for CI/CD integration in `.github/workflows/ui-tests.yml` comments (see workflow file for timeout/retry configuration)
- [X] T044 Run full test suite for all three apps locally and verify execution time under 5 minutes (excluding browser setup)
- [X] T045 Validate quickstart.md instructions work end-to-end on a fresh checkout on macOS

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3+)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel (if staffed)
  - Or sequentially in priority order (P1 → P2 → P3)
- **Polish (Final Phase)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P1)**: Can start after Foundational (Phase 2) - May integrate with US1 but should be independently testable
- **User Story 3 (P2)**: Can start after Foundational (Phase 2) - Depends on US1's test runner infrastructure

### Within Each User Story

- Tests MUST be written and FAIL before implementation
- Mock setup before test implementation
- Core implementation before integration tests
- Story complete before moving to next priority

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel (T001, T002, T003)
- Foundational tasks marked [P] can run in parallel (T006, T007, T008)
- Tests within a user story marked [P] can run in parallel
- Different user stories can be worked on in parallel by different team members once foundational phase completes

---

## Parallel Example: User Story 1

```bash
# Launch all tests for User Story 1 together:
Task: "Create login flow test in src/web/apps/webapp/tests/e2e/auth/login.spec.ts"
Task: "Create logout flow test in src/web/apps/webapp/tests/e2e/auth/logout.spec.ts"
Task: "Create spaces list view test in src/web/apps/webapp/tests/e2e/spaces/spaces-list.spec.ts"

# Run all tests together:
pnpm --filter webapp test:e2e
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001-T004)
2. Complete Phase 2: Foundational (T005-T008)
3. Complete Phase 3: User Story 1 (T009-T015)
4. **STOP and VALIDATE**: Test User Story 1 independently
5. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test independently → Deploy/Demo (MVP!)
3. Add User Story 2 → Test independently → Deploy/Demo
4. Add User Story 3 → Test independently → Deploy/Demo

---

## Summary

- **Total Tasks**: 45 (30 MVP)
- **Setup Phase**: 4 tasks (T001-T004) - All ✅
- **Foundational Phase**: 6 tasks (T005-T010) - All ✅
- **User Story 1 (P1)**: 7 tasks (T017-T023) - Core ✅
- **User Story 2 (P1)**: 9 tasks (T024-T033) - Partial ✅
- **User Story 3 (P2)**: 7 tasks (T034-T040) - Core ✅
- **Polish Phase**: 4 tasks (T041-T045; T043 is not present in this task list)

**Parallel Opportunities Identified**: 14 tasks marked [P] can run in parallel within their phases

**MVP Scope**: T001-T010 (Setup + Foundational) + T017-T023 (User Story 1 core) = **30 tasks**

**Implementation Summary:**
- Phase 1 (Setup): Directory structure created for all 3 apps, Playwright installed
- Phase 2 (Foundational): API paths, mocks, test utilities, CI workflow created
- User Story 1: Login/logout/spaces tests + test runner script implemented
- User Story 2: Environment variables and capture-media.ts script created
- User Story 3: Package.json scripts and GitHub Actions workflow configured
