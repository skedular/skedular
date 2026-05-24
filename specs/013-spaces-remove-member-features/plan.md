# Implementation Plan: Remove Member-Facing Features from Spaces App

**Branch**: `013-remove-spaces-org-private` | **Date**: 2026-05-24 | **Spec**: [spec.md](spec.md)  
**Input**: Feature specification from `specs/013-spaces-remove-member-features/spec.md`

## Summary

Remove three member-facing features from `web/apps/webapp-spaces` — My Billing & Payment, My Settings (personal profile), and Member Notifications (invitation bell + page). The spaces app is the administrative interface for co-working space owners and managers; these features are end-member concerns that belong in the main webapp. The implementation is a pure deletion and cleanup: delete routes, component directories, orphaned Relay-generated files, nav entries, AppBar UI blocks, and finally run `pnpm relay` to regenerate the two AppBar artefacts whose fragments are modified, then verify with `pnpm build` and `pnpm test`.

## Technical Context

**Language/Version**: TypeScript 6.0.3, React 19.2.6, Next.js 16.2.6 App Router  
**Primary Dependencies**: Relay 20.1.1, MUI v9, `@skedular/ui` (design system), `@skedular/shared` (cross-product runtime), pnpm 11.1.2, Turborepo  
**Storage**: N/A — frontend-only, no persistence layer changes  
**Testing**: Vitest + React Testing Library (existing suite); `pnpm test` inside `web/apps/webapp-spaces/`  
**Target Platform**: Web browser (Next.js SSR/CSR)  
**Project Type**: web-application (webapp-spaces product)  
**Performance Goals**: N/A — no new rendering or network paths introduced  
**Constraints**: Generated Relay artefacts MUST NOT be hand-edited; `pnpm relay` must run after any GraphQL fragment modifications; mobile nav variants require no direct edits (pure Drawer wrappers that inherit changes from content components)  
**Scale/Scope**: 3 features removed; 4 source files modified; 13 Relay-generated files deleted; 2 Relay-generated files regenerated

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — No changes to `api-definitions/` or the backend GraphQL schema. The `pendingOrganizationInvitationsCount` and `pendingTeamInvitationsCount` fields are simply no longer queried by this client; they remain in the schema for other consumers. Frontend fragment modifications in `app-bar.tsx` and `no-organization-app-bar.tsx` require running `pnpm relay` — this is identified and captured in T020. ✅
- [x] **II. Domain Boundaries** — Frontend-only change. No cross-domain service or event interface is involved. ✅
- [x] **III. Testing** — No new behaviour is introduced. Test files co-located with removed components are deleted with them. The existing `pnpm test` suite must pass after removal (verified in T023). No integration tests required — no persistence or event boundaries are crossed. ✅
- [x] **IV. Frontend** — Relay fragments are collocated with their components. Generated Relay artefacts are not hand-edited: orphaned files are deleted; modified fragment artefacts are regenerated via `pnpm relay` (T020). No new typography components introduced; existing typography is unchanged. ✅
- [x] **V. Pattern Consistency** — No new patterns introduced. This is pure deletion. All removals follow the existing file conventions for routes, component directories, Relay-generated files, and nav/AppBar content. ✅
- [x] **VI. Logging** — No new behaviour is added. All logging inside removed components is deleted with them. No new structured logging is required (LOG-001 in spec). ✅

## Project Structure

### Documentation (this feature)

```text
specs/013-spaces-remove-member-features/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command — NOT created by /speckit.plan)
```

_Note: No `data-model.md` or `contracts/` are required — this is a pure deletion feature. No new entities, fields, or API surfaces are introduced._

### Source Code (affected files within `web/apps/webapp-spaces/`)

```text
src/rootPages/
├── billing-and-payment/page.tsx        [DELETE] US1 route
├── settings/page.tsx                   [DELETE] US2 route
└── notifications/page.tsx              [DELETE] US3 route

src/components/
├── myBillingAndPayment/                [DELETE entire dir — 7 files] US1
│   ├── add-my-payment-method-dialog.tsx
│   ├── index.ts
│   ├── my-billing-and-payment-autosave.test.ts
│   ├── my-billing-and-payment-section-nav.tsx
│   ├── my-billing-and-payment.test.tsx
│   ├── my-billing-and-payment.tsx
│   └── my-payment-method-setup-form.tsx
├── mySettings/                         [DELETE entire dir — 4 files] US2
│   ├── index.ts
│   ├── my-settings-autosave.test.ts
│   ├── my-settings.test.tsx
│   └── my-settings.tsx
├── notification/                       [KEEP parent dir — toast helpers]
│   └── notifications/                  [DELETE subdirectory — 2 files] US3
│       ├── index.ts
│       └── notifications.tsx
├── navigationMenu/
│   └── no-organization-left-side-navigation-menu-content.tsx  [MODIFY] US1+US2+US3
├── appBar/
│   ├── app-bar.tsx                     [MODIFY] US1+US2+US3
│   └── no-organization-app-bar.tsx     [MODIFY] US1+US2+US3
└── links/
    └── index.ts                        [MODIFY] remove 3 dead exports after all callers gone

src/queries/__generated__/
├── myBillingAndPayment_addMyBillingDetailsMutation.graphql.ts              [DELETE]
├── myBillingAndPayment_customerPaymentMethodsDetails_query.graphql.ts      [DELETE]
├── myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment.graphql.ts  [DELETE]
├── myBillingAndPayment_removeCustomerPaymentMethodMutation.graphql.ts      [DELETE]
├── myBillingAndPayment_rootQuery.graphql.ts                                [DELETE]
├── myBillingAndPayment_updateMyBillingDetailsMutation.graphql.ts           [DELETE]
├── mySettings_rootQuery.graphql.ts                                         [DELETE]
├── mySettings_updateCustomerDetailsMutation.graphql.ts                     [DELETE]
├── notifications_acceptInvitationToJoinOrganizationMutation.graphql.ts     [DELETE]
├── notifications_acceptInvitationToJoinTeamMutation.graphql.ts             [DELETE]
├── notifications_rejectInvitationToJoinOrganizationMutation.graphql.ts     [DELETE]
├── notifications_rejectInvitationToJoinTeamMutation.graphql.ts             [DELETE]
├── notifications_rootQuery.graphql.ts                                      [DELETE]
├── appBar_query.graphql.ts                                                 [REGENERATE via pnpm relay]
└── noOrganizationAppBar_query.graphql.ts                                   [REGENERATE via pnpm relay]
```

**Structure Decision**: All changes are confined to `web/apps/webapp-spaces/src/`. No changes to `web/packages/ui`, `web/packages/shared`, `api-definitions/`, or any backend domain. The mobile navigation variants (`no-organization-mobile-left-side-navigation-menu.tsx`, `mobile-left-side-navigation-menu.tsx`) require no direct edits — they are pure Drawer wrappers that inherit from the content component.

## Complexity Tracking

_No constitution violations. No new patterns. No deviations from established conventions._
