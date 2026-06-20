# Tasks: Skedular Competitor Comparison Hub

**Input**: Design documents from `specs/029-competitor-comparison-hub/`
**Prerequisites**: [plan.md](./plan.md), [spec.md](./spec.md), [research.md](./research.md), [data-model.md](./data-model.md), [contracts/](./contracts/), [quickstart.md](./quickstart.md)

**Tests**: Test tasks are included because the spec and plan require route, metadata, structured-data, link, evidence, and all-or-nothing publication validation.

**Organization**: Tasks are grouped by user story so each story can be implemented and validated as an independent increment after the shared foundation.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Confirm the existing public-web baseline and prepare comparison-specific data locations.

- [X] T001 Review the existing comparison implementation in `src/web/apps/public-web/src/data/comparison-pages.ts`, `src/web/apps/public-web/src/pages/compare/[slug].astro`, and `src/web/apps/public-web/src/components/FeatureComparisonSections.astro`
- [X] T002 Review existing public-web inventory and SEO flow in `src/web/apps/public-web/src/data/content-inventory.ts`, `src/web/apps/public-web/src/data/seo.ts`, `src/web/apps/public-web/src/pages/sitemap.xml.ts`, and `src/web/apps/public-web/src/pages/llms.txt.ts`
- [X] T003 [P] Create comparison data folder and barrel structure in `src/web/apps/public-web/src/data/comparison/index.ts`
- [X] T004 [P] Create comparison validation test section placeholders in `src/web/apps/public-web/tests/public-site-content.test.ts`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Define shared data contracts, validation helpers, and generation utilities required by every story.

**CRITICAL**: No user story work should begin until this phase is complete.

- [X] T005 Extend comparison-related content types for products, claims, evidence, feature categories, feature support, page targets, FAQs, and structured data in `src/web/apps/public-web/src/data/content-types.ts`
- [X] T006 [P] Define required feature categories and normalized feature rows in `src/web/apps/public-web/src/data/comparison/feature-matrix.ts`
- [X] T007 [P] Define comparison page path constants for hub, individual pages, supporting pages, and removed legacy routes in `src/web/apps/public-web/src/data/comparison/page-paths.ts`
- [X] T008 [P] Define shared comparison support-state labels and display helpers in `src/web/apps/public-web/src/data/comparison/support-states.ts`
- [X] T009 Create comparison data validation helpers for duplicate ids/slugs/paths, missing required routes, missing evidence, blocked claims, and incomplete publication in `src/web/apps/public-web/src/data/comparison/validation.ts`
- [X] T010 Add build/test diagnostics for comparison data validation failures in `src/web/apps/public-web/tests/public-site-content.test.ts`
- [X] T011 Wire comparison data exports into `src/web/apps/public-web/src/data/comparison/index.ts`

**Checkpoint**: Shared comparison model and validation scaffolding exist; user-story implementation can now begin.

---

## Phase 3: User Story 0 - Replace Existing One-Off Skedda Page (Priority: P0) MVP

**Goal**: Remove the current one-off comparison implementation with no redirect or alias, then start clean with generated comparison routes.

**Independent Test**: Confirm the old one-off comparison page data/route behavior is gone, `/compare` exists, and Skedular vs Skedda is reachable only through the new generated comparison system.

### Tests for User Story 0

- [X] T012 [P] [US0] Add a failing test that legacy comparison paths are not emitted, redirected, aliased, linked, or listed in `src/web/apps/public-web/tests/public-site-content.test.ts`
- [X] T013 [P] [US0] Add a failing test that `/compare` exists and links to `/compare/skedular-vs-skedda` in `src/web/apps/public-web/tests/public-site-content.test.ts`

### Implementation for User Story 0

- [X] T014 [US0] Replace current shallow comparison records with empty generated-target placeholders sourced from `src/web/apps/public-web/src/data/comparison/page-paths.ts` in `src/web/apps/public-web/src/data/comparison-pages.ts`
- [X] T015 [US0] Create the comparison hub route shell in `src/web/apps/public-web/src/pages/compare/index.astro`
- [X] T016 [US0] Update `src/web/apps/public-web/src/pages/compare/[slug].astro` to render only generated comparison page targets and not preserve legacy page behavior
- [X] T017 [US0] Verify no removed legacy comparison route is present in `src/web/apps/public-web/src/data/redirects.ts`, `src/web/apps/public-web/src/data/routes.ts`, `src/web/apps/public-web/src/data/navigation.ts`, or `src/web/apps/public-web/src/data/content-inventory.ts`
- [X] T018 [US0] Add route/removal diagnostics for legacy comparison paths in `src/web/apps/public-web/tests/public-site-content.test.ts`

