# Split Web Products Verification

## Foundation Commands

Run from the repository root:

```bash
cd web
pnpm webapp-teams#lint
pnpm webapp-teams#test
pnpm webapp-teams#build
```

## Manual Inspection

Run from `web/apps/webapp-teams`:

```bash
pnpm dev
```

Inspect `http://localhost:15002`.

Current review routes:

- The Teams source started as a full mirror of `web/apps/webapp/src`.
- The Teams app route surface has now been pruned so marketplace discovery, marketplace organisation creation, marketplace setup, products, subscriptions, Stripe Connect, bank accounts, customer-facing subdomain handling, and marketplace location creation are not available as Teams routes.
- Start with `http://localhost:15002`, then compare private organisation routes against the matching WebApp route on `http://localhost:15000`.

## Slice Notes

- WebApp Teams owns private organisation and team journeys.
- The current baseline intentionally copies all WebApp source before pruning.
- Teams now removes marketplace/co-working operator and customer-facing route entry points from the Teams app surface while keeping the original WebApp routes untouched for review.
- Teams organisation selection filters to private organisations.
- Teams booking list/detail routes are limited to private booking flows.
- Unused Teams copies of marketplace/product/subscription/storefront/refund/bank-account/Stripe Connect components and tests have been removed.
- Product-tag selectors are still present because active Teams resource-management screens still use product tag fields. Treat that as a separate resource model cleanup if product tags should also disappear from private organisation resources.
- Relay artefacts were regenerated after Teams-specific GraphQL operation changes.
