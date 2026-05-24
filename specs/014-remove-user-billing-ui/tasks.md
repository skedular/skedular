---
description: "Task list for 014-remove-user-billing-ui"
---

# Tasks: Remove User-Level Billing & Payment UI

**Input**: Design documents from `specs/014-remove-user-billing-ui/`  
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, quickstart.md ✅  
**Apps**: `web/apps/webapp/` and `web/apps/webapp-teams/` (all `src/` paths are relative to each app root)

**Tests**: No test generation tasks — this is a pure deletion feature. No new behaviour is introduced. The existing `pnpm test --run` suite is verified in the Polish phase.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing. US1 (webapp) and US2 (webapp-teams) are fully independent and may be implemented in parallel.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks in the same phase)
- **[Story]**: Which user story this task belongs to (US1, US2)
- All file paths are relative to `web/apps/<app>/`

> **Do NOT delete** these org-level generated files (they must be preserved):
> `organizationAdminBillingPaymentSection_*`, `organizationMarketplaceSetup_*`,
> `multipleChoicesProductPricingBillingModes_*`, `singleChoiceOrganization*`, `singleChoiceProductPricingBillingMode_*`

---

## Phase 1: Setup (Baseline Verification)

**Purpose**: Confirm the baseline build and toolchain work before any deletions begin.

- [X] T001 Verify `pnpm relay`, `pnpm tsc --noEmit`, and `pnpm test --run` succeed in both `web/apps/webapp/` and `web/apps/webapp-teams/` and record baseline Relay artifact counts

---

## Phase 2: Foundational (Blocking Prerequisites)

_No foundational tasks required. This is a pure removal feature with no new shared infrastructure, no new models, and no cross-story dependencies. Both user stories are fully independent._

**Checkpoint**: Foundation confirmed — user story work can begin immediately after T001.

---

## Phase 3: User Story 1 — Remove User Billing UI from `webapp` (Priority: P1) 🎯 MVP

**Goal**: Fully remove the user-level "My Billing & Payment" feature from `webapp`: routes, component directory, orphaned Relay artifacts, navigation entry, and all three AppBar profile dropdown entries.

**Independent Test**: Navigate to `webapp` as a regular member — no "Billing & Payment" link in the left nav, no "Billing & Payment" item in any profile dropdown (standard, no-org, or storefront), and `/billing-and-payment` returns 404.

### Implementation for User Story 1

- [X] T002 [US1] Delete `src/rootPages/billing-and-payment/` directory (including `page.tsx`) from `web/apps/webapp/`
- [X] T003 [P] [US1] Delete `src/app/billing-and-payment/` and `src/app/msteams/billing-and-payment/` directories from `web/apps/webapp/`
- [X] T004 [P] [US1] Delete entire `src/components/myBillingAndPayment/` directory (7 files: `add-my-payment-method-dialog.tsx`, `index.ts`, `my-billing-and-payment-autosave.test.ts`, `my-billing-and-payment-section-nav.tsx`, `my-billing-and-payment.test.tsx`, `my-billing-and-payment.tsx`, `my-payment-method-setup-form.tsx`) from `web/apps/webapp/`
- [X] T005 [P] [US1] Delete 7 orphaned Relay artifacts from `web/apps/webapp/src/queries/__generated__/`: `addMyPaymentMethodDialog_addCustomerPaymentMethodIntentMutation.graphql.ts`, `myBillingAndPayment_addMyBillingDetailsMutation.graphql.ts`, `myBillingAndPayment_customerPaymentMethodsDetails_query.graphql.ts`, `myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment.graphql.ts`, `myBillingAndPayment_removeCustomerPaymentMethodMutation.graphql.ts`, `myBillingAndPayment_rootQuery.graphql.ts`, `myBillingAndPayment_updateMyBillingDetailsMutation.graphql.ts`
- [X] T006 [P] [US1] Remove the "Billing & Payment" `ListItem` block from `web/apps/webapp/src/components/navigationMenu/no-organization-left-side-navigation-menu-content.tsx`; remove `getBillingAndPaymentLink` from its imports and remove the `billingAndPaymentLink` variable
- [X] T007 [P] [US1] In `web/apps/webapp/src/components/appBar/app-bar.tsx`: remove the Billing & Payment `MenuItem` block from the profile dropdown; remove `getBillingAndPaymentLink` import, `billingAndPaymentLink` variable, and `BillingAndPaymentIcon` import
- [X] T008 [P] [US1] In `web/apps/webapp/src/components/appBar/no-organization-app-bar.tsx`: apply the same removals as T007
- [X] T009 [P] [US1] In `web/apps/webapp/src/components/appBar/organization-store-front-app-bar.tsx`: remove the Billing & Payment `MenuItem` block; remove `getBillingAndPaymentLink` import, `billingAndPaymentLink` variable, and `BillingAndPaymentIcon` import
- [X] T010 [US1] Run `pnpm relay` inside `web/apps/webapp/` to reconcile compiler state and confirm artifact count decreases by 7 compared to T001 baseline _(T005 pre-deletes these files as a defense-in-depth precaution matching the feature 013 pattern; this relay run is still required to validate compiler state and confirm the count)_

