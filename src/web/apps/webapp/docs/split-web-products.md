# Split Web Products Verification

## Foundation Commands

Run from the repository root:

```bash
cd web
pnpm webapp#lint
pnpm webapp#test
pnpm webapp#build
```

## Manual Inspection

Run from `web/apps/webapp`:

```bash
pnpm dev
```

Inspect `http://localhost:15000`.

Also inspect an existing co-working custom domain mapped to WebApp when storefront behaviour is under review.

## Slice Notes

- WebApp remains the customer-facing product for root discovery and subdomain customer experiences.
- WebApp now has an explicit customer-facing entry resolver for public discovery, co-working subdomains, and future private organisation subdomains.
- Custom domains still default to the existing co-working storefront until data-backed organisation-type detection is wired.
- Do not remove a WebApp route until backend-originated return URL usage is checked in `specs/009-split-web-products/route-retirement-register.md`.
- Run Relay checks when a WebApp GraphQL operation moves or changes.
- Completed slices in this phase did not move WebApp GraphQL operations, so Relay generation was not required.
