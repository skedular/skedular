# Tasks: Public Website Content Integration

**Input**: Design documents from `specs/025-public-website-content/`
**Prerequisites**: [plan.md](./plan.md), [spec.md](./spec.md), [research.md](./research.md), [data-model.md](./data-model.md), [contracts/public-website-contract.md](./contracts/public-website-contract.md), [quickstart.md](./quickstart.md)

**Tests**: Included because the specification requires environment validation, route/link validation, accessibility checks, build diagnostics, and reviewable content inventories.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare public-web structure and environment contract.

- [X] T001 Create planned public-web source folders in `src/web/apps/public-web/src/content/`, `src/web/apps/public-web/src/data/`, `src/web/apps/public-web/src/layouts/`, `src/web/apps/public-web/src/components/`, `src/web/apps/public-web/src/pages/resources/`, `src/web/apps/public-web/src/pages/support/`, `src/web/apps/public-web/src/pages/features/`, and `src/web/apps/public-web/src/pages/compare/`
- [X] T002 Update public URL typings for `PUBLIC_SKEDULAR_APP_URL`, `PUBLIC_SKEDULAR_SIGNUP_URL`, and `PUBLIC_SKEDULAR_DEMO_URL` in `src/web/apps/public-web/src/env.d.ts`
- [X] T003 Update local URL examples for the three required public destination URLs in `src/web/apps/public-web/.env.template`
- [X] T004 Update Docker build defaults to use non-production examples for all three required public destination URLs in `src/web/apps/public-web/Dockerfile`
- [X] T005 Update public-web README setup, validation, Cloudflare, and Vercel instructions for the three required public destination URLs in `src/web/apps/public-web/README.md`
- [X] T006 [P] Add static content type definitions for pages, resources, comparisons, claims, CTAs, and draft coverage in `src/web/apps/public-web/src/data/content-types.ts`
- [X] T007 [P] Add non-production URL fixture constants for tests in `src/web/apps/public-web/tests/public-url-fixtures.ts`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Shared content, route, metadata, URL, and validation foundations required before any story page work.

**Critical**: No user story work should start until this phase is complete.

- [X] T008 Implement required public destination URL validation and safe access helpers in `src/web/apps/public-web/src/data/public-destination-urls.ts`
- [X] T009 Update build diagnostics tests to require all three URL variables and hide URL values in `src/web/apps/public-web/tests/build-diagnostics.test.ts`
- [X] T010 [P] Create central CTA definitions that map search/book actions to app URL, login/sign-up actions to signup URL, and demo/contact actions to demo URL in `src/web/apps/public-web/src/data/ctas.ts`
- [X] T011 [P] Create central route definitions for Home, Teams, Spaces, Pricing, Resources, Support, Features, and Compare in `src/web/apps/public-web/src/data/routes.ts`
- [X] T012 [P] Create primary navigation and footer navigation data using route and CTA definitions in `src/web/apps/public-web/src/data/navigation.ts`
- [X] T013 Create shared site layout with semantic banner, main, footer, navigation, and CTA slots in `src/web/apps/public-web/src/layouts/SiteLayout.astro`
- [X] T014 [P] Create reusable SEO metadata and canonical URL helper in `src/web/apps/public-web/src/components/SeoHead.astro`
- [X] T015 [P] Create structured data helper for Organization, Product, FAQ, and Breadcrumb candidates in `src/web/apps/public-web/src/components/StructuredData.astro`
- [X] T016 [P] Create shared page section, CTA, card-grid, and resource-list components in `src/web/apps/public-web/src/components/`
- [X] T017 Update global responsive layout, typography, focus, contrast, navigation, card, search, pricing, and content styles in `src/web/apps/public-web/src/styles/global.css`
- [X] T018 Add cross-route semantic landmark, one-H1, metadata, CTA, and hardcoded-domain validation tests in `src/web/apps/public-web/tests/public-site-content.test.ts`
- [X] T019 Add content inventory validation helpers for pages, resources, comparisons, redirects, pricing, and draft coverage in `src/web/apps/public-web/tests/content-inventory.test.ts`