**Checkpoint**: The legacy one-off page pattern is removed with no redirect/alias, and the new comparison section shell exists.

---

## Phase 4: User Story 1 - Establish Evidence-Based Comparison Data (Priority: P1)

**Goal**: Build the shared source of truth for Skedular evidence, competitor seed data, normalized features, feature support, FAQs, and content inventory.

**Independent Test**: Review the comparison data inventory and confirm every published Skedular capability has current evidence, every published competitor claim has evidence/review status, and no page has hardcoded comparison claims outside shared data.

### Tests for User Story 1

- [X] T019 [P] [US1] Add failing tests for required feature categories, normalized feature rows, unique feature ids, and complete product-feature support coverage in `src/web/apps/public-web/tests/public-site-content.test.ts`
- [X] T020 [P] [US1] Add failing tests for Skedular evidence requirements and competitor evidence/review status requirements in `src/web/apps/public-web/tests/public-site-content.test.ts`
- [X] T021 [P] [US1] Add failing tests that generated comparison content is sourced from shared data rather than page-local hardcoded claim arrays in `src/web/apps/public-web/tests/public-site-content.test.ts`

### Implementation for User Story 1

- [X] T022 [P] [US1] Create Skedular capability evidence records with current source references in `src/web/apps/public-web/src/data/comparison/skedular-evidence.ts`
- [X] T023 [P] [US1] Create competitor product seed records for Skedda, OfficeRnD, Nexudus, Gable, Robin, Officely, Envoy, Kadence, Archie, and deskbird in `src/web/apps/public-web/src/data/comparison/competitors.ts`
- [X] T024 [P] [US1] Create competitor claim records with evidence notes or explicit review status in `src/web/apps/public-web/src/data/comparison/competitor-claims.ts`
- [X] T025 [P] [US1] Create feature support records for Skedular and all competitors in `src/web/apps/public-web/src/data/comparison/feature-support.ts`
- [X] T026 [P] [US1] Create shared comparison FAQ records and schema eligibility flags in `src/web/apps/public-web/src/data/comparison/faqs.ts`
- [X] T027 [US1] Create page target generation utilities that combine products, claims, feature support, FAQs, and CTAs in `src/web/apps/public-web/src/data/comparison/page-targets.ts`
- [X] T028 [US1] Create content inventory generation helpers for generated comparison pages in `src/web/apps/public-web/src/data/comparison/content-inventory.ts`
- [X] T029 [US1] Wire comparison products, claims, evidence, support, FAQs, page targets, and inventory through `src/web/apps/public-web/src/data/comparison/index.ts`

**Checkpoint**: The shared dataset can generate reviewed comparison content and can fail validation when evidence or review state is missing.

---

## Phase 5: User Story 2 - Compare Skedular Against a Specific Competitor (Priority: P1)

**Goal**: Render each individual competitor comparison page with required sections, matrix data, metadata, FAQ schema, internal links, and CTA.

**Independent Test**: Open each required individual comparison URL and confirm required sections, competitor identity, matrix rows, pricing/integration comparison, FAQ schema, internal links, and CTA.

### Tests for User Story 2

- [X] T030 [P] [US2] Add failing tests that all ten individual comparison routes build and include one H1, metadata, canonical path, CTA, and `/compare` backlink in `src/web/apps/public-web/tests/public-site-content.test.ts`
- [X] T031 [P] [US2] Add failing tests that every individual comparison page renders Overview, Feature Matrix, Pricing Comparison, Integration Comparison, Best For, Limitations, Why Teams Choose Skedular, FAQ, and CTA sections in `src/web/apps/public-web/tests/public-site-content.test.ts`
- [ ] T032 [P] [US2] Add failing tests that visible FAQ text matches emitted FAQ schema on individual comparison pages in `src/web/apps/public-web/tests/public-site-content.test.ts`

### Implementation for User Story 2

