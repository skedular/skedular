# Quickstart: MembershipTerm Terminology Rename

## Prerequisites

- Check out branch `048-membership-term-rename`.
- Restore dependencies and use the normal backend/frontend test infrastructure.
- Confirm the working tree before and after contract generation.

## Source-first regeneration

Update authoritative definitions first, then run:

```bash
make generate
pnpm --dir src/web relay
```

Do not hand-edit generated GraphQL schemas, event classes, OpenAPI clients, or Relay artifacts.

## Validation scenarios

1. Create/read reservation pricing and verify the domain/API expose `MembershipTerm` with unchanged behavior.
2. Read an existing stored product/offering and write a new one. Verify the the renamed storage field is used and values round-trip without loss.
3. Exercise renewal, non-renewal, billing slices, entitlements, cancellation, refund, booking, and invoice workflows; verify outcomes and independent periods remain unchanged.
4. Verify all web apps display “membership term” and no active old label.
5. Verify event, GraphQL, OpenAPI, generated-client, and Relay artifacts use the renamed field and build.
6. Search active repository surfaces for former `PurchaseCadence` identifiers; allow only clearly labeled historical migration evidence.

## Recommended checks

```bash
git diff --check
dotnet build
dotnet test --list-tests
pnpm --dir src/web --filter webapp test
```

Record build success separately from zero-test, hung, or unavailable-infrastructure results.

## Scope boundary

This guide includes the database column/property rename and migrated historical-record verification. The migration is forward-only and preserves values.

## Verification record

- `dotnet build src/Skedular.UnitTests.slnx --no-restore`: passed with 0 warnings and 0 errors.
- `dotnet test src/Skedular.UnitTests.slnx --no-build --no-restore`: 1,578 passed, 0 failed, 0 skipped.
- `pnpm --dir src/web relay`: passed; Relay artifacts regenerated.
- Event contract generation: completed; generated event outputs match the updated protobuf sources.
- GraphQL schema generation and verification: completed; generated per-API, composed, and integration-test schema outputs expose `membershipTerm`.
- Web tests: customer webapp 159, Host 169, Spaces 209, Teams 102, public web 69; all passed.
- Web builds: customer webapp, Host, Spaces, Teams, and public web passed. Public web reported existing route-conflict warnings but no build failure.
- Public web `astro check`: passed with 0 errors, warnings, or hints.
- Working-tree and cached `git diff --check`: passed after refreshing the staged feature metadata.
- Repository search found no stale active former identifiers outside the intentional persistence-facing `ProductPricing.MembershipTerm` model and historical task/spec references.
- The persistence model is `ProductPricing.MembershipTerm`; persisted property and database-column names are renamed by the forward migration with no compatibility alias.
- Event, GraphQL, backend OpenAPI, and web OpenAPI generation completed successfully; generated outputs were verified against their authoritative definitions.
