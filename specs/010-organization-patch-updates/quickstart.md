# Quickstart: Organization Patch Updates

## 1. Review source contracts

- Read `specs/010-organization-patch-updates/spec.md`.
- Read `specs/010-organization-patch-updates/contracts/organization-patch-update.graphql.md`.
- Inspect existing organisation update code:
  - `organization/apis/Organization.Api/GraphQL/Organization/UpdateOrganizationInput.cs`
  - `organization/apis/Organization.Api/GraphQL/Organization/RootMutation.cs`
  - `organization/apis/Organization.Api/Services/OrganizationService.cs`
  - `organization/apis/Organization.Api/Mappers/GraphQlMapper.cs`

## 2. Implement contract and service behaviour

- Use the GraphQL update mutation `updateOrganization` with `fieldsToUpdate`.
- Add explicit enum-list patch field selection named `fieldsToUpdate` for all editable organisation setup fields.
- Remove dead full-replacement update methods and temporary public `*Patch` aliases once a surface has been migrated.
- Use `updateOrganizationSsoSettings` for organisation SSO settings. Treat the SSO values as one aggregate `SSO_SETTINGS` patch field because they are validated together.
- Reject non-allowlisted patch fields.
- Rely on existing entity-layer concurrency protection; on concurrency failure, reload the latest organisation and retry only the selected patch fields.
- Accept no-op patches and return the latest organisation details.
- Keep normal public GraphQL `Update*` names for the migrated specialised update surfaces while requiring `fieldsToUpdate`.
- Keep normal public gRPC `Update*` RPC names for billing details, tag, custom tag, product tag, and zone updates while requiring field-masked inputs.

## 3. Add logging

Add structured logs for:

- patch update started
- patch update completed with applied changes
- patch update completed with the latest organisation details returned
- concurrency retry attempted
- invalid/disallowed field selection rejected
- validation failure rejected
- authorisation failure rejected
- persistence failure

Logs must include request/correlation context and organisation identifiers where safe, but must not include sensitive payload values.

## 4. Add tests

- Unit-test field selection, allowlist rejection, no-op handling, concurrency retry, validation, authorisation, and logging behaviour.
- Integration-test the GraphQL mutation contract through `organization/domain/Organization.Domain.IntegrationTests`.
- Unit-test web setup saves in all three web apps so inline fields and full-form setup submits use `updateOrganization`, show inline saving state, suppress success toasts, and show failure toasts.
- Unit-test web SSO settings saves in all three web apps so they use `updateOrganizationSsoSettings` with `fieldsToUpdate: [SSO_SETTINGS]`.
- Use repository/query-layer methods for persisted-state assertions; do not query EF `DbContext` directly from tests.
- Confirm removed `*Patch` public aliases are gone and organisation setup scenarios pass through field-masked updates.

## 5. Regenerate generated surfaces

Run from repository root after GraphQL schema changes:

```bash
scripts/generate-graphql.sh
```

If web Relay operations are changed or added, regenerate web artifacts through the existing web generation command.

## 6. Verify

Suggested verification commands:

```bash
dotnet test organization/apis/Organization.Api.UnitTests/Organization.Api.UnitTests.csproj --no-restore
dotnet test organization/domain/Organization.Domain.IntegrationTests/Organization.Domain.IntegrationTests.csproj --no-restore --filter FullyQualifiedName~Organization
pnpm --dir web/apps/webapp test src/components/organization/organizationAdmin/organization-admin-setup-section.test.tsx
pnpm --dir web/apps/webapp-teams test src/components/organization/organizationAdmin/organization-admin-setup-section.test.tsx
pnpm --dir web/apps/webapp-spaces test src/components/organization/organizationAdmin/organization-admin-setup-section.test.tsx
pnpm --dir web/apps/webapp test src/components/organization/organizationAdmin/organization-admin-sso-section.test.tsx
pnpm --dir web/apps/webapp-teams test src/components/organization/organizationAdmin/organization-admin-sso-section.test.tsx
pnpm --dir web/apps/webapp-spaces test src/components/organization/organizationAdmin/organization-admin-sso-section.test.tsx
pnpm --dir web/apps/webapp lint
pnpm --dir web/apps/webapp-teams lint
pnpm --dir web/apps/webapp-spaces lint
```
