---
description: "Task list for 013-spaces-remove-member-features"
---

# Tasks: Remove Member-Facing Features from Spaces App

**Input**: Design documents from `specs/013-spaces-remove-member-features/`  
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, quickstart.md ✅  
**App**: `web/apps/webapp-spaces/` (all `src/` paths are relative to this root)

**Tests**: No test generation tasks — this is a pure deletion feature. No new behaviour is introduced. The existing `pnpm test` suite is verified in the Polish phase.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks in the same phase)
- **[Story]**: Which user story this task belongs to (US1, US2, US3)
- All file paths are relative to `web/apps/webapp-spaces/`

---

## Phase 1: Setup (Baseline Verification)

**Purpose**: Confirm the baseline build and toolchain work before any deletions begin.

- [x] T001 Verify `pnpm build` and `pnpm relay` run successfully in `web/apps/webapp-spaces/` before any changes (establishes clean baseline)

---

## Phase 2: Foundational (Blocking Prerequisites)

_No foundational tasks required. This is a pure removal feature with no new shared infrastructure, no new models, no new API surfaces, and no new cross-story dependencies._

**Checkpoint**: Foundation confirmed — user story work can begin immediately after T001.

---

## Phase 3: User Story 1 — Remove Billing & Payment (Priority: P1) 🎯 MVP

**Goal**: Fully remove the My Billing & Payment member feature: route, component, generated artifacts, navigation entry, and AppBar links.

**Independent Test**: Navigate to webapp-spaces — no "Billing & Payment" entry in the left nav, no "Billing & Payment" item in the profile dropdown, and `/billing-and-payment` returns 404.

### Implementation for User Story 1

- [x] T002 [US1] Delete route page `src/rootPages/billing-and-payment/page.tsx` (and the `billing-and-payment/` directory)
- [x] T003 [P] [US1] Delete entire `src/components/myBillingAndPayment/` directory (7 files: `add-my-payment-method-dialog.tsx`, `index.ts`, `my-billing-and-payment-autosave.test.ts`, `my-billing-and-payment-section-nav.tsx`, `my-billing-and-payment.test.tsx`, `my-billing-and-payment.tsx`, `my-payment-method-setup-form.tsx`)
- [x] T004 [P] [US1] Delete 6 orphaned Relay generated files from `src/queries/__generated__/`: `myBillingAndPayment_addMyBillingDetailsMutation.graphql.ts`, `myBillingAndPayment_customerPaymentMethodsDetails_query.graphql.ts`, `myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment.graphql.ts`, `myBillingAndPayment_removeCustomerPaymentMethodMutation.graphql.ts`, `myBillingAndPayment_rootQuery.graphql.ts`, `myBillingAndPayment_updateMyBillingDetailsMutation.graphql.ts`
- [x] T005 [US1] Remove the "Billing & Payment" `ListItem` block from `src/components/navigationMenu/no-organization-left-side-navigation-menu-content.tsx`; also remove `getBillingAndPaymentLink` and `BillingAndPaymentIcon` from its imports and the `billingAndPaymentLink` variable
- [x] T006 [US1] In `src/components/appBar/app-bar.tsx`: remove the `{selectedOrganizationId && ...}` Billing & Payment `MenuItem` block from the profile dropdown; remove `getBillingAndPaymentLink` import, `billingAndPaymentLink` variable declaration, and `BillingAndPaymentIcon` import
- [x] T007 [US1] In `src/components/appBar/no-organization-app-bar.tsx`: remove the Billing & Payment `MenuItem` block from the profile dropdown; remove `getBillingAndPaymentLink` import, `billingAndPaymentLink` variable declaration, and `BillingAndPaymentIcon` import

**Checkpoint**: User Story 1 fully complete. No "Billing & Payment" visible anywhere; `/billing-and-payment` returns 404.

---

## Phase 4: User Story 2 — Remove My Settings (Priority: P2)

**Goal**: Fully remove the My Settings personal profile feature: route, component, generated artifacts, navigation entry, and AppBar link.

**Independent Test**: Navigate to webapp-spaces — no "Settings" personal profile entry in the left nav, no "Settings" item in the profile dropdown, and `/settings` returns 404.

### Implementation for User Story 2