- [ ] T033 [P] [US2] Expand `src/web/apps/public-web/src/components/FeatureComparisonSections.astro` to render overview, grouped feature matrix, pricing, integrations, best-for, limitations, Skedular reasons, FAQ, and CTA sections from a page target
- [X] T034 [P] [US2] Create comparison-specific section helpers or components in `src/web/apps/public-web/src/components/ComparisonSections.astro`
- [X] T035 [US2] Update `src/web/apps/public-web/src/pages/compare/[slug].astro` to select competitor comparison targets and pass page target data to shared comparison components
- [ ] T036 [US2] Add structured data graph generation for individual comparison pages in `src/web/apps/public-web/src/data/comparison/structured-data.ts`
- [X] T037 [US2] Wire individual comparison page targets into `src/web/apps/public-web/src/data/comparison-pages.ts`
- [ ] T038 [US2] Add individual comparison pages to public page inventory with metadata, canonical paths, structured-data types, and review flags in `src/web/apps/public-web/src/data/content-inventory.ts`

**Checkpoint**: Each individual competitor page is generated from shared data and independently validates against the page contract.

---

## Phase 6: User Story 3 - Browse the Comparison Hub (Priority: P2)

**Goal**: Publish `/compare` as the complete comparison index linking to every individual and supporting comparison page.

**Independent Test**: Open `/compare` and confirm every generated comparison and supporting page is linked with category context, summaries, and clear CTA navigation.

### Tests for User Story 3

- [X] T039 [P] [US3] Add failing tests that `/compare` lists and links to all ten individual comparison pages and all six supporting pages in `src/web/apps/public-web/tests/public-site-content.test.ts`
- [ ] T040 [P] [US3] Add failing tests that `/compare` includes category groupings, summaries, one H1, metadata, canonical path, structured data, and CTA links in `src/web/apps/public-web/tests/public-site-content.test.ts`

### Implementation for User Story 3

- [ ] T041 [P] [US3] Create hub grouping and summary data helpers in `src/web/apps/public-web/src/data/comparison/hub.ts`
- [X] T042 [US3] Implement the `/compare` hub page using generated page target data in `src/web/apps/public-web/src/pages/compare/index.astro`
- [ ] T043 [US3] Add `/compare` to public page inventory, sitemap inputs, and LLM page eligibility in `src/web/apps/public-web/src/data/content-inventory.ts` and `src/web/apps/public-web/src/data/seo.ts`
- [ ] T044 [US3] Add hub ItemList/Breadcrumb structured-data graph support in `src/web/apps/public-web/src/data/comparison/structured-data.ts`

**Checkpoint**: The hub is a complete index of the generated comparison section.

---

## Phase 7: User Story 4 - Discover Alternative and Best-Software Pages (Priority: P2)

**Goal**: Generate supporting SEO pages under `/compare` from the same competitor dataset and feature matrix.

**Independent Test**: Open each supporting page and confirm it uses shared data, links to detailed comparisons, includes SEO metadata, FAQ schema, structured data, and CTA.

### Tests for User Story 4

- [X] T045 [P] [US4] Add failing tests that all six supporting SEO routes build with `/compare` canonical paths and link to relevant individual comparison pages in `src/web/apps/public-web/tests/public-site-content.test.ts`
- [ ] T046 [P] [US4] Add failing tests that supporting pages use the same competitor dataset and normalized feature matrix as individual pages in `src/web/apps/public-web/tests/public-site-content.test.ts`
- [ ] T047 [P] [US4] Add failing tests that supporting page FAQ schema matches visible FAQ text in `src/web/apps/public-web/tests/public-site-content.test.ts`

### Implementation for User Story 4

- [X] T048 [P] [US4] Create supporting page target records for best-software and alternatives pages in `src/web/apps/public-web/src/data/comparison/supporting-pages.ts`
- [ ] T049 [US4] Extend `src/web/apps/public-web/src/pages/compare/[slug].astro` to render supporting page targets with comparison-list sections
- [ ] T050 [P] [US4] Add supporting page section rendering to `src/web/apps/public-web/src/components/ComparisonSections.astro`
- [ ] T051 [US4] Add supporting page structured data graph generation in `src/web/apps/public-web/src/data/comparison/structured-data.ts`
- [ ] T052 [US4] Add supporting pages to `src/web/apps/public-web/src/data/comparison-pages.ts` and `src/web/apps/public-web/src/data/content-inventory.ts`