**Checkpoint**: Foundation ready. User-story pages can now be implemented and tested independently.

---

## Phase 3: User Story 1 - Visitor Finds and Books Workspace (Priority: P1) MVP

**Goal**: Home page presents Skedular as public workspace discovery and forwards booking/search actions to the configured app destination.

**Independent Test**: Build the app with all three URL variables, open `/`, verify first-viewport discovery/search messaging, supported resource categories, semantic layout, accessible CTAs, and booking/search links pointing to `PUBLIC_SKEDULAR_APP_URL`.

### Tests for User Story 1

- [X] T020 [P] [US1] Update home page tests for search entry, resource categories, app-forwarding CTA, and no hardcoded destination domains in `src/web/apps/public-web/tests/home-page.test.ts`
- [X] T021 [P] [US1] Add home page accessibility and metadata assertions to `src/web/apps/public-web/tests/public-site-content.test.ts`

### Implementation for User Story 1

- [X] T022 [P] [US1] Create home page content data for hero, search entry, discovery modules, product paths, why-Skedular, feature highlights, and social-proof readiness in `src/web/apps/public-web/src/data/home-page.ts`
- [X] T023 [P] [US1] Create reusable workspace search-entry component that submits or links to the configured app destination in `src/web/apps/public-web/src/components/WorkspaceSearchEntry.astro`
- [X] T024 [P] [US1] Create discovery and feature-highlight components for homepage sections in `src/web/apps/public-web/src/components/HomeSections.astro`
- [X] T025 [US1] Replace the existing single-page home implementation with the expanded layout and data-driven sections in `src/web/apps/public-web/src/pages/index.astro`
- [X] T026 [US1] Verify User Story 1 by running public-web tests with three URL variables and record any manual review notes in `specs/025-public-website-content/tasks.md`

**Checkpoint**: User Story 1 is independently functional as an MVP.

---

## Phase 4: User Story 2 - Organization Buyer Understands Skedular Teams (Priority: P2)

**Goal**: Teams page explains private workplace management for organizations and routes buyers to configured demo/pricing next steps.

**Independent Test**: Visit `/teams`, verify target audience, desk/room/parking/equipment, attendance, floor plans, analytics, Slack, Microsoft Teams, SSO, configured CTAs, and metadata.

### Tests for User Story 2

- [X] T027 [P] [US2] Add Teams page route, content, metadata, and CTA tests in `src/web/apps/public-web/tests/public-site-content.test.ts`

### Implementation for User Story 2

- [X] T028 [P] [US2] Create Teams product path content with audience, positioning, resource booking, workplace experience, collaboration, administration, and security sections in `src/web/apps/public-web/src/data/teams-page.ts`
- [X] T029 [P] [US2] Create product page section components for capability groups and integration callouts in `src/web/apps/public-web/src/components/ProductPageSections.astro`
- [X] T030 [US2] Implement the Teams page using the shared layout and Teams content data in `src/web/apps/public-web/src/pages/teams.astro`
- [X] T031 [US2] Add Teams route to navigation and footer data in `src/web/apps/public-web/src/data/navigation.ts`
- [X] T032 [US2] Verify User Story 2 by running public-web tests with three URL variables and record any manual review notes in `specs/025-public-website-content/tasks.md`

**Checkpoint**: User Story 2 works independently and does not require Spaces or Resources.

---

## Phase 5: User Story 3 - Workspace Operator Understands Skedular Spaces (Priority: P3)

**Goal**: Spaces page explains operator workflows for running, monetizing, billing, and publishing workspace inventory.

**Independent Test**: Visit `/spaces`, verify operator audience, resource management, product catalog, pricing, billing, payments, invoicing, tax, marketplace publishing, branding, configured CTAs, and metadata.

### Tests for User Story 3