**Checkpoint**: User Story 1 fully complete. No "Billing & Payment" visible anywhere in `webapp`; `/billing-and-payment` returns 404.

---

## Phase 4: User Story 2 — Remove User Billing UI from `webapp-teams` (Priority: P2)

**Goal**: Fully remove the user-level "My Billing & Payment" feature from `webapp-teams`: routes, component directory, orphaned Relay artifacts, navigation entry, and both AppBar profile dropdown entries (no storefront app bar variant in this app).

**Independent Test**: Navigate to `webapp-teams` as a regular member — no "Billing & Payment" link in the left nav, no "Billing & Payment" item in any profile dropdown, and `/billing-and-payment` returns 404.

### Implementation for User Story 2

- [X] T011 [US2] Delete `src/rootPages/billing-and-payment/` directory (including `page.tsx`) from `web/apps/webapp-teams/`
- [X] T012 [P] [US2] Delete `src/app/billing-and-payment/` and `src/app/msteams/billing-and-payment/` directories from `web/apps/webapp-teams/`
- [X] T013 [P] [US2] Delete entire `src/components/myBillingAndPayment/` directory (6 files: `add-my-payment-method-dialog.tsx`, `index.ts`, `my-billing-and-payment-autosave.test.ts`, `my-billing-and-payment-section-nav.tsx`, `my-billing-and-payment.tsx`, `my-payment-method-setup-form.tsx`) from `web/apps/webapp-teams/`
- [X] T014 [P] [US2] Delete 7 orphaned Relay artifacts from `web/apps/webapp-teams/src/queries/__generated__/`: `addMyPaymentMethodDialog_addCustomerPaymentMethodIntentMutation.graphql.ts`, `myBillingAndPayment_addMyBillingDetailsMutation.graphql.ts`, `myBillingAndPayment_customerPaymentMethodsDetails_query.graphql.ts`, `myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment.graphql.ts`, `myBillingAndPayment_removeCustomerPaymentMethodMutation.graphql.ts`, `myBillingAndPayment_rootQuery.graphql.ts`, `myBillingAndPayment_updateMyBillingDetailsMutation.graphql.ts`
- [X] T015 [P] [US2] Remove the "Billing & Payment" `ListItem` block from `web/apps/webapp-teams/src/components/navigationMenu/no-organization-left-side-navigation-menu-content.tsx`; remove `getBillingAndPaymentLink` from its imports and remove the `billingAndPaymentLink` variable
- [X] T016 [P] [US2] In `web/apps/webapp-teams/src/components/appBar/app-bar.tsx`: remove the Billing & Payment `MenuItem` block; remove `getBillingAndPaymentLink` import, `billingAndPaymentLink` variable, and `BillingAndPaymentIcon` import
- [X] T017 [P] [US2] In `web/apps/webapp-teams/src/components/appBar/no-organization-app-bar.tsx`: apply the same removals as T016
- [X] T018 [US2] Run `pnpm relay` inside `web/apps/webapp-teams/` to reconcile compiler state and confirm artifact count decreases by 7 compared to T001 baseline _(T014 pre-deletes these files as a defense-in-depth precaution matching the feature 013 pattern; this relay run is still required to validate compiler state and confirm the count)_

