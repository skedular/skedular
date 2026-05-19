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

- The full WebApp route surface is currently available in Teams because `web/apps/webapp-teams/src` is an exact mirror of `web/apps/webapp/src`.
- Start with `http://localhost:15002`, then compare any route against the matching WebApp route on `http://localhost:15000`.

## Slice Notes

- WebApp Teams owns private organisation and team journeys.
- The current baseline intentionally copies all WebApp source before pruning.
- Teams will remove or block marketplace/co-working operator and customer-facing routes in later reviewed slices.
- Relay artefacts were copied from WebApp as part of the full source mirror. Relay generation was not required because no GraphQL operation text changed.