- [X] T033 [P] [US3] Add Spaces page route, content, metadata, and CTA tests in `src/web/apps/public-web/tests/public-site-content.test.ts`

### Implementation for User Story 3

- [X] T034 [P] [US3] Create Spaces product path content with resource management, product management, payments, billing, finance, marketplace publishing, and branding sections in `src/web/apps/public-web/src/data/spaces-page.ts`
- [X] T035 [P] [US3] Extend product page section components for billing, finance, and operator workflow callouts in `src/web/apps/public-web/src/components/ProductPageSections.astro`
- [X] T036 [US3] Implement the Spaces page using the shared layout and Spaces content data in `src/web/apps/public-web/src/pages/spaces.astro`
- [X] T037 [US3] Add Spaces route to navigation and footer data in `src/web/apps/public-web/src/data/navigation.ts`
- [X] T038 [US3] Verify User Story 3 by running public-web tests with three URL variables and record any manual review notes in `specs/025-public-website-content/tasks.md`

**Checkpoint**: User Story 3 works independently and does not require Resources or Comparison pages.

---

## Phase 6: User Story 4 - Prospect Compares Pricing and Next Steps (Priority: P4)

**Goal**: Pricing page publishes the draft pricing model, suggested tiers, public booking model, and host commission range.

**Independent Test**: Visit `/pricing`, verify Teams active-user tiers, Spaces location tiers, public booking terms, host commission range, centralized pricing data, configured CTAs, and metadata.

### Tests for User Story 4

- [X] T039 [P] [US4] Add pricing page route, pricing-model, centralized-value, metadata, and CTA tests in `src/web/apps/public-web/tests/public-site-content.test.ts`

### Implementation for User Story 4

- [X] T040 [P] [US4] Create centralized pricing data for Teams tiers, Spaces tiers, public booking terms, and host commission range in `src/web/apps/public-web/src/data/pricing.ts`
- [X] T041 [P] [US4] Create pricing table and pricing-model components in `src/web/apps/public-web/src/components/PricingSections.astro`
- [X] T042 [US4] Implement the pricing page using centralized pricing data in `src/web/apps/public-web/src/pages/pricing.astro`
- [X] T043 [US4] Add pricing review status and pricing claim references to content inventory data in `src/web/apps/public-web/src/data/content-inventory.ts`
- [X] T044 [US4] Verify User Story 4 by running public-web tests with three URL variables and record any manual pricing review notes in `specs/025-public-website-content/tasks.md`

**Checkpoint**: User Story 4 works independently and pricing values are centralized.

---

## Phase 7: User Story 5 - Reader and Search Engine Discover Helpful Resources (Priority: P5)

**Goal**: Fully migrate and publish current blog/support content, preserve useful search intent, and provide resource/support indexes.

**Independent Test**: Inspect resource/support inventory, visit `/resources`, `/support`, and migrated article/support routes, verify every current public blog/support URL has a first-implementation destination or redirect to a published replacement, and confirm metadata/navigation.

### Tests for User Story 5

- [X] T045 [P] [US5] Add resource/support route coverage, metadata, and index tests in `src/web/apps/public-web/tests/public-site-content.test.ts`
- [X] T046 [P] [US5] Add current public blog/support migration and redirect-target validation tests in `src/web/apps/public-web/tests/content-inventory.test.ts`

### Implementation for User Story 5

