# Tasks: Skedular Documentation Center

**Input**: Design documents from `/specs/033-documentation-center/`
**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/documentation-content-contract.md`, `quickstart.md`

**Tests**: Required by FR-025 and the public-web testing convention. Add catalog/unit and compiled-output tests before the implementation they verify.

**Organization**: Tasks are grouped by user story so each increment can be built and verified independently after the foundational documentation infrastructure is available.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Different files; can proceed in parallel once dependencies are complete.
- **[US#]**: Maps to the numbered user story in `spec.md`.

## Phase 1: Setup

**Purpose**: Establish the public-web documentation content and test locations.

- [X] T001 Create the documentation content directory structure in `src/web/apps/public-web/src/content/docs/{teams,spaces,host,shared}/`.
- [X] T002 [P] Create `src/web/apps/public-web/tests/documentation-content.test.ts` with a catalog-validation test harness and public URL fixtures.
- [X] T003 [P] Add documentation visual tokens, responsive layout hooks, and focus-state placeholders to `src/web/apps/public-web/src/styles/global.css`.

---

## Phase 2: Foundational

**Purpose**: Build the static documentation model, route resolution, shared presentation shell, and discovery integration that block all user stories.

**⚠️ CRITICAL**: Complete this phase before beginning story work.

- [X] T004 Define the typed Astro documentation content collection and front-matter validation in `src/web/apps/public-web/src/content.config.ts`.
- [X] T005 Define product/shared taxonomy, category ordering, article registry, evidence and terminology validation, canonical path helpers, related-article validation, replacement targets, and publication-state filtering in `src/web/apps/public-web/src/data/documentation.ts`.
- [X] T006 Implement static `/docs` product/shared-category/article path resolution, non-indexable withdrawn-page or replacement-redirect handling, and not-found handling in `src/web/apps/public-web/src/pages/docs/[...slug].astro`.
- [X] T007 [P] Implement reusable documentation navigation, category cards, article chrome, breadcrumbs, previous/next, and related-article components in `src/web/apps/public-web/src/components/{DocumentationNavigation,DocumentationCardGrid,DocumentationArticle,DocumentationLayout}.astro`.
- [X] T008 Extend the public route family and header/footer resource links with Documentation in `src/web/apps/public-web/src/data/{routes,navigation}.ts` and `src/web/apps/public-web/src/layouts/SiteLayout.astro`.
- [X] T009 Define the documentation-to-public-page discovery adapter and publication-state interface in `src/web/apps/public-web/src/data/{content-inventory,seo}.ts` so later registered articles have one discovery integration point; do not add page records until US1–US3 content exists.
- [X] T010 Add foundational catalog validation for duplicate routes, category ownership, evidence and terminology references, relationship/replacement targets, withdrawn-route exclusion from discovery, publication-state exclusion, and reserved future roots in `src/web/apps/public-web/tests/documentation-content.test.ts`.
- [X] T011 Add shared documentation layout styles for desktop, narrow screens, visible keyboard focus, and both color preferences in `src/web/apps/public-web/src/styles/global.css`.

**Checkpoint**: `/docs` can resolve a valid published catalog entry with shared metadata/navigation, while unpublished content is excluded from public routes and discovery output.

---

## Phase 3: User Story 1 — Choose the right product documentation (Priority: P1) 🎯 MVP

**Goal**: A public visitor can select Documentation from site navigation, understand the three product boundaries, and enter the correct product documentation area.

**Independent Test**: Build the public site, open `/docs`, and verify the three product cards, navigation link, product landing paths, metadata, and breadcrumb context without relying on product-app routes.

- [X] T012 [P] [US1] Add failing compiled-output assertions for `/docs`, `/docs/teams`, `/docs/spaces`, and `/docs/host` navigation, H1, canonical URL, and product boundary copy in `src/web/apps/public-web/tests/public-site-content.test.ts`.
- [X] T013 [US1] Create the Documentation home hub and product selection experience in `src/web/apps/public-web/src/pages/docs/index.astro`.
- [X] T014 [US1] Create published product landing content for Teams, Spaces, and Host in `src/web/apps/public-web/src/content/docs/{teams,spaces,host}/index.md`.
- [X] T015 [US1] Register product landing metadata, category summaries, product-boundary copy, and cross-links in `src/web/apps/public-web/src/data/documentation.ts`.
- [X] T016 [US1] Extend built-site navigation assertions to verify Documentation appears in header, mobile navigation, and footer/resource navigation in `src/web/apps/public-web/tests/public-site-content.test.ts`.

**Checkpoint**: User Story 1 is independently usable: a visitor can choose the correct product documentation without entering Teams, Spaces, or Host apps.

---

## Phase 4: User Story 2 — Get a product running (Priority: P1)

**Goal**: New Teams administrators, Spaces operators, and Hosts can complete accurate product-specific Getting Started journeys and continue to the next relevant guide.

**Independent Test**: Build the site and verify each Getting Started path contains a product-appropriate sequence, evidence references, a next step, breadcrumbs, and related links.

- [X] T017 [P] [US2] Add catalog and rendered-output tests for Getting Started category order, evidence, next-step links, and product-specific routes in `src/web/apps/public-web/tests/documentation-content.test.ts` and `src/web/apps/public-web/tests/public-site-content.test.ts`.
- [X] T018 [P] [US2] Write the complete Teams Getting Started guide in `src/web/apps/public-web/src/content/docs/teams/getting-started/get-started-with-teams.md`.
- [X] T019 [P] [US2] Write the complete Spaces Getting Started guide in `src/web/apps/public-web/src/content/docs/spaces/getting-started/get-started-with-spaces.md`.
- [X] T020 [P] [US2] Write the complete Host Getting Started guide in `src/web/apps/public-web/src/content/docs/host/getting-started/get-started-with-host.md`.
- [X] T021 [US2] Register the three guides, verified route evidence, ordered next/previous relationships, and related product/pricing links in `src/web/apps/public-web/src/data/documentation.ts`.

**Checkpoint**: All three Getting Started guides are complete, evidence-backed, and independently navigable.

---

## Phase 5: User Story 3 — Find trustworthy feature guidance (Priority: P1)

**Goal**: Every discovered live capability has a useful, product-scoped initial article or placeholder, and readers can browse categories and workflow relationships without unsupported claims.

**Independent Test**: Run catalog validation and confirm every approved capability maps to a published article/placeholder/shared article or an explicit non-public decision; inspect one article from every category for required placeholder content.

- [X] T022 [P] [US3] Add coverage-matrix tests requiring every Teams, Spaces, and Host capability inventory item to have exactly one coverage decision and every published placeholder to have evidence, scope, next step, and related content in `src/web/apps/public-web/tests/documentation-content.test.ts`.
- [X] T023 [P] [US3] Create a clearly labeled shared-concepts article that distinguishes cross-product terminology, then create Teams Core Features placeholder articles for organizations, locations/resources, zones/floor-plans, teams/users, and availability/analytics in `src/web/apps/public-web/src/content/docs/{shared/core-concepts/skedular-concepts,teams/core-features/{organizations,locations-and-resources,zones-and-floor-plans,teams-and-users,availability-and-analytics}}.md`.
- [X] T024 [P] [US3] Create Teams Bookings, Settings, Integrations, FAQs, and Best Practices articles in `src/web/apps/public-web/src/content/docs/teams/{bookings/private-bookings,settings/access-and-notifications,integrations/{slack,microsoft-teams,enterprise-sign-in},faqs/teams-faq,best-practices/workplace-rollout}.md`.
- [X] T025 [P] [US3] Create Spaces Core Features placeholder articles for marketplace setup, locations/resources, zones/floor-plans, products/pricing, publishing, customers, and analytics in `src/web/apps/public-web/src/content/docs/spaces/core-features/{marketplace-setup,locations-and-resources,zones-and-floor-plans,products-and-pricing,marketplace-publishing,customers,analytics}.md`.
- [X] T026 [P] [US3] Create Spaces Bookings and Settings articles for bookings, subscriptions, public-safe refunds, access, bank accounts, and payment connection in `src/web/apps/public-web/src/content/docs/spaces/{bookings/{bookings,subscriptions,refunds},settings/{access-and-organization,bank-accounts-and-payment-connection}}.md`.
- [X] T027 [P] [US3] Create Spaces Integrations, FAQs, and Best Practices articles for Slack, Microsoft Teams, enterprise sign-in, public-safe Xero guidance, commerce FAQs, and operator operations in `src/web/apps/public-web/src/content/docs/spaces/{integrations/{slack,microsoft-teams,enterprise-sign-in,xero-accounting},faqs/spaces-faq,best-practices/operator-operations}.md`.
- [X] T028 [P] [US3] Create Host Core Features placeholder articles for places/listings, pricing, availability/rules, cancellation, media/amenities, and draft/publication lifecycle in `src/web/apps/public-web/src/content/docs/host/core-features/{places-and-listings,pricing,availability-and-booking-rules,cancellation-policies,media-and-amenities,draft-and-publication}.md`.
- [X] T029 [P] [US3] Create Host Bookings, Settings, Integrations, FAQs, and Best Practices articles for bookings/renters, public-safe payments/refunds, organization/payment settings, payment connection, Host FAQs, and listing operations in `src/web/apps/public-web/src/content/docs/host/{bookings/{bookings-and-renters,payments-cancellations-and-refunds},settings/{organization-settings,payment-connection},integrations/payment-connection,faqs/host-faq,best-practices/listing-operations}.md`.
- [X] T030 [US3] Register all initial feature and shared-concept articles, empty-category boundary statements, evidence/terminology references, publication states, previous/next order, and related links in `src/web/apps/public-web/src/data/documentation.ts`.
- [X] T031 [US3] Add category-list rendering and useful placeholder messaging for all product category routes in `src/web/apps/public-web/src/pages/docs/[...slug].astro` and `src/web/apps/public-web/src/components/{DocumentationCardGrid,DocumentationArticle}.astro`.
- [X] T032 [US3] Add public-safe content review assertions for payment, refund, identity, accounting, integration, content-gap, and unsupported/future claim handling in `src/web/apps/public-web/tests/documentation-content.test.ts`.

**Checkpoint**: The complete initial capability inventory is covered and no placeholder is empty or presents unverified behavior as live.

---

## Phase 6: User Story 4 — Discover documentation through search (Priority: P2)

**Goal**: Published documentation has canonical metadata and participates correctly in public SEO and AI discovery, while non-published content remains excluded.

**Independent Test**: A production build contains canonical documentation pages in sitemap and LLM outputs, with unique metadata/structured data; draft/future/content-gap pages are absent.

- [X] T033 [P] [US4] Add failing discovery tests for documentation sitemap entries, robots eligibility, canonical URLs, LLM index entries, and publication-state exclusion in `src/web/apps/public-web/tests/{public-site-content,llms-content,documentation-content}.test.ts`.
- [X] T034 [US4] Connect registered published documentation entries to the T009 adapter, then add last-modified handling, metadata uniqueness, canonical URL generation, sitemap eligibility, and withdrawn-route discovery exclusion in `src/web/apps/public-web/src/data/{content-inventory,seo}.ts`.
- [X] T035 [US4] Add documentation breadcrumb and article structured-data rendering to `src/web/apps/public-web/src/components/{StructuredData,DocumentationLayout}.astro`.
- [X] T036 [US4] Add published documentation summaries and canonical links to `src/web/apps/public-web/src/pages/{llms.txt,llms-full.txt}.ts`.
- [X] T037 [US4] Verify documentation-specific sitemap/robots/LLM routes and rendered canonical metadata in `src/web/apps/public-web/tests/public-site-content.test.ts`.

**Checkpoint**: Search and AI discovery consume one authoritative published documentation inventory.

---

## Phase 7: User Story 5 — Use documentation on any device (Priority: P2)

**Goal**: The documentation shell remains keyboard-accessible, responsive, and readable in both supported color preferences.

**Independent Test**: At narrow and wide viewports, keyboard traversal reaches navigation, breadcrumbs, category links, article controls, and related articles in logical order with visible focus and no horizontal overflow.

- [X] T038 [P] [US5] Add DOM/axe assertions for documentation landmarks, heading order, navigation labels, breadcrumb semantics, link names, and focusable article controls in `src/web/apps/public-web/tests/documentation-content.test.ts`.
- [X] T039 [US5] Implement accessible landmarks, `aria-current` states, disclosure behavior, and descriptive labels in `src/web/apps/public-web/src/components/{DocumentationLayout,DocumentationNavigation,DocumentationArticle}.astro`.
- [X] T040 [US5] Complete responsive documentation grid/sidebar/article styles, reduced-width behavior, focus indicators, and color-mode contrast in `src/web/apps/public-web/src/styles/global.css`.
- [X] T041 [US5] Add compiled-output assertions for responsive documentation hooks and navigation control semantics in `src/web/apps/public-web/tests/public-site-content.test.ts`.

**Checkpoint**: Documentation reading and wayfinding work without a mouse and remain usable on small screens.

---

## Phase 8: User Story 6 — Maintain accurate content as Skedular evolves (Priority: P3)

**Goal**: Authors can add future documentation types without changing existing addresses or weakening metadata/content safety.

**Independent Test**: Catalog tests model an API, release-note, localized, versioned, media, and search extension entry while preserving existing article paths and excluding non-published entries.

- [X] T042 [P] [US6] Add validation fixtures for reserved API/release-note/version roots, locale/version/media extension metadata, and stable existing paths in `src/web/apps/public-web/tests/documentation-content.test.ts`.
- [X] T043 [US6] Add future-extension fields, reserved root definitions, and stable-path validation to `src/web/apps/public-web/src/data/documentation.ts` and `src/web/apps/public-web/src/content.config.ts`.
- [X] T044 [US6] Document content-authoring, the canonical terminology glossary and review record, evidence review, publication, non-indexable withdrawal/replacement behavior, and future-extension rules in `src/web/apps/public-web/src/content/docs/README.md`.
- [X] T045 [US6] Add an internal non-published content-gap/future-work inventory with explicit exclusion reasons in `src/web/apps/public-web/src/data/documentation.ts`.

**Checkpoint**: New documentation types can be modeled without route migration or accidental public publication.

---

## Phase 9: Polish and Cross-Cutting Validation

**Purpose**: Finish cross-story discovery, quality, diagnostics, and validation work.

- [X] T046 [P] Add build-time actionable validation records for duplicate paths, missing metadata/evidence/terminology, invalid related or replacement links, and inconsistent publication state in `src/web/apps/public-web/src/data/documentation.ts`; every record must include a safe failure category, article ID or public path, and build context without sensitive data.
- [X] T047 [P] Review all documentation copy for American English, consistent product terminology, founder-style tone, public safety, and natural links to relevant product/pricing/blog/comparison pages in `src/web/apps/public-web/src/content/docs/`.
- [X] T048 Run `pnpm --dir src/web/apps/public-web test` and resolve failures in `src/web/apps/public-web/tests/`.
- [X] T049 Run `pnpm --dir src/web/apps/public-web check`, `pnpm --dir src/web/apps/public-web lint`, and the environment-configured production build from `src/web/apps/public-web/README.md`.
- [ ] T050 Perform the recorded `/docs` keyboard, responsive, color-mode, metadata, sitemap, robots, LLM, product-evidence, and guided-usability review documented in `specs/033-documentation-center/quickstart.md`; use at least 10 new readers, record the 30-second product-selection task and next-action task, and compare results to SC-003/SC-004.

---

## Dependencies and Execution Order

```text
Phase 1 Setup
  -> Phase 2 Foundational
      -> US1 Product selection (MVP)
      -> US2 Getting Started
      -> US3 Feature guidance
      -> US4 Search discovery
      -> US5 Accessibility/responsiveness
      -> US6 Future extensibility
          -> Phase 9 Polish and validation
