# Developer Quickstart: Remove User-Level Billing & Payment UI

**Feature**: `014-remove-user-billing-ui`  
**Apps in scope**: `web/apps/webapp/` and `web/apps/webapp-teams/`  
**Prerequisite**: Branch `014-remove-user-billing-ui` checked out

---

## What This Change Does

Removes all user-level "My Billing & Payment" UI entry points from `webapp` and `webapp-teams`. Backend APIs and data are unchanged. Organisation-level billing UI (org admin section) is preserved.

---

## Baseline (run before any changes)

```bash
# webapp
cd web/apps/webapp
pnpm relay      # note the compiled document counts
pnpm tsc --noEmit && echo "tsc OK"
pnpm test --run

# webapp-teams
cd web/apps/webapp-teams
pnpm relay
pnpm tsc --noEmit && echo "tsc OK"
pnpm test --run
```

---

## Change Summary Per App

### webapp (`web/apps/webapp/src/`)

**Delete (directories and all contents)**:

- `app/billing-and-payment/`
- `app/msteams/billing-and-payment/`
- `rootPages/billing-and-payment/`
- `components/myBillingAndPayment/`

**Delete (7 orphaned generated files in `queries/__generated__/`)**:

- `addMyPaymentMethodDialog_addCustomerPaymentMethodIntentMutation.graphql.ts`
- `myBillingAndPayment_addMyBillingDetailsMutation.graphql.ts`
- `myBillingAndPayment_customerPaymentMethodsDetails_query.graphql.ts`
- `myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment.graphql.ts`
- `myBillingAndPayment_removeCustomerPaymentMethodMutation.graphql.ts`
- `myBillingAndPayment_rootQuery.graphql.ts`
- `myBillingAndPayment_updateMyBillingDetailsMutation.graphql.ts`

**Modify (remove billing nav + dropdown entries)**:

- `components/navigationMenu/no-organization-left-side-navigation-menu-content.tsx` — remove Billing & Payment `ListItem` block and its imports/variable
- `components/appBar/app-bar.tsx` — remove Billing & Payment `MenuItem` and `getBillingAndPaymentLink` import/variable/`BillingAndPaymentIcon`
- `components/appBar/no-organization-app-bar.tsx` — same as above
- `components/appBar/organization-store-front-app-bar.tsx` — same as above
- `components/links/index.ts` — remove `getBillingAndPaymentLink` export

### webapp-teams (`web/apps/webapp-teams/src/`)

**Delete (directories and all contents)**:

- `app/billing-and-payment/`
- `app/msteams/billing-and-payment/`
- `rootPages/billing-and-payment/`
- `components/myBillingAndPayment/`

**Delete (7 orphaned generated files in `queries/__generated__/`)**:

- Same 7 files as webapp (see above)

**Modify (remove billing nav + dropdown entries)**:

- `components/navigationMenu/no-organization-left-side-navigation-menu-content.tsx` — same as webapp
- `components/appBar/app-bar.tsx` — same as webapp (no storefront variant in teams)
- `components/appBar/no-organization-app-bar.tsx` — same as webapp
- `components/links/index.ts` — remove `getBillingAndPaymentLink` export

---

## After Each App's Changes

```bash
# Run relay in that app's directory
pnpm relay
# Expect: compiled document count decreases by ~7 (one per deleted query)

# TypeScript check
pnpm tsc --noEmit && echo "tsc OK"
# Expect: exit 0, no errors

# Tests
pnpm test --run
# Expect: all tests pass
```

---

## Smoke Test Checklist

For both `webapp` and `webapp-teams` (running locally):

- [ ] `/billing-and-payment` → 404 / not-found page
- [ ] `/msteams/billing-and-payment` → 404 / not-found page
- [ ] Left-side nav (no-org shell) — no "Billing & Payment" entry visible
- [ ] Profile dropdown (standard app bar) — no "Billing & Payment" menu item
- [ ] Profile dropdown (no-org app bar) — no "Billing & Payment" menu item
- [ ] `webapp` only: profile dropdown (storefront app bar) — no "Billing & Payment" menu item
- [ ] Organisation admin billing section — still accessible and functional
- [ ] No console errors or missing import warnings

---

## Important: Do NOT Delete

These org-level billing generated files in `queries/__generated__/` must be preserved:

**webapp**:

- `organizationAdminBillingPaymentSection_*`
- `organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation.graphql.ts`
- `multipleChoicesProductPricingBillingModes_query.graphql.ts`
- `singleChoiceOrganizationBillingCycle_query.graphql.ts`
- `singleChoiceOrganizationXeroBillingMode_query.graphql.ts`
- `singleChoiceProductPricingBillingMode_query.graphql.ts`

**webapp-teams**:

- `organizationAdminBillingPaymentSection_*`
- `singleChoiceOrganizationBillingCycle_query.graphql.ts`
- `singleChoiceOrganizationXeroBillingMode_query.graphql.ts`
