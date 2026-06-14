# Frontend Pricing Inventory

## Public Website

Current static pricing source:

- `src/web/apps/public-web/src/data/pricing.ts`

Current hardcoded data includes:

- pricing page title and description
- product offerings: Teams, Spaces, Hosts
- product ordering
- audience copy
- pricing basis copy
- tier names
- tier prices
- tier summaries
- CTA identifiers

Tests currently asserting pricing behavior:

- `src/web/apps/public-web/tests/public-site-content.test.ts`

Observed risk:

- Public-web currently owns commercial values that the feature requires to come from backend-owned pricing catalog data.

Planned direction:

- Keep public-web rendering components product-focused.
- Move commercial source of truth to backend-owned pricing catalog data.
- Replace `pricing.ts` hardcoded commercial values with a catalog adapter or generated/static catalog artifact sourced from backend contracts.
- Keep user-facing copy in American English.

## Authenticated Web App

Potential generated Relay impact:

- New GraphQL pricing catalog and Teams subscription fields may produce Relay artifacts under `src/web/apps/webapp/src/queries/__generated__/`.
- Relay artifacts must be regenerated, not hand-edited.

Potential OpenAPI impact:

- If REST endpoints are added for catalog consumption, generated web clients must be regenerated with `src/web/apps/webapp/scripts/generate.sh`.

## Non-Goals

- Do not hardcode plan names, prices, capacity options, plan order, feature lists, recommendations, or Contact Us thresholds in frontend code.
- Do not duplicate shared formatting/runtime helpers inside product apps when `@skedular/shared` or `@skedular/ui` owns the pattern.
