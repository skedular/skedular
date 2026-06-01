# Quickstart: Customer Landing Cleanup

## Goal

Validate that webapp becomes the no-subdomain aggregate marketplace and customer self-service surface while existing owner-specific custom-subdomain marketplace behavior remains unchanged.

## Prerequisites

- Use branch `020-customer-landing-cleanup`.
- Install web dependencies with `pnpm install` from `src/web` if dependencies are not already installed.
- Do not edit generated Relay artifacts by hand.
- If GraphQL fields or selections change, regenerate through the established web generation path.

## Planning Validation Flow

1. Review the feature specification:

   ```bash
   sed -n '1,260p' specs/020-customer-landing-cleanup/spec.md
   ```

2. Review the responsibility inventory contract:

   ```bash
   sed -n '1,260p' specs/020-customer-landing-cleanup/contracts/capability-inventory.md
   ```

3. Review the aggregate marketplace route contract:

   ```bash
   sed -n '1,260p' specs/020-customer-landing-cleanup/contracts/aggregate-marketplace.md
   ```

4. Confirm the key source areas before implementation tasks are generated:

   ```bash
   find src/web/apps/webapp/src/app -maxdepth 4 -type f -name 'page.tsx' | sort
   find src/web/apps/webapp/src/rootPages/marketplace -type f | sort
   find src/web/apps/webapp/src/components/location/marketplaceLocations -type f | sort
   ```

## Implementation Validation Flow

Run these from `src/web` after implementation tasks are completed:

```bash
pnpm webapp#test
pnpm webapp#lint
pnpm webapp#build
```

If GraphQL schema, Relay selections, OpenAPI clients, or generated web artifacts change, run the appropriate generation command before validation:

```bash
cd ../..
make generate
```

## Manual Acceptance Checks

- No-subdomain webapp shows aggregate marketplace discovery across eligible marketplace-enabled customer-bookable locations.
- Existing owner-specific custom-subdomain marketplace pages still behave as they did before.
- Selecting an aggregate location reaches location-level marketplace product browsing and purchase behavior without URL redirects.
- Customer bookings and subscriptions are visible across organizations for the signed-in customer.
- Eligible customer actions for cancel, change, and refund appear only when policy allows.
- Private organization booking, resource management, coworking-owner subscription management, and admin workflows are absent from customer-facing webapp navigation.
- Removed or unsupported webapp paths resolve in place with customer-safe messaging and no URL redirects.
- User-facing and operator-facing copy uses American spelling.