- [X] T047 [P] [US5] Create current public blog/support source inventory with source URLs, titles, dates, summaries, topics, and destination paths in `src/web/apps/public-web/src/data/current-public-content.ts`
- [X] T048 [P] [US5] Create resource article content files for migrated current blog posts in `src/web/apps/public-web/src/content/resources/`
- [X] T049 [P] [US5] Create support article content files for migrated current support pages in `src/web/apps/public-web/src/content/support/`
- [X] T050 [P] [US5] Create resource/support rendering components with article metadata, related links, and CTAs in `src/web/apps/public-web/src/components/ResourceSections.astro`
- [X] T051 [US5] Implement resources index route in `src/web/apps/public-web/src/pages/resources/index.astro`
- [X] T052 [US5] Implement dynamic resource article route in `src/web/apps/public-web/src/pages/resources/[slug].astro`
- [X] T053 [US5] Implement support index route in `src/web/apps/public-web/src/pages/support/index.astro`
- [X] T054 [US5] Implement dynamic support article route in `src/web/apps/public-web/src/pages/support/[slug].astro`
- [X] T055 [US5] Create redirect inventory for current public blog/support URLs in `src/web/apps/public-web/src/data/redirects.ts`
- [X] T056 [US5] Wire Astro redirects for migrated current public URLs in `src/web/apps/public-web/astro.config.mjs`
- [X] T057 [US5] Add Resources and Support entries to navigation and footer data in `src/web/apps/public-web/src/data/navigation.ts`
- [X] T058 [US5] Verify User Story 5 by running public-web tests with three URL variables and record migration review notes in `specs/025-public-website-content/tasks.md`

**Checkpoint**: User Story 5 works independently and current public content has first-implementation destinations.

---

## Phase 8: User Story 6 - Product Team Confirms Full Draft Coverage (Priority: P6)

**Goal**: Provide complete section-by-section draft coverage, feature pages, comparison pages, SEO/AI discoverability, future-item handling, and review evidence.

**Independent Test**: Compare `public-website-content-draft.md` against coverage inventory, verify every heading/major bullet has a decision, visit feature and comparison routes, and confirm future items are not presented as current capabilities.

### Tests for User Story 6

- [X] T059 [P] [US6] Add draft coverage inventory completeness tests in `src/web/apps/public-web/tests/content-inventory.test.ts`
- [X] T060 [P] [US6] Add feature page, comparison page, structured-data, metadata, and future-item tests in `src/web/apps/public-web/tests/public-site-content.test.ts`

### Implementation for User Story 6

- [X] T061 [P] [US6] Create section-by-section draft coverage inventory for every heading and major bullet group in `src/web/apps/public-web/src/data/draft-coverage.ts`
- [X] T062 [P] [US6] Create feature page content for draft feature-page candidates in `src/web/apps/public-web/src/data/feature-pages.ts`
- [X] T063 [P] [US6] Create comparison page content for all draft comparison-page candidates in `src/web/apps/public-web/src/data/comparison-pages.ts`
- [X] T064 [P] [US6] Create capability claim and competitor claim review inventory in `src/web/apps/public-web/src/data/claim-review.ts`
- [X] T065 [P] [US6] Create future-feature planning inventory for community, mobile, AI, forecasting, and AI analytics items in `src/web/apps/public-web/src/data/future-features.ts`
- [X] T066 [P] [US6] Create feature and comparison rendering components in `src/web/apps/public-web/src/components/FeatureComparisonSections.astro`
- [X] T067 [US6] Implement dynamic feature page route in `src/web/apps/public-web/src/pages/features/[slug].astro`
- [X] T068 [US6] Implement dynamic comparison page route in `src/web/apps/public-web/src/pages/compare/[slug].astro`
- [X] T069 [US6] Add feature and comparison route families to sitemap/navigation data in `src/web/apps/public-web/src/data/routes.ts`
- [X] T070 [US6] Add structured data output to primary, resource, feature, and comparison pages in `src/web/apps/public-web/src/components/StructuredData.astro`
- [X] T071 [US6] Create launch review checklist data for draft coverage, pricing, claims, competitor review, accessibility, SEO, and human-quality tone in `src/web/apps/public-web/src/data/launch-review.ts`
- [X] T072 [US6] Verify User Story 6 by running public-web tests with three URL variables and record draft coverage and competitor review notes in `specs/025-public-website-content/tasks.md`

**Checkpoint**: User Story 6 works independently and proves complete draft coverage.

---

## Phase 9: Polish & Cross-Cutting Concerns

