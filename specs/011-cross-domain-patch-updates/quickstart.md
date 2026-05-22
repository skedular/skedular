# Quickstart: Cross-Domain Patch Updates

## 1. Start from the reference pattern

- Read `specs/011-cross-domain-patch-updates/spec.md`.
- Read `specs/010-organization-patch-updates/plan.md` and the organisation patch contract for the completed pattern.
- Read `specs/011-cross-domain-patch-updates/contracts/cross-domain-patch-contracts.md`.

## 2. Re-scan remaining update surfaces

Before implementation closes, inventory owned editable update surfaces in:

- booking GraphQL and `api-definitions/grpc/skedular/booking`
- customer GraphQL and `api-definitions/grpc/skedular/customer`
- location GraphQL and `api-definitions/grpc/skedular/location`
- marketplace GraphQL
- team GraphQL and `api-definitions/grpc/skedular/team`
- cross-domain consumers such as Slack that construct changed gRPC inputs

The contract artifact records the implementation re-scan result:

- core exposes no comparable owned editable GraphQL/gRPC update contract for this rollout
- Microsoft Teams exposes cache/subscriber and repository update paths, but no comparable owned editable GraphQL/gRPC
  update contract
- Slack owns changed booking, customer, location, and team gRPC consumers and does not add a separate owned editable
  update contract for this rollout

Re-scan before closure if a new remaining-domain editable update surface lands while implementation is in progress.

## 3. Implement owning-domain patch semantics

- Keep one normal public `Update*` mutation or RPC per migrated surface.
- Add required typed `fieldsToUpdate` selection and allowlisted patch fields for each update surface.
- Apply only selected fields or aggregate edit units inside the owning service and mapper.
- Preserve omitted values and distinguish explicit clear/default values from omission.
- Reject unsupported field selection atomically.
- Accept valid no-op updates and return the latest surface details.
- Reload latest state and retry only selected fields after detected concurrency conflicts.
- Remove dead full-replacement paths or temporary patch aliases once the surface is migrated.

## 4. Migrate autosave edit units

- Update user-facing screens that consume migrated GraphQL mutations.
- Autosave independent values as field units.
- Autosave related values as grouped edit units where current validation requires them to move together.
- Remove redundant update buttons for autosaved values while retaining explicit non-save workflow actions.
- Show save and failure feedback at the affected edit area.

Current web callers include booking editors, customer profile and billing screens, location and resource editors,
marketplace product editors, and related team screens when those update surfaces are present.

## 5. Add logging and tests

Add structured logs for:

- patch update started and completed
- selected fields or grouped edit units accepted
- no-op updates
- invalid or unsupported field selection
- validation and authorisation rejection
- concurrency reload and retry
- changed gRPC integration boundaries
- persistence failure

Test expectations:

- Unit-test mapper/service application, allowlists, no-op handling, selected-field retry, validation, authorisation,
  and logging.
- Integration-test changed GraphQL mutation contracts in each owning domain.
- Integration-test changed gRPC update inputs and affected cross-domain consumers.
- Assert persisted state through repositories or query-layer methods in integration tests, never raw `DbContext`.
- Use Vitest and React Testing Library for autosave save/failure states and removal of redundant update buttons.

## 6. Regenerate generated surfaces

For backend GraphQL schema changes:

```bash
scripts/generate-graphql.sh
```

If web Relay operations change, regenerate Relay artifacts through the existing web generation command. gRPC generated
C# code follows the edited protobuf contracts during consuming builds.

## 7. Verify

Suggested focused verification starts with the affected domains and web apps:

```bash
dotnet test booking/apis/Booking.Api.UnitTests/Booking.Api.UnitTests.csproj --no-restore
dotnet test customer/apis/Customer.Api.UnitTests/Customer.Api.UnitTests.csproj --no-restore
dotnet test location/apis/Location.Api.UnitTests/Location.Api.UnitTests.csproj --no-restore
dotnet test marketplace/apis/Marketplace.Api.UnitTests/Marketplace.Api.UnitTests.csproj --no-restore
dotnet test team/apis/Team.Api.UnitTests/Team.Api.UnitTests.csproj --no-restore
dotnet test booking/domain/Booking.Domain.IntegrationTests/Booking.Domain.IntegrationTests.csproj --no-restore
dotnet test customer/domain/Customer.Domain.IntegrationTests/Customer.Domain.IntegrationTests.csproj --no-restore
dotnet test location/domain/Location.Domain.IntegrationTests/Location.Domain.IntegrationTests.csproj --no-restore
dotnet test marketplace/domain/Marketplace.Domain.IntegrationTests/Marketplace.Domain.IntegrationTests.csproj --no-restore
dotnet test team/domain/Team.Domain.IntegrationTests/Team.Domain.IntegrationTests.csproj --no-restore
pnpm --dir web/apps/webapp test
pnpm --dir web/apps/webapp-teams test
pnpm --dir web/apps/webapp-spaces test
```

Autosave regression test files (source-analysis pattern via `readFileSync`):

- `web/apps/webapp/src/components/booking/edit-booking-autosave.test.ts`
- `web/apps/webapp/src/components/organization/organizationTeam/organization-team-autosave.test.ts`
- `web/apps/webapp/src/components/organization/organizationLocation/organization-location-autosave.test.ts`
- `web/apps/webapp/src/components/mySettings/my-settings-autosave.test.ts`
- `web/apps/webapp/src/components/myBillingAndPayment/my-billing-and-payment-autosave.test.ts`
- `web/apps/webapp/src/components/product/editProduct/edit-product-autosave.test.ts`

The same autosave test files exist under `web/apps/webapp-teams/src/` and `web/apps/webapp-spaces/src/`.

## Implementation status

All five domain rollouts are complete as of this feature:

| Domain      | GraphQL patch | gRPC patch | Autosave UI | Logging |
| ----------- | ------------- | ---------- | ----------- | ------- |
| Booking     | ✓             | ✓          | ✓           | ✓       |
| Customer    | ✓             | ✓          | ✓           | ✓       |
| Location    | ✓             | ✓          | ✓           | ✓       |
| Marketplace | ✓             | N/A        | ✓           | ✓       |
| Team        | ✓             | ✓          | ✓           | ✓       |

See `specs/011-cross-domain-patch-updates/contracts/cross-domain-patch-contracts.md` for the full migrated-surface
completion matrix and no-surface domain findings.
