# Tasks: Astro Public Website

**Input**: Design documents from `/specs/023-astro-public-website/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/public-website-contract.md, quickstart.md

**Tests**: Automated tests are required. Use Vitest with Testing Library DOM utilities for the rendered Astro page, axe for accessibility assertions, and scenario-driven tests for build diagnostics and required CTA configuration.

**Organization**: Tasks are grouped by user story so the visitor page, local developer workflow, and deployment workflow can be implemented and validated as separate increments.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel because it changes a different file and does not depend on an incomplete task
- **[Story]**: Maps a task to User Story 1, 2, or 3
- Every task includes an exact repository file path

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Scaffold the Astro app as a first-class package in the existing web workspace.

- [X] T001 Create the Astro package manifest with `dev`, `build`, `preview`, `start`, `check`, `test`, `lint`, `lint-fix`, and `format` scripts plus Astro, TypeScript, Vitest, Testing Library DOM, axe, and formatting dependencies in `src/web/apps/public-web/package.json`
- [X] T002 [P] Configure Astro for default static output with no deployment adapter in `src/web/apps/public-web/astro.config.mjs`
- [X] T003 [P] Add Astro client and typed `PUBLIC_SKEDULAR_SIGNUP_URL` environment declarations in `src/web/apps/public-web/src/env.d.ts`
- [X] T004 [P] Configure the Vitest environment and test file discovery for Astro page and build-diagnostics tests in `src/web/apps/public-web/vitest.config.ts`
- [X] T005 Install the new workspace package dependencies and record the resolved Astro test toolchain in `src/web/pnpm-lock.yaml`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish required CTA configuration, build observability, and workspace commands required by every story.

**CRITICAL**: No user story work should be considered complete until this phase is finished.

- [X] T006 Add a post-build diagnostics script that emits structured page count and static output size metadata without exposing environment values in `src/web/apps/public-web/scripts/report-build.mjs`
- [X] T007 [P] Add failing Vitest scenarios for structured build metadata, visible warning/error output, sensitive-value exclusion, and missing `PUBLIC_SKEDULAR_SIGNUP_URL` build failure in `src/web/apps/public-web/tests/build-diagnostics.test.ts`
- [X] T008 Wire required CTA validation, Astro diagnostics, static build diagnostics, tests, non-mutating lint, fixable lint, and formatting behavior into `src/web/apps/public-web/package.json`
- [X] T009 Add `public-web#dev`, `public-web#build`, `public-web#start`, `public-web#test`, `public-web#lint`, `public-web#lint-fix`, and `public-web#format` convenience commands in `src/web/package.json`
- [X] T010 Add matching `public-web#build`, `public-web#start`, `public-web#test`, `public-web#lint`, `public-web#lint-fix`, and `public-web#format` task definitions with `dist/**` build output tracking in `src/web/turbo.json`
- [X] T011 Run the build-diagnostics tests and complete the implementation until success metadata and failure-path output assertions pass in `src/web/apps/public-web/tests/build-diagnostics.test.ts`

**Checkpoint**: The app package has a required CTA configuration contract, observable builds, passing build-log tests, and participation in the pnpm/Turborepo task graph.

---

## Phase 3: User Story 1 - Visitor Gets a First Impression of Skedular (Priority: P1) MVP

**Goal**: Deliver one professional, static home page that explains Skedular's high-level value and provides a clear configured next step.

**Independent Test**: Build with `PUBLIC_SKEDULAR_SIGNUP_URL`, open `/`, and verify the first viewport identifies Skedular, explains hybrid workspace management, and links the CTA to the configured URL; confirm automated axe checks pass, the page remains useful with JavaScript disabled, and mobile widths do not scroll horizontally.

### Tests for User Story 1

- [X] T012 [US1] Add failing Vitest and Testing Library DOM scenarios for visible Skedular purpose, configured CTA URL, semantic landmarks, one page-level heading, descriptive link text, and no critical axe violations in `src/web/apps/public-web/tests/home-page.test.ts`

### Implementation for User Story 1