**Purpose**: Final validation, documentation, deployment readiness, and quality review across all stories.

- [X] T073 [P] Update `src/web/apps/public-web/README.md` with final route families, content inventory workflow, redirect expectations, and three-variable deployment examples
- [X] T074 [P] Update `src/web/apps/public-web/tests/build-diagnostics.test.ts` to assert build output page count increases and still excludes full public URL values
- [X] T075 [P] Run and fix formatting issues from `pnpm --dir src/web/apps/public-web lint`
- [X] T076 Run and fix Astro type issues from `PUBLIC_SKEDULAR_APP_URL=https://app.example.test PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book pnpm --dir src/web/apps/public-web check`
- [X] T077 Run and fix app-local test failures from `PUBLIC_SKEDULAR_APP_URL=https://app.example.test PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book pnpm --dir src/web/apps/public-web test`
- [X] T078 Run and fix production build failures from `PUBLIC_SKEDULAR_APP_URL=https://app.example.test PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book pnpm --dir src/web/apps/public-web build`
- [X] T079 Run workspace validation and record any unrelated failures from `PUBLIC_SKEDULAR_APP_URL=https://app.example.test PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book pnpm --dir src/web build`
- [X] T080 [P] Complete manual human-quality copy review across generated public pages and record findings in `src/web/apps/public-web/src/data/launch-review.ts`
- [X] T081 [P] Complete manual capability, pricing, and competitor claim review and record findings in `src/web/apps/public-web/src/data/claim-review.ts`
- [X] T082 [P] Complete manual accessibility and mobile-responsive review and record findings in `src/web/apps/public-web/src/data/launch-review.ts`
- [X] T083 [P] Complete Lighthouse/Core Web Vitals review against staging when available, or against a local static preview fallback, and record findings in `src/web/apps/public-web/README.md`
- [X] T084 [P] Create source/reference audit inventory for current public URLs, in-repository public content, competitor/reference URLs, review dates, and evidence notes in `src/web/apps/public-web/src/data/source-audit.ts`
- [X] T085 [P] Create privacy-safe analytics readiness metadata for page categories, CTA identifiers, route families, and future measurement notes without adding tracking scripts or hardcoded analytics vendors in `src/web/apps/public-web/src/data/analytics-readiness.ts`
- [X] T086 [P] Define manual review protocol, participant counts, review prompts, success-threshold fields, and evidence fields for the 90% review-participant success criteria in `src/web/apps/public-web/src/data/launch-review.ts`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 Setup**: No dependencies.
- **Phase 2 Foundational**: Depends on Phase 1; blocks all user story phases.
- **Phase 3 US1**: Depends on Phase 2; MVP.
- **Phase 4 US2**: Depends on Phase 2; can run after or alongside US1 once shared product components exist.
- **Phase 5 US3**: Depends on Phase 2; can run after or alongside US2 once shared product components exist.
- **Phase 6 US4**: Depends on Phase 2; independent of US2/US3 except shared layout/navigation.
- **Phase 7 US5**: Depends on Phase 2; can run in parallel with product pages after shared content patterns exist.
- **Phase 8 US6**: Depends on Phase 2 and benefits from US1-US5 route/content patterns; comparison and feature pages can begin once shared content models exist.
- **Phase 9 Polish**: Depends on selected user story phases being complete.

### User Story Dependencies

- **US1 (P1)**: No user-story dependencies; MVP.
- **US2 (P2)**: No hard dependency on US1; shares foundational layout/components.
- **US3 (P3)**: No hard dependency on US1/US2; shares product page components.
- **US4 (P4)**: No hard dependency on other stories; uses centralized CTA and pricing data.
- **US5 (P5)**: No hard dependency on product stories; uses content route foundation.
- **US6 (P6)**: Requires foundational inventories and should be finalized after all route families exist.

### Parallel Opportunities

