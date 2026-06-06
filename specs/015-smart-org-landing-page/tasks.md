# Tasks: Smart Organization Landing Page

**Input**: Design documents from `/specs/015-smart-org-landing-page/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/ ✅

> **Scope note**: Organization selection panel (US1) and no-org create prompt (US2) apply to
> **webapp-teams** and **webapp-spaces** only. Left nav removal (US3) applies to all three
> apps; webapp's only change is passing `hideSideNav` on its landing page.
>
> **Spelling note**: All UI copy in this feature uses American spelling (user-approved override
> of constitution IV's British spelling requirement). Do not flag American spellings during PR
> review for this feature.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no incomplete-task dependencies)
- **[Story]**: US1, US2, or US3
- Exact file paths included in all task descriptions

---

## Phase 1: Setup (Shared Root Shell Changes)

**Purpose**: Add the `hideSideNav` prop to all three apps' `NoOrganizationRootShell`
components. This is the only cross-cutting prerequisite for all user stories. All three tasks
are parallel (different files).

- [X] T001 [P] Add `hideSideNav?: boolean` prop to `NoOrganizationRootShell`; wrap `<NoOrganizationLeftSideNavigationMenu>` render in `{!hideSideNav && ...}` to omit it from the DOM entirely when `true` in `web/apps/webapp/src/components/rootShell/no-organization-root-shell.tsx`
- [X] T002 [P] Add `hideSideNav?: boolean` prop to `NoOrganizationRootShell`; wrap `<NoOrganizationLeftSideNavigationMenu>` render in `{!hideSideNav && ...}` in `web/apps/webapp-teams/src/components/rootShell/no-organization-root-shell.tsx`
- [X] T003 [P] Add `hideSideNav?: boolean` prop to `NoOrganizationRootShell`; wrap `<NoOrganizationLeftSideNavigationMenu>` render in `{!hideSideNav && ...}` in `web/apps/webapp-spaces/src/components/rootShell/no-organization-root-shell.tsx`

**Checkpoint**: `hideSideNav` prop available on all three root shells — user story phases can now begin

---

## Phase 2: Foundational (Blocking Prerequisites)

No additional foundational tasks — Phase 1 is the only cross-cutting prerequisite.

---

## Phase 3: User Story 1 — Organization Selection Panel (Priority: P1) 🎯 MVP

**Goal**: Signed-in users with one or more organizations see a centered org-selection panel on
the landing pages of webapp-teams and webapp-spaces, replacing the generic static message.

**Scope**: webapp-teams and webapp-spaces only. webapp is unaffected by US1.

**Independent Test**: Sign in as a user with one or more organization memberships, navigate to
`/` in webapp-teams or webapp-spaces. Confirm: centered panel (`maxWidth: 760`, `p: { xs: 2, md: 4 }`) showing org cards (each with `OrganizationAvatar`, org name via `LeadIconTypography`); clicking a card navigates to the org home; "Create organization" action is present; left nav is absent from the DOM; a loading spinner appears while the query is in flight.

### Tests for User Story 1

- [X] T004a [P] [US1] Write unit tests for webapp-teams `NoOrganizationLandingContent`: cover loading state, `myOrganizations.length === 0` (no-org), `length === 1` (single-org), `length > 1` (multi-org); assert correct heading, CTA button, org card rendering, and navigation link href per state in `web/apps/webapp-teams/src/components/noOrganizationLanding/no-organization-landing-content.test.tsx`
- [X] T005a [P] [US1] Write unit tests for webapp-spaces `NoOrganizationLandingContent`: same coverage as T004a with `types: [MARKETPLACE]` and spaces-specific create-org link in `web/apps/webapp-spaces/src/components/noOrganizationLanding/no-organization-landing-content.test.tsx`

### Implementation for User Story 1

- [X] T004 [P] [US1] Create `NoOrganizationLandingContent` component in webapp-teams: declare Relay fragment `noOrganizationLandingContent_query` on `Query` fetching `myOrganizations(types: [PRIVATE]) { name, uniqueId, customDomain, logoUrl }`; render has-orgs state as a centered `StackColumn` (`maxWidth: 760`, `p: { xs: 2, md: 4 }`) listing org cards — each card uses `StackRow` + `OrganizationAvatar` + `LeadIconTypography` (org name); card click calls `router.push(getOrganizationBaseLink(integratedPlatform, org.uniqueId))`; include "Create organization" `Button` linking to `getOrganizationSetupLink(integratedPlatform)`; create barrel `index.ts`; **SC-004 acceptance**: outer wrapper must use `maxWidth: 760` and `p: { xs: 2, md: 4 }` — confirm against other single-column pages; files: `web/apps/webapp-teams/src/components/noOrganizationLanding/no-organization-landing-content.tsx` and `web/apps/webapp-teams/src/components/noOrganizationLanding/index.ts`
- [X] T005 [P] [US1] Create `NoOrganizationLandingContent` component in webapp-spaces: identical structure to T004 but fragment uses `myOrganizations(types: [MARKETPLACE])`; `getOrganizationSetupLink` routes to `/organizations/add-marketplace`; **SC-004 acceptance**: same `maxWidth: 760` and `p` constraints as T004; files: `web/apps/webapp-spaces/src/components/noOrganizationLanding/no-organization-landing-content.tsx` and `web/apps/webapp-spaces/src/components/noOrganizationLanding/index.ts`
- [X] T006 [US1] (after T004) Refactor webapp-teams landing page: add page-level `noOrganizationLandingPage_rootQuery` (spreads `...noOrganizationLandingContent_query`); replace static content with `useQueryLoader` + `loadQuery` in `useEffect`; pass `hideSideNav` to `<NoOrganizationRootShell>`; render `<Suspense fallback={<CircularProgress />}><NoOrganizationLandingContent queryRef={queryRef} /></Suspense>`; **FR-009 acceptance**: confirm existing root shell onboarding redirect still fires before landing content renders in `web/apps/webapp-teams/src/rootPages/page.tsx`
- [X] T007 [US1] (after T005) Refactor webapp-spaces landing page: same pattern as T006 with spaces-specific query; **FR-009 acceptance**: confirm onboarding redirect preserved in `web/apps/webapp-spaces/src/rootPages/page.tsx`
- [X] T008 [P] [US1] Run `pnpm relay` in `web/apps/webapp-teams/` to regenerate typed Relay artifacts for `noOrganizationLandingPage_rootQuery` and `noOrganizationLandingContent_query` under `web/apps/webapp-teams/src/queries/__generated__/`; commit the generated files
- [X] T009 [P] [US1] Run `pnpm relay` in `web/apps/webapp-spaces/` to regenerate typed Relay artifacts under `web/apps/webapp-spaces/src/queries/__generated__/`; commit the generated files

**Checkpoint**: US1 fully functional — org panel renders live data, org card navigation works, left nav absent from DOM, loading state visible while query is in flight

---

## Phase 4: User Story 2 — No-Organization Create Prompt (Priority: P2)

**Goal**: Signed-in users with zero organization memberships see a centered create-org prompt
instead of the org panel on the landing pages of webapp-teams and webapp-spaces.

**Scope**: webapp-teams and webapp-spaces only. webapp is unaffected by US2.

**Independent Test**: Sign in as a user with zero org memberships, navigate to `/` in
webapp-teams or webapp-spaces. Confirm: centered prompt with heading, description text, and a
"Create organization" button; button navigates to `/organizations/add-private` (teams) or
`/organizations/add-marketplace` (spaces); left nav absent; layout matches `maxWidth: 760` constraint.

### Implementation for User Story 2

- [X] T010 [P] [US2] Add `myOrganizations.length === 0` render branch to `NoOrganizationLandingContent` in webapp-teams: centered `StackColumn` with heading (`LeadIconTypography`), description (`BodyIconTypography`), and `Button` linking to `getOrganizationSetupLink(integratedPlatform)` in `web/apps/webapp-teams/src/components/noOrganizationLanding/no-organization-landing-content.tsx`
- [X] T011 [P] [US2] Add `myOrganizations.length === 0` render branch to `NoOrganizationLandingContent` in webapp-spaces: same structure with spaces-appropriate copy and `getOrganizationSetupLink()` link in `web/apps/webapp-spaces/src/components/noOrganizationLanding/no-organization-landing-content.tsx`

**Checkpoint**: US2 fully functional — zero-org users see the create-org prompt; US1 and US2 both independently testable

---

## Phase 5: User Story 3 — Remove Left Nav on Landing Pages (Priority: P2)

**Goal**: Left-side navigation is fully absent from the DOM on the root landing page of all
three apps when no organization is selected.

**Scope — webapp only** (webapp-teams and webapp-spaces already pass `hideSideNav` via T006/T007 in Phase 3).

**Independent Test**: Navigate to `/` as a signed-in user in webapp. Inspect the DOM — confirm `NoOrganizationLeftSideNavigationMenu` is not present. Navigate to any org-scoped page — confirm the left nav renders normally.

### Implementation for User Story 3

- [X] T012 [US3] Pass `hideSideNav` to `<NoOrganizationRootShell>` on the webapp landing page in `web/apps/webapp/src/rootPages/page.tsx`

**Checkpoint**: Left nav absent from the DOM on all three apps' landing pages; non-landing pages unaffected

---

## Final Phase: Polish & Cross-Cutting Concerns

- [X] T013 [P] Add `useEffect` structured log on mount in webapp-teams `NoOrganizationLandingContent`: log `{ event: 'org_landing_state', state: 'no-orgs' | 'single-org' | 'multi-org', orgCount: number, userId: hashedUserId }` per LOG-001–LOG-003 in `web/apps/webapp-teams/src/components/noOrganizationLanding/no-organization-landing-content.tsx`
- [X] T014 [P] Add `useEffect` structured log on mount in webapp-spaces `NoOrganizationLandingContent` per LOG-001–LOG-003 in `web/apps/webapp-spaces/src/components/noOrganizationLanding/no-organization-landing-content.tsx`
- [X] T013a [P] Add error handler to `noOrganizationLandingPage_rootQuery` in webapp-teams landing page: emit warning log with error context when the query fails per LOG-002 in `web/apps/webapp-teams/src/rootPages/page.tsx`
- [X] T014a [P] Add error handler for query failure in webapp-spaces landing page per LOG-002 in `web/apps/webapp-spaces/src/rootPages/page.tsx`
- [X] T015 Verify `<CircularProgress />` (or equivalent loading indicator) renders while `noOrganizationLandingPage_rootQuery` is in flight per FR-008 in both webapp-teams (`web/apps/webapp-teams/src/rootPages/page.tsx`) and webapp-spaces (`web/apps/webapp-spaces/src/rootPages/page.tsx`)
- [X] T016 Run quickstart.md smoke test checklist across all three apps — confirm all 10 items pass per `specs/015-smart-org-landing-page/quickstart.md`; fix any regressions before marking complete

---

## Dependencies & Execution Order

### Phase Dependencies

```
Phase 1 (Setup) ─── no dependencies, start immediately
       │
       ├──▶ Phase 3 (US1)   — needs T002 + T003 complete
       │         │
       │         └──▶ Phase 4 (US2)  — needs Phase 3 complete (adds state to US1 component)
       │
       └──▶ Phase 5 (US3)   — needs T001 complete (webapp only, independent of US1/US2)
                │
                └──▶ Final Phase — needs all story phases complete