- [X] T013 [P] [US1] Add an accessible Skedular brand logo asset for the public site in `src/web/apps/public-web/public/images/skedular-logo-primary.svg`
- [X] T014 [P] [US1] Implement responsive global styling, visible focus states, sufficient contrast, and mobile overflow protection in `src/web/apps/public-web/src/styles/global.css`
- [X] T015 [US1] Implement the single semantic `/` route with title, meta description, required configured CTA, Skedular hero copy, desks/rooms/hybrid-team positioning, minimal feature highlights, and credible footer in `src/web/apps/public-web/src/pages/index.astro`
- [X] T016 [US1] Review and refine all visitor-facing copy for accuracy, minimal scope, American English, descriptive links, and absence of placeholder or unavailable-page links in `src/web/apps/public-web/src/pages/index.astro`
- [X] T017 [US1] Run the home-page test suite and complete the implementation until content, CTA, semantic, and axe assertions pass in `src/web/apps/public-web/tests/home-page.test.ts`

**Checkpoint**: User Story 1 is a complete, independently testable visitor-facing MVP.

---

## Phase 4: User Story 2 - Developer Runs the Public Website Locally (Priority: P2)

**Goal**: Make the public website maintainable through documented app-local and monorepo development commands.

**Independent Test**: From an installed `src/web` workspace, configure the CTA URL, start the app's dev server, change the home page, observe hot reload without restarting, run test/check/lint/build/format, and confirm the production preview serves the static site.

### Implementation for User Story 2

- [X] T018 [US2] Document prerequisites, required `PUBLIC_SKEDULAR_SIGNUP_URL`, install, local development, hot-reload editing, test, check, lint, format, build, preview, output directory, and future analytics integration location in `src/web/apps/public-web/README.md`
- [X] T019 [P] [US2] Add `public-web` to the web app topology and explain that it is a static public website rather than an authenticated product app in `src/web/apps/README.md`
- [X] T020 [US2] Validate and correct app-local command behavior for `test`, `check`, `lint`, `format`, `build`, and `preview` in `src/web/apps/public-web/package.json`
- [X] T021 [US2] Validate the Astro development server hot reloads a home-page content change without process restart and record the result in `specs/023-astro-public-website/quickstart.md`
- [X] T022 [US2] Validate and correct root convenience command participation for the new app in `src/web/package.json`
- [X] T023 [US2] Validate mismatched workspace dependency versions fail clearly through the existing version-sync tooling and record the result in `specs/023-astro-public-website/quickstart.md`

**Checkpoint**: User Story 2 is independently testable by a developer using only repository documentation.

---

## Phase 5: User Story 3 - Maintainer Deploys the Public Website to a Hosting Platform (Priority: P3)

**Goal**: Make the static output ready for Cloudflare Pages first and Vercel as a fallback without platform-specific runtime code.

**Independent Test**: Follow the documented settings to build `dist`, verify that the output is static-hosting ready for Cloudflare Pages and Vercel, and measure the under-two-second target against a deployed Cloudflare URL when one is available.

### Implementation for User Story 3

- [X] T024 [US3] Document Cloudflare Pages app-root and repository-root build commands, required CTA environment configuration, output directories, static hosting constraints, and deployed performance measurement procedure in `src/web/apps/public-web/README.md`
- [X] T025 [US3] Document Vercel monorepo root, build command, required CTA environment configuration, output directory, no-adapter requirement, and future server-runtime adapter boundary in `src/web/apps/public-web/README.md`
- [X] T026 [US3] Verify the production output remains adapter-free and contains only static deployable assets, correcting static output configuration if needed in `src/web/apps/public-web/astro.config.mjs`
- [X] T027 [US3] Measure the home page load time against the under-two-second target using the deployed Cloudflare URL, or record the unavailable-URL environment limitation without substituting a local-preview result, in `specs/023-astro-public-website/quickstart.md`

**Checkpoint**: User Story 3 is independently testable for static deployment readiness without requiring an actual deployment in this feature.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Validate the complete feature against the specification and protect existing web apps from regressions.