- T006 and T007 can run in parallel during setup.
- T010 through T016 can run in parallel after T008 is defined.
- US2 and US3 data tasks can run in parallel after Phase 2.
- US5 content file creation tasks T047 through T050 can run in parallel.
- US6 inventory/content tasks T061 through T066 can run in parallel.
- Polish manual review and audit tasks T080 through T086 can run in parallel.

---

## Parallel Example: User Story 5

```text
Task: "T047 [P] [US5] Create current public blog/support source inventory with source URLs, titles, dates, summaries, topics, and destination paths in src/web/apps/public-web/src/data/current-public-content.ts"
Task: "T048 [P] [US5] Create resource article content files for migrated current blog posts in src/web/apps/public-web/src/content/resources/"
Task: "T049 [P] [US5] Create support article content files for migrated current support pages in src/web/apps/public-web/src/content/support/"
Task: "T050 [P] [US5] Create resource/support rendering components with article metadata, related links, and CTAs in src/web/apps/public-web/src/components/ResourceSections.astro"
```

## Parallel Example: User Story 6

```text
Task: "T061 [P] [US6] Create section-by-section draft coverage inventory for every heading and major bullet group in src/web/apps/public-web/src/data/draft-coverage.ts"
Task: "T062 [P] [US6] Create feature page content for draft feature-page candidates in src/web/apps/public-web/src/data/feature-pages.ts"
Task: "T063 [P] [US6] Create comparison page content for all draft comparison-page candidates in src/web/apps/public-web/src/data/comparison-pages.ts"
Task: "T064 [P] [US6] Create capability claim and competitor claim review inventory in src/web/apps/public-web/src/data/claim-review.ts"
Task: "T065 [P] [US6] Create future-feature planning inventory for community, mobile, AI, forecasting, and AI analytics items in src/web/apps/public-web/src/data/future-features.ts"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1 setup.
2. Complete Phase 2 foundation.
3. Complete Phase 3 US1.
4. Validate home page independently with app-local tests and build.
5. Stop and review before expanding into product, pricing, resource, and comparison content.

### Incremental Delivery

1. Deliver US1 as the public discovery homepage MVP.
2. Add US2 Teams.
3. Add US3 Spaces.
4. Add US4 Pricing.
5. Add US5 Resources and Support migration.
6. Add US6 full draft coverage, feature pages, and comparison pages.
7. Run Phase 9 polish and deployment readiness.

### Parallel Team Strategy

After Phase 2:

- Developer A: US1 and shared homepage/discovery components.
- Developer B: US2 and US3 product pages.
- Developer C: US4 pricing and CTA validation.
- Developer D: US5 resource/support migration.
- Developer E: US6 draft coverage, feature pages, and comparison pages.

---

## Notes

- Every task uses exact repository paths and checklist format.
- Tasks marked `[P]` touch separate files or independent content areas.
- User story tasks include `[US#]` labels for traceability.
- Direct public-site booking is out of scope; outbound app/search links must use `PUBLIC_SKEDULAR_APP_URL`.
- Do not hardcode staging or production destination domains.
- Keep public copy American, friendly, professional, specific, and human-written.

## Implementation Validation Notes

- 2026-06-05: `pnpm --dir src/web/apps/public-web lint` passed.
- 2026-06-05: `PUBLIC_SKEDULAR_APP_URL=https://app.example.test PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book pnpm --dir src/web/apps/public-web check` passed.
- 2026-06-05: `PUBLIC_SKEDULAR_APP_URL=https://app.example.test PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book pnpm --dir src/web/apps/public-web test` passed with 19 tests across 4 files.
- 2026-06-05: `PUBLIC_SKEDULAR_APP_URL=https://app.example.test PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book pnpm --dir src/web/apps/public-web build` passed and emitted `pageCount: 23`.
- 2026-06-05: `PUBLIC_SKEDULAR_APP_URL=https://app.example.test PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book pnpm --dir src/web build` completed the `public-web` build successfully, then stalled in unrelated Next.js help app builds and was terminated after no new output. No public-web failure was observed in the workspace run.