**Checkpoint**: All supporting SEO pages are generated under `/compare` from the same data as individual pages.

---

## Phase 8: User Story 5 - Maintain and Extend Competitor Data (Priority: P3)

**Goal**: Make future competitor additions, feature updates, FAQ changes, and publication-state changes safe through shared data and validation.

**Independent Test**: Add a sample future competitor in validation and confirm the hub, page target, metadata, FAQ eligibility, structured data inputs, and related links update without page-specific duplicated copy.

### Tests for User Story 5

- [ ] T053 [P] [US5] Add a validation fixture test for adding a sample future competitor through shared data only in `src/web/apps/public-web/tests/public-site-content.test.ts`
- [ ] T054 [P] [US5] Add tests for unpublished competitor filtering from hub links, supporting pages, sitemap, and generated route targets in `src/web/apps/public-web/tests/public-site-content.test.ts`
- [ ] T055 [P] [US5] Add tests for renamed normalized feature labels propagating through matrix rendering without page-local updates in `src/web/apps/public-web/tests/public-site-content.test.ts`

### Implementation for User Story 5

- [ ] T056 [US5] Add maintainer-facing comments and examples for competitor additions in `src/web/apps/public-web/src/data/comparison/competitors.ts`
- [ ] T057 [US5] Add maintainer-facing comments and examples for feature support updates in `src/web/apps/public-web/src/data/comparison/feature-support.ts`
- [ ] T058 [US5] Add validation summary exports for content inventory review in `src/web/apps/public-web/src/data/comparison/validation.ts`
- [ ] T059 [US5] Update `src/web/apps/public-web/src/data/comparison/content-inventory.ts` to expose source records, matrix rows, metadata, FAQ entries, and structured-data types for every generated page

**Checkpoint**: A future competitor can be added through shared data without creating new one-off page templates.

---

## Phase 9: Polish & Cross-Cutting Concerns

**Purpose**: Validate the full comparison section and clean up documentation.

- [X] T060 Run app-local comparison validation using the `test` script in `src/web/apps/public-web/package.json`
- [X] T061 Run Astro checks using the `check` script in `src/web/apps/public-web/package.json`
- [X] T062 Run formatting validation using the `lint` script in `src/web/apps/public-web/package.json`
- [X] T063 Run static build validation with public-web URL variables using the `build` script in `src/web/apps/public-web/package.json`
- [ ] T064 Review built pages for mobile and desktop layout issues in `src/web/apps/public-web/dist/compare/index.html`
- [ ] T065 Update implementation notes and validation outcomes in `specs/029-competitor-comparison-hub/quickstart.md`
- [X] T066 Run `graphify update .` from the repository root to refresh `graphify-out/graph.json`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 Setup**: No dependencies.
- **Phase 2 Foundational**: Depends on Phase 1 and blocks all user-story phases.
- **Phase 3 US0**: Depends on Phase 2; MVP clean-start gate.
- **Phase 4 US1**: Depends on Phase 2; can begin after US0 if route cleanup is already underway, but publication depends on US0.
- **Phase 5 US2**: Depends on US1 shared dataset.
- **Phase 6 US3**: Depends on US1 page target data; final hub links depend on US2 and US4 page targets.
- **Phase 7 US4**: Depends on US1 shared dataset.
- **Phase 8 US5**: Depends on US1 validation helpers and page target model.
- **Phase 9 Polish**: Depends on all desired user stories and all-or-nothing publication validation.

### User Story Dependencies

- **US0 (P0)**: Must complete first for clean removal and no legacy redirect/alias behavior.
- **US1 (P1)**: Required before generated pages can make evidence-backed claims.
- **US2 (P1)**: Depends on US1 data; produces individual comparison pages.
- **US3 (P2)**: Depends on US1 and links to outputs from US2 and US4.
- **US4 (P2)**: Depends on US1 data; can run in parallel with US2 after shared page target helpers exist.
- **US5 (P3)**: Depends on US1 validation; can run after primary page generation is stable.

### Parallel Opportunities

