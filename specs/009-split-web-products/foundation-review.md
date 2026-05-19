# Foundation Review: Split Web Products

## Review URLs

- WebApp: `http://localhost:15000`
- WebApp Teams: `http://localhost:15002`
- WebApp Spaces: `http://localhost:15004`

## Checklist

- [x] WebApp still serves the existing customer-facing root and custom-domain behaviour.
- [x] WebApp Spaces shows the marketplace/co-working operator foundation shell.
- [x] WebApp Teams shows the private organisation foundation shell.
- [x] Spaces and Teams can render an organisation empty state without exposing the wrong organisation concepts.
- [x] Shared visual primitives come from `@skedular/ui`.
- [x] Shared runtime helpers come from `@skedular/shared`.
- [x] No backend service, API contract, or backend ownership change is required.

## Verification Results

- `CI=true pnpm install --no-frozen-lockfile` from `web`: passed; refreshed workspace install after adding `@skedular/shared` to Spaces and Teams.
- `pnpm webapp#lint`: passed.
- `pnpm webapp#test`: passed.
- `pnpm webapp#build`: passed.
- `pnpm webapp-spaces#lint`: passed.
- `pnpm webapp-spaces#test`: passed.
- `pnpm webapp-spaces#build`: passed.
- `pnpm webapp-teams#lint`: passed.
- `pnpm webapp-teams#test`: passed.
- `pnpm webapp-teams#build`: passed.
- Relay: not run; no GraphQL operations or generated Relay artefacts moved in this foundation slice.
- Browser smoke: `http://localhost:15000`, `http://localhost:15002`, and `http://localhost:15004` loaded with the expected page titles and root content.
- Foundation refinement: Spaces and Teams now use the same local font setup as WebApp and a shared Skedular app frame/header instead of a standalone placeholder style.

## Manual Review Status

Ready for manual review after foundation style refinement. Journey migration must not begin until this foundation is reviewed and accepted.
