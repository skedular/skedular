# Generated Surface Inventory: Skedular Spaces Pricing

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

Expected triggers for Spaces pricing:

| What to Add | Schema Location | Trigger |
|-------------|-----------------|---------|
| `pricingCatalogSpaces` query | Organization API | Read catalog with Free/Growth/Business/Contact Us plans |
| `organizationSpacesSubscription` query | Organization API | Read subscription state (plan, capacity, rollover date) |
| `updateOrganizationSpacesSubscription` mutation | Organization API | Assign plan (Free/Growth/Business), set custom capacity |
| `spacesQuotaStatus` query | Booking API | Current usage, quota limit, remaining, upgrade plans |
| `createPrivateBookingWithSpacesQuota` mutation | Booking API | Create booking with quota check, return error payload on exceed |

New GraphQL types to add:

```graphql
# Organization API extensions
extend type Query {
    pricingCatalogSpaces: PricingCatalogOffering!
    organizationSpacesSubscription(organizationId: ID!): OrganizationSpacesSubscription!
}

extend type Mutation {
    updateOrganizationSpacesSubscription(input: UpdateOrganizationSpacesSubscriptionInput!): OrganizationSpacesSubscription!
}

type OrganizationSpacesSubscription {
    id: ID!
    organizationId: ID!
    planCode: PricingCatalogSubscriptionPlanCode!
    commercialModel: PricingCatalogCommercialModel!
    currentPeriodStart: DateTime!
    currentPeriodEnd: DateTime!
    quotaLimit: Int!
    currentUsage: Int!
    remainingQuota: Int!
    rolloverDate: DateTime
}

enum SpacesQuotaReasonCode {
    FREE_TIER_LIMIT_EXCEEDED
    PAID_TIER_LIMIT_EXCEEDED
    CUSTOM_CAPACITY_EXCEEDED
    NO_SUBSCRIPTION_STATE
}
```

```graphql
# Booking API extensions
extend type Query {
    spacesQuotaStatus(organizationId: ID!, from: DateTime!, until: DateTime!): SpacesQuotaStatus!
}

type SpacesQuotaStatus {
    organizationId: ID!
    currentPeriodStart: DateTime!
    currentPeriodEnd: DateTime!
    quotaLimit: Int!
    currentUsage: Int!
    remainingQuota: Int!
    outOfPeriodCount: Int!
    quotaExceeded: Boolean!
    reasonCode: SpacesQuotaReasonCode
    upgradePlans: [UpgradePlan!]!
}

type UpgradePlan {
    planCode: PricingCatalogSubscriptionPlanCode!
    name: String!
    availability: PricingCatalogPlanAvailability!
    priceDescription: String
}
```

## Events

Primary source definitions:

- `api-definitions/events/skedular/organization_v1_value.proto`
- `api-definitions/events/skedular/organization_internal_v1_value.proto` (if internal projection needed)

Required generator:

```bash
api-definitions/events/generate.sh
```

Expected triggers:

- publishing Spaces subscription assignment from Organization
- adding `SpacesSubscriptionAssigned` event with plan, capacity, period info

## OpenAPI

Optional for this feature. GraphQL remains the primary client-facing surface.

If REST is needed (e.g., for admin tooling):

```yaml
# api-definitions/openapi/skedular/organization/workaround-v1.yaml
paths:
  /v1/organizations/{organizationId}/spaces-subscription:
    get: ...
    patch: ...  # custom capacity update
```

Required generator:

```bash
api-definitions/openapi/generate.sh
```

## Generated Files Rule

Do not hand-edit generated GraphQL schemas, OpenAPI controller bases, OpenAPI clients, event protobuf generated C# classes, or Relay artifacts. Change source definitions and run the matching generator.