```

### User Story Dependencies

| Story             | Depends on           | Can parallelize with |
| ----------------- | -------------------- | -------------------- |
| US1 (P1)          | Phase 1 (T002, T003) | US3 (once T001 done) |
| US2 (P2)          | US1 Phase 3 complete | —                    |
| US3 (P2) — webapp | Phase 1 (T001)       | US1 + US2            |

### Within-Phase Parallel Opportunities

**Phase 1** — all simultaneous:

- T001 + T002 + T003

**Phase 3** — staggered by dependency:

- T004a + T005a simultaneously (test files, different apps)
- T004 + T005 simultaneously (different apps)
- T006 (after T004) + T007 (after T005) simultaneously
- T008 + T009 simultaneously once T006/T007 done

**Phase 4** — simultaneous:

- T010 + T011

**Final Phase** — simultaneous:

- T013 + T014

### Implementation Strategy

**MVP scope** (Phase 1 + Phase 3 only — 9 tasks):  
Delivers the highest-priority case: users with orgs see the org-selection panel in webapp-teams
and webapp-spaces, left nav absent. Relay artifacts regenerated and committed. Independently
shippable.

**Full delivery** (all phases — 16 tasks):  
Adds the no-org create prompt (US2), webapp left nav removal (US3), structured logging, and
smoke test validation.