**Checkpoint**: User Story 2 fully complete. No "Billing & Payment" visible anywhere in `webapp-teams`; `/billing-and-payment` returns 404.

---

## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Dead-code cleanup, TypeScript validation, full test run, and smoke test.

- [X] T019 [P] Remove the `getBillingAndPaymentLink` export from `web/apps/webapp/src/components/links/index.ts` (all callers removed by T006–T009)
- [X] T020 [P] Remove the `getBillingAndPaymentLink` export from `web/apps/webapp-teams/src/components/links/index.ts` (all callers removed by T015–T017)
- [X] T021 [P] Run `pnpm tsc --noEmit` in `web/apps/webapp/` — must exit 0 with zero TypeScript errors
- [X] T022 [P] Run `pnpm tsc --noEmit` in `web/apps/webapp-teams/` — must exit 0 with zero TypeScript errors
- [X] T023 [P] Run `pnpm test --run` in `web/apps/webapp/` — all existing tests must pass
- [X] T024 [P] Run `pnpm test --run` in `web/apps/webapp-teams/` — all existing tests must pass
- [X] T025 Smoke test both apps per `quickstart.md` checklist: confirm `/billing-and-payment` → 404, `/msteams/billing-and-payment` → 404, left nav clean, all profile dropdowns clean, organisation admin billing section still accessible

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately
- **Foundational (Phase 2)**: N/A — no foundational tasks
- **User Stories (Phase 3–4)**: Both depend only on Phase 1 (T001 baseline). US1 and US2 are **fully independent** — they touch separate app directories and may be implemented in parallel by two developers
- **Polish (Phase 5)**: T019–T020 depend on Phase 3 and Phase 4 being complete; T021–T024 depend on T019–T020; T025 depends on T021–T024

### User Story Dependencies

```
T001 (baseline)
  ├── T002, T003, T004, T005, T006, T007, T008, T009 [all US1, all parallel — independent directories/files]
  │     └── T010 (relay webapp) → Phase 5
  └── T011 → T012, T013, T014, T015, T016, T017 [all US2, parallelizable within the group]
        └── T018 (relay webapp-teams) → Phase 5

Phase 5:
  T010 + T018 → T019, T020 [parallel]
              → T021, T022 [parallel, after T019+T020]
              → T023, T024 [parallel, after T021+T022]
              → T025
```

### Parallel Execution Examples

**Single developer (sequential)**:

```
T001 → T002,T003,T004,T005,T006,T007,T008,T009 (all parallel) → T010
     → T011 → T012,T013,T014 (parallel) → T015,T016,T017 (parallel) → T018
     → T019,T020 (parallel) → T021,T022 (parallel) → T023,T024 (parallel) → T025
```

**Two developers (parallel stories)**:

```
Dev 1: T001 → T002,T003,T004,T005,T006,T007,T008,T009 (parallel) → T010 → T019 → T021 → T023
Dev 2:         ↕    → T011 → T012,T013,T014 → T015,T016,T017 → T018 → T020 → T022 → T024
Both: T025
```

---

## Implementation Strategy

**MVP**: Complete Phase 3 (US1 — webapp) first. Delivers the highest-impact removal immediately.

**Full delivery**: Phase 4 (US2 — webapp-teams) extends the same pattern to the second app. No architectural decisions required — identical file shapes, two fewer app bar variants.

**Relay note**: Running `pnpm relay` removes orphaned `myBillingAndPayment_*` artifacts automatically; it does NOT affect org-level billing artifacts (`organizationAdminBillingPaymentSection_*`, etc.) which remain untouched.