- [x] T008 [US2] Delete route page `src/rootPages/settings/page.tsx` (and the `settings/` directory)
- [x] T009 [P] [US2] Delete entire `src/components/mySettings/` directory (4 files: `index.ts`, `my-settings-autosave.test.ts`, `my-settings.test.tsx`, `my-settings.tsx`)
- [x] T010 [P] [US2] Delete 2 orphaned Relay generated files from `src/queries/__generated__/`: `mySettings_rootQuery.graphql.ts`, `mySettings_updateCustomerDetailsMutation.graphql.ts`
- [x] T011 [US2] Remove the "Settings" `ListItem` block from `src/components/navigationMenu/no-organization-left-side-navigation-menu-content.tsx`; also remove `getSettingsLink` from its imports and the `settingsBaseLink` variable
- [x] T012 [US2] In `src/components/appBar/app-bar.tsx`: remove the `{selectedOrganizationId && ...}` Settings `MenuItem` block from the profile dropdown; remove `getSettingsLink` import, `settingsLink` variable declaration, and `SettingsIcon` import
- [x] T013 [US2] In `src/components/appBar/no-organization-app-bar.tsx`: remove the Settings `MenuItem` block from the profile dropdown; remove `getSettingsLink` import, `settingsLink` variable declaration, and `SettingsIcon` import

**Checkpoint**: User Stories 1 AND 2 both complete. No "Settings" profile link visible anywhere; `/settings` returns 404.

---

## Phase 5: User Story 3 — Remove Member Notifications (Priority: P3)

**Goal**: Fully remove the member invitation Notifications feature: route, component, generated artifacts, navigation entry, AppBar notification bell and badge, and the `pendingOrganizationInvitationsCount`/`pendingTeamInvitationsCount` GraphQL fragment fields. Regenerate affected Relay artifacts.

**Independent Test**: Navigate to webapp-spaces — no notification bell icon or pending invitation badge in the top bar, no "Notifications" nav entry, and `/notifications` returns 404. Having pending org/team invitations produces no visible badge or link.

> **FR-013 (mobile nav audit)**: Completed in research.md §2. Both `no-organization-mobile-left-side-navigation-menu.tsx` and `mobile-left-side-navigation-menu.tsx` are pure Drawer wrappers with no independent links to removed routes — no direct edits needed; changes to content components cascade automatically.

### Implementation for User Story 3

- [x] T014 [US3] Delete route page `src/rootPages/notifications/page.tsx` (and the `notifications/` directory)
- [x] T015 [P] [US3] Delete the `src/components/notification/notifications/` subdirectory (2 files: `index.ts`, `notifications.tsx`); do NOT delete the parent `notification/` directory — shared toast helpers there must remain
- [x] T016 [P] [US3] Delete 5 orphaned Relay generated files from `src/queries/__generated__/`: `notifications_acceptInvitationToJoinOrganizationMutation.graphql.ts`, `notifications_acceptInvitationToJoinTeamMutation.graphql.ts`, `notifications_rejectInvitationToJoinOrganizationMutation.graphql.ts`, `notifications_rejectInvitationToJoinTeamMutation.graphql.ts`, `notifications_rootQuery.graphql.ts`
- [x] T017 [US3] Remove the "Notifications" `ListItem` block from `src/components/navigationMenu/no-organization-left-side-navigation-menu-content.tsx`; also remove `getNotificationsLink` and `NotificationsIcon` from its imports and the `notificationsLink` variable
- [x] T018 [US3] In `src/components/appBar/app-bar.tsx`: (a) remove the desktop notification bell `IconButton` block entirely; (b) remove the mobile-only Notifications `MenuItem` block inside `Box sx={{ display: { xs: 'block', md: 'none' } }}`; (c) remove `getNotificationsLink` import, `notificationsLink` and `pendingInvitationsCount` variable declarations, `NotificationsIcon` import, and `Badge` import if unused; (d) remove `pendingOrganizationInvitationsCount` and `pendingTeamInvitationsCount` fields from the Relay GraphQL fragment
- [x] T019 [US3] In `src/components/appBar/no-organization-app-bar.tsx`: apply the same comprehensive cleanup as T018 — remove notification bell IconButton, mobile Notifications MenuItem, imports, variables, and the two fragment fields
- [x] T020 [US3] Run `pnpm relay` inside `web/apps/webapp-spaces/` to regenerate `src/queries/__generated__/appBar_query.graphql.ts` and `src/queries/__generated__/noOrganizationAppBar_query.graphql.ts` — these are the only two generated files that must be regenerated (not hand-edited) after the fragment field removals in T018 and T019

**Checkpoint**: User Story 3 fully complete. No notification bell, no badge, no Notifications nav entry; `/notifications` returns 404; Relay artifacts up-to-date.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final dead-code cleanup, build verification, and smoke test.

