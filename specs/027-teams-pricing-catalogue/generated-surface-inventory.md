# Generated Surface Inventory

## GraphQL

Primary generated surfaces:

- per-API `schema.graphql` files
- composed gateway schema at `api-definitions/graphql/skedular/v1/schema.graphql`
- system/integration test GraphQL schema files
- web Relay artifacts when web app queries consume new fields

Required generator:

```bash
scripts/generate-graphql.sh
```

Expected triggers:

- adding `pricingCatalog`
- adding `organizationTeamsSubscription`
- adding `updateOrganizationTeamsSubscription`
- adding pricing choice-detail queries
- adding entitlement reason-code choice details

## Events

Primary source definitions:

- `api-definitions/events/skedular/organization_v1_value.proto`
- possibly `api-definitions/events/skedular/organization_internal_v1_value.proto` if internal-only projection state is needed

Required generator:

```bash
api-definitions/events/generate.sh
```

Expected triggers:

- publishing pricing/subscription JSON projection state from Organization
- adding catalog version, product offering, plan, capacity, or entitlement projection fields

## OpenAPI

Primary source definitions:

- `api-definitions/openapi/skedular/organization/organization_core_v1.yaml`
- `api-definitions/openapi/skedular/organization/organization_graphql_v1.yaml` if current conventions require GraphQL-hosted surfaces there

Required generator:

```bash
api-definitions/openapi/generate.sh
```

Web client generator if consumed by web apps:

```bash
src/web/apps/webapp/scripts/generate.sh
```

OpenAPI is optional for this feature. GraphQL remains the primary client-facing surface unless implementation discovers a static/public website need for REST.

## Generated Files Rule

Do not hand-edit generated GraphQL schemas, OpenAPI controller bases, OpenAPI clients, event protobuf generated C# classes, or Relay artifacts. Change source definitions and run the matching generator.