- [X] T028 Run the app-local test, check, lint, build, and preview quickstart validation commands with the required CTA configuration and record any environment-specific limitations in `specs/023-astro-public-website/quickstart.md`
- [X] T029 Run the workspace-level build, test, lint, format, and workspace version-sync commands and record any unrelated pre-existing failures in `specs/023-astro-public-website/quickstart.md`
- [X] T030 Perform the manual first-viewport, JavaScript-disabled, mobile-overflow, semantic-landmark, CTA, contrast, and placeholder-content acceptance review and record the result in `specs/023-astro-public-website/quickstart.md`
- [X] T031 Verify the final implementation does not modify backend contracts, generated artifacts, existing product app routes, or existing app dependencies, and record the scope check in `specs/023-astro-public-website/quickstart.md`
- [X] T032 Add staging and production Cloudflare Pages Terraform workspaces with custom domains in `src/web/apps/public-web/infrastructure/`
- [X] T033 Add `public-web` build, infrastructure validation, and Cloudflare Pages direct-upload deployment to `.github/workflows/skedular-cicd-pipeline.yml`
- [X] T034 Add Wrangler to the public web package and document Cloudflare token permissions and environment deployment settings in `src/web/apps/public-web/README.md`
- [X] T035 Validate the new Terraform workspaces, Astro production build, Wrangler installation, and deployment prerequisites, recording any credential limitations in `specs/023-astro-public-website/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies; start immediately.
- **Foundational (Phase 2)**: Depends on Setup completion and blocks completion of all user stories.
- **User Story 1 (Phase 3)**: Depends on Foundational; delivers the visitor-facing MVP.
- **User Story 2 (Phase 4)**: Depends on Foundational and can proceed in parallel with User Story 1 after the app scaffold exists.
- **User Story 3 (Phase 5)**: Depends on Foundational and can proceed in parallel with User Stories 1 and 2 after static build output exists.
- **Polish (Phase 6)**: Depends on all desired user stories being complete.

### User Story Dependencies

- **User Story 1 (P1)**: No dependency on another user story.
- **User Story 2 (P2)**: No dependency on another user story; its command and hot-reload validation become meaningful once the page exists.
- **User Story 3 (P3)**: No dependency on another user story; its deployment-readiness verification becomes meaningful once the static build contains the page.

### Within Each User Story

- User Story 1: Write failing page tests first; brand asset and CSS can then be created in parallel; implement the page; finish by passing tests and reviewing copy.
- User Story 2: App README and topology documentation can be drafted in parallel; then validate app-local commands, hot reload, root commands, and version-sync behavior.
- User Story 3: Cloudflare and Vercel documentation share one README and should be edited sequentially; static output and deployed-site performance verification follow the documentation.

---

## Parallel Opportunities

- T002, T003, and T004 can run in parallel after T001.
- T007 can run in parallel with workspace command work after T006 defines the diagnostics behavior.
- T013 and T014 can run in parallel after T012.
- T019 can run in parallel with T018 because it changes a separate documentation file.
- After Phase 2, different developers can work on User Stories 1, 2, and 3 concurrently, coordinating only where User Stories 2 and 3 both edit `src/web/apps/public-web/README.md`.

## Parallel Example: User Story 1

```text
Task T013: Add the brand logo asset in src/web/apps/public-web/public/images/skedular-logo-primary.svg
Task T014: Implement responsive styling in src/web/apps/public-web/src/styles/global.css
```

## Parallel Example: User Story 2

```text
Task T018: Document app-local workflows in src/web/apps/public-web/README.md
Task T019: Add public-web to src/web/apps/README.md
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1 to scaffold the package and test tooling.
2. Complete Phase 2 to establish required CTA configuration, static build diagnostics, and passing build-log tests.
3. Complete Phase 3 to deliver and test the visitor-facing home page.
4. Stop and validate User Story 1 independently before expanding deployment and maintenance documentation.

### Incremental Delivery

1. Deliver Setup + Foundational so the Astro package is a valid, observable workspace participant.
2. Deliver User Story 1 as the tested first public website MVP.
3. Deliver User Story 2 so developers can maintain the site through standard commands.
4. Deliver User Story 3 so maintainers can deploy the same static output to Cloudflare Pages or Vercel and measure Cloudflare performance when a URL exists.
5. Complete cross-cutting validation without changing existing product apps or generated surfaces.

## Notes

- Keep v1 to exactly one public route: `/`.
- Do not add Astro deployment adapters, SSR, API routes, React hydration, product authentication, Relay, or generated GraphQL artifacts.
- Do not add placeholder pages, placeholder copy, fallback CTA URLs, or links to unavailable pages.
- Preserve visible build warnings and errors; do not log environment variable values.
- An actual Cloudflare deployment is not required in this feature.
- All user-facing copy must use American spelling and grammar.