```

### User Story Dependencies

- **US1 (P1)** depends only on Foundational and is the MVP.
- **US2 (P1)** depends on the catalog/route foundation and can proceed in parallel with US1 after Phase 2.
- **US3 (P1)** depends on the catalog/route foundation; it can proceed in parallel with US1/US2, then shares final registry work with them.
- **US4 (P2)** depends on published documentation registry entries from US1–US3.
- **US5 (P2)** depends on the shared documentation components from Phase 2 and can run alongside US1–US4 once those components stabilize.
- **US6 (P3)** depends on the foundational catalog schema and can run in parallel with US4/US5.

## Parallel Opportunities

- After T004–T011, T013/T014, T018–T020, and the product content tasks T023–T029 can be split across contributors.
- The three Getting Started guides (T018–T020) and the Teams/Spaces/Host placeholder groups (T023–T029) touch separate content paths.
- US4, US5, and US6 use largely separate discovery, component/style, and schema/authoring files once published content is registered.

## Implementation Strategy

### MVP First

1. Complete Setup and Foundational phases.
2. Complete US1 and validate `/docs` plus the three product landing pages.
3. Add US2 Getting Started guides and validate each journey.
4. Add US3 catalog coverage before treating the documentation center as a production foundation.

### Incremental Delivery

1. Product selection and the shared route shell establish the first usable increment.
2. Getting Started guides supply the first complete help workflows.
3. Feature placeholders fill inventory coverage without unsafe assumptions.
4. Discovery, accessibility, and extensibility harden the foundation for public growth.

## Format Validation

All 50 implementation tasks use the required checkbox, sequential ID, optional parallel marker, story label for story phases, and explicit file-path format.
