# Research: Remove User-Level Billing & Payment UI

**Feature**: `014-remove-user-billing-ui`  
**Phase**: 0 — Research  
**Date**: 2026-05-24

---

## Decision 1: Scope of Relay artifact changes

**Question**: Does removing the `myBillingAndPayment` components require AppBar/rootShell GraphQL fragment modifications (like feature 013 did for notifications)?

**Decision**: No. Billing is navigation-only — the AppBar fragments do not carry billing-specific GraphQL fields. Running `pnpm relay` after deleting component files is sufficient to clean up orphaned artefacts. No AppBar fragment fields need to be removed, and no rootShell query modifications are needed.

**Rationale**: In feature 013, `pendingOrganizationInvitationsCount` and `pendingTeamInvitationsCount` had to be removed from AppBar fragments because the AppBar component itself rendered invitation counts. The billing AppBar entry is a plain navigation link — it carries no data via GraphQL.

**Alternatives considered**: Hand-deleting generated files without running `pnpm relay` — rejected; the relay compiler must reconcile its manifest to avoid stale checksums.

---

## Decision 2: Full deletion of `myBillingAndPayment/` directory

**Question**: Is the `myBillingAndPayment` component directory safe to delete entirely, or do other components depend on it?

**Decision**: Safe to delete entirely in both `webapp` and `webapp-teams`. Verified by grep — the only external import in each app is from `rootPages/billing-and-payment/page.tsx`, which is itself being deleted.

**Rationale**: User confirmed this expectation. Code search confirmed zero external consumers outside the billing route page.

**Alternatives considered**: Keep reusable sub-components (dialog, form) while removing the page — rejected; no consumer exists.

---

## Decision 3: `getBillingAndPaymentLink` export removal

**Question**: Is `getBillingAndPaymentLink` in each app's `components/links/index.ts` safe to delete?

**Decision**: Yes — in both apps, all callers are in files that are either being deleted or modified to remove the billing reference. After the implementation changes, zero callers remain.

**Callers in webapp** (all removed):

- `components/appBar/app-bar.tsx` (modified — link removed)
- `components/appBar/no-organization-app-bar.tsx` (modified — link removed)
- `components/appBar/organization-store-front-app-bar.tsx` (modified — link removed)
- `components/navigationMenu/no-organization-left-side-navigation-menu-content.tsx` (modified — link removed)
- `components/myBillingAndPayment/my-billing-and-payment-section-nav.tsx` (deleted)

**Callers in webapp-teams** (all removed):

- `components/appBar/app-bar.tsx` (modified — link removed)
- `components/appBar/no-organization-app-bar.tsx` (modified — link removed)
- `components/navigationMenu/no-organization-left-side-navigation-menu-content.tsx` (modified — link removed)
- `components/myBillingAndPayment/my-billing-and-payment-section-nav.tsx` (deleted)

---

## Decision 4: Org-billing generated artefacts — do not delete

**Question**: Which `__generated__` files are org-level billing (must be kept) vs user-level billing (must be deleted)?

**Decision**: Keep all `organizationAdmin*`, `organizationMarketplaceSetup*`, `singleChoiceOrganization*`, `multipleChoicesProductPricing*`, and `singleChoiceProductPricing*` generated files — they belong to organisation billing management which is out of scope.

**Files to delete in webapp** (7):

- `addMyPaymentMethodDialog_addCustomerPaymentMethodIntentMutation.graphql.ts`
- `myBillingAndPayment_addMyBillingDetailsMutation.graphql.ts`
- `myBillingAndPayment_customerPaymentMethodsDetails_query.graphql.ts`
- `myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment.graphql.ts`
- `myBillingAndPayment_removeCustomerPaymentMethodMutation.graphql.ts`
- `myBillingAndPayment_rootQuery.graphql.ts`
- `myBillingAndPayment_updateMyBillingDetailsMutation.graphql.ts`

**Files to delete in webapp-teams** (7 — same set):

- `addMyPaymentMethodDialog_addCustomerPaymentMethodIntentMutation.graphql.ts`
- `myBillingAndPayment_addMyBillingDetailsMutation.graphql.ts`
- `myBillingAndPayment_customerPaymentMethodsDetails_query.graphql.ts`
- `myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment.graphql.ts`
- `myBillingAndPayment_removeCustomerPaymentMethodMutation.graphql.ts`
- `myBillingAndPayment_rootQuery.graphql.ts`
- `myBillingAndPayment_updateMyBillingDetailsMutation.graphql.ts`

---

## Decision 5: AppBar surface inventory per app

**webapp AppBar variants** (3 — all have billing):

- `app-bar.tsx` — standard org-context app bar
- `no-organization-app-bar.tsx` — no-org shell app bar
- `organization-store-front-app-bar.tsx` — storefront (marketplace) app bar

**webapp-teams AppBar variants** (2 — both have billing; no storefront variant):

- `app-bar.tsx` — standard org-context app bar
- `no-organization-app-bar.tsx` — no-org shell app bar
- `unauthenticated-app-bar.tsx` — no billing (user not logged in; confirmed safe to skip)

---

## NEEDS CLARIFICATION — Resolved

All unknowns were resolved during planning research. No outstanding clarifications.
