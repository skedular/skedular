# Split Web Products Verification

## Foundation Commands

Run from the repository root:

```bash
cd web
pnpm webapp-spaces#lint
pnpm webapp-spaces#test
pnpm webapp-spaces#build
```

## Manual Inspection

Run from `web/apps/webapp-spaces`:

```bash
pnpm dev
```

Inspect `http://localhost:15004`.

Current review routes:

- The full WebApp route surface is currently available in Spaces because `web/apps/webapp-spaces/src` is an exact mirror of `web/apps/webapp/src`.
- Start with `http://localhost:15004`, then compare any route against the matching WebApp route on `http://localhost:15000`.

## Slice Notes

- WebApp Spaces owns marketplace/co-working operator journeys.
- The current baseline intentionally copies all WebApp source before pruning.
- Spaces will remove or block private/team-only and customer-facing routes in later reviewed slices.
- Relay artefacts were copied from WebApp as part of the full source mirror. Relay generation was not required because no GraphQL operation text changed.