- T003 and T004 can run in parallel after T001/T002 review.
- T006, T007, and T008 can run in parallel after T005 content types are agreed.
- T019, T020, and T021 can run in parallel because they validate different shared-data guarantees.
- T022 through T026 can run in parallel after foundational type definitions.
- US2 component work T033/T034 and structured-data work T036 can run in parallel after US1 page target data exists.
- US4 tests T045/T046/T047 can run in parallel before supporting page implementation.
- US5 tests T053/T054/T055 can run in parallel because they cover distinct maintainability guarantees.

---

## Parallel Example: User Story 1

```text
Task: "T019 [P] [US1] Add failing tests for required feature categories, normalized feature rows, unique feature ids, and complete product-feature support coverage in src/web/apps/public-web/tests/public-site-content.test.ts"
Task: "T020 [P] [US1] Add failing tests for Skedular evidence requirements and competitor evidence/review status requirements in src/web/apps/public-web/tests/public-site-content.test.ts"
Task: "T021 [P] [US1] Add failing tests that generated comparison content is sourced from shared data rather than page-local hardcoded claim arrays in src/web/apps/public-web/tests/public-site-content.test.ts"
Task: "T022 [P] [US1] Create Skedular capability evidence records with current source references in src/web/apps/public-web/src/data/comparison/skedular-evidence.ts"
Task: "T023 [P] [US1] Create competitor product seed records for Skedda, OfficeRnD, Nexudus, Gable, Robin, Officely, Envoy, Kadence, Archie, and deskbird in src/web/apps/public-web/src/data/comparison/competitors.ts"
```

## Parallel Example: User Story 2

```text
Task: "T030 [P] [US2] Add failing tests that all ten individual comparison routes build and include one H1, metadata, canonical path, CTA, and /compare backlink in src/web/apps/public-web/tests/public-site-content.test.ts"
Task: "T031 [P] [US2] Add failing tests that every individual comparison page renders Overview, Feature Matrix, Pricing Comparison, Integration Comparison, Best For, Limitations, Why Teams Choose Skedular, FAQ, and CTA sections in src/web/apps/public-web/tests/public-site-content.test.ts"
Task: "T033 [P] [US2] Expand src/web/apps/public-web/src/components/FeatureComparisonSections.astro to render overview, grouped feature matrix, pricing, integrations, best-for, limitations, Skedular reasons, FAQ, and CTA sections from a page target"
Task: "T034 [P] [US2] Create comparison-specific section helpers or components in src/web/apps/public-web/src/components/ComparisonSections.astro"
```

## Parallel Example: User Story 4

```text
Task: "T045 [P] [US4] Add failing tests that all six supporting SEO routes build with /compare canonical paths and link to relevant individual comparison pages in src/web/apps/public-web/tests/public-site-content.test.ts"
Task: "T046 [P] [US4] Add failing tests that supporting pages use the same competitor dataset and normalized feature matrix as individual pages in src/web/apps/public-web/tests/public-site-content.test.ts"
Task: "T048 [P] [US4] Create supporting page target records for best-software and alternatives pages in src/web/apps/public-web/src/data/comparison/supporting-pages.ts"
Task: "T050 [P] [US4] Add supporting page section rendering to src/web/apps/public-web/src/components/ComparisonSections.astro"
```

---

## Implementation Strategy

### MVP First

1. Complete Phase 1 and Phase 2.
2. Complete US0 to remove the existing one-off comparison page with no redirect or alias.
3. Complete US1 to establish the evidence-backed dataset and validation gate.
4. Complete US2 for all individual comparison pages.
5. Validate with app-local tests before adding hub/supporting-page polish.

### Full Publication Gate

The comparison section must not be treated as publishable until US0, US1, US2, US3, and US4 all pass validation together. The hub, all ten individual comparison pages, and all six supporting pages are an all-or-nothing required page set.

### Incremental Delivery

1. Clean-start route removal with US0.
2. Shared data and evidence model with US1.
3. Individual comparison page renderer with US2.
4. Hub index with US3.
5. Supporting SEO pages with US4.
6. Maintainer extension workflow with US5.

## Notes

- `[P]` tasks touch different files or independent test sections and can run in parallel after their dependencies.
- User story labels map to the spec story numbering, including `[US0]` for the clean-start removal story.
- Tests should be written to fail before implementation when the task says "Add failing tests".
- Static-site observability is implemented through build/test diagnostics and content inventory review, not runtime server logs.
- Do not add redirects or aliases for removed one-off comparison URLs.