- [x] T021 Remove the three now-unused link exports from `src/components/links/index.ts`: `getBillingAndPaymentLink`, `getNotificationsLink`, and `getSettingsLink` (all callers removed by T005–T007, T011–T013, T017–T019)
- [x] T022 [P] Run `pnpm build` inside `web/apps/webapp-spaces/` — must complete with zero TypeScript errors and zero unused-import warnings related to removed code
- [x] T023 [P] Run `pnpm test` inside `web/apps/webapp-spaces/` — all remaining tests must pass; no failures caused or exposed by the removals
- [x] T024 Smoke-test the running app: confirm (a) `/billing-and-payment` returns 404, (b) `/settings` returns 404, (c) `/notifications` returns 404, (d) left nav shows only "Home" in the no-organisation shell, (e) profile dropdown shows no Settings/Billing/Notifications links, (f) no notification bell in the top bar

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately
- **Foundational (Phase 2)**: N/A — no foundational tasks
- **User Stories (Phase 3–5)**: All depend on Phase 1 (T001 baseline verification). Sequential ordering is recommended because T005/T011/T017 all edit `no-organization-left-side-navigation-menu-content.tsx`, T006/T012/T018 all edit `app-bar.tsx`, and T007/T013/T019 all edit `no-organization-app-bar.tsx` — batching edits per file reduces merge conflicts
- **Polish (Phase 6)**: Depends on all three user story phases being complete; T022 and T023 depend on T021

### User Story Dependencies

- **User Story 1 (P1)**: Starts after T001. No dependency on US2 or US3.
- **User Story 2 (P2)**: Starts after T001. No functional dependency on US1. Sequential ordering recommended (shared source files).
- **User Story 3 (P3)**: Starts after T001. T020 (`pnpm relay`) must run after BOTH T018 and T019 are complete.

### Within Each User Story

- Route deletion, component directory deletion, and generated file deletion are parallel (different files)
- Nav/AppBar edits are sequential per file (one file at a time)
- US3 only: AppBar edits (T018, T019) must complete before `pnpm relay` (T020)

### Parallel Opportunities

- T003 and T004 can run in parallel (US1 component dir + generated files)
- T009 and T010 can run in parallel (US2 component dir + generated files)
- T015 and T016 can run in parallel (US3 component subdir + generated files)
- T022 and T023 (build + test) can run in parallel after T021

---

## Parallel Example: User Story 1

```bash
# All deletion tasks for US1 can run simultaneously:
Task T002: Delete rootPages/billing-and-payment/page.tsx
Task T003: Delete components/myBillingAndPayment/ (7 files)
Task T004: Delete queries/__generated__/myBillingAndPayment_*.graphql.ts (6 files)

# Then sequential nav/AppBar edits (each task touches a different file):
Task T005: Edit no-organization-left-side-navigation-menu-content.tsx
Task T006: Edit app-bar.tsx
Task T007: Edit no-organization-app-bar.tsx
```

## Parallel Example: User Story 3

```bash
# All deletion tasks for US3 can run simultaneously:
Task T014: Delete rootPages/notifications/page.tsx
Task T015: Delete components/notification/notifications/ (2 files — NOT parent)
Task T016: Delete queries/__generated__/notifications_*.graphql.ts (5 files)

# Then sequential nav/AppBar edits — T018 and T019 must BOTH finish before T020:
Task T017: Edit no-organization-left-side-navigation-menu-content.tsx
Task T018: Edit app-bar.tsx (remove bell, mobile MenuItem, fragment fields)
Task T019: Edit no-organization-app-bar.tsx (same cleanup)
Task T020: pnpm relay (ONLY after T018 + T019 are both done)
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: T001 baseline
2. Complete Phase 3: US1 (T002–T007)
3. **STOP and VALIDATE**: `/billing-and-payment` returns 404; no Billing & Payment in nav or dropdown
4. Deploy/demo if ready

### Incremental Delivery

1. T001 (baseline) → Foundation confirmed
2. T002–T007 (US1) → Test independently: Billing & Payment gone
3. T008–T013 (US2) → Test independently: Settings gone
4. T014–T020 (US3 + relay) → Test independently: Notifications bell and page gone
5. T021–T024 (Polish) → Full build + test + smoke check

**Critical notes**:

- `components/notification/` parent directory (shared toast helpers) must NOT be deleted — only the `notifications/` subdirectory inside it (T015)
- `pnpm relay` in T020 must run AFTER both T018 AND T019 are fully complete
- Do not hand-edit `appBar_query.graphql.ts` or `noOrganizationAppBar_query.graphql.ts` — regenerate only via `pnpm relay`
- T022 and T023 can run in parallel after T021 is complete
