# Contract: Organization Patch Update

## Scope

Contract for field-masked organisation updates. Public update contracts keep normal `Update*` names; their
`fieldsToUpdate` masks provide the patch semantics and replace the old full-object update behaviour.

GraphQL setup and specialised organisation update surfaces use this pattern. Migrated gRPC billing details, tag, custom
tag, product tag, and zone endpoints use the same field-mask principle with their normal `Update*` RPC names.

## Mutation surface

```graphql
type Mutation {
  updateOrganization(input: UpdateOrganizationInput!): OrganizationPayload!
  updateOrganizationSsoSettings(input: UpdateOrganizationSsoSettingsInput!): OrganizationPayload!
  updateOrganizationBillingDetails(input: UpdateOrganizationBillingDetailsInput!): OrganizationPayload!
  updateOrganizationTaxDetails(input: UpdateOrganizationTaxDetailsInput!): OrganizationPayload!
  updateOrganizationBankAccount(input: UpdateOrganizationBankAccountInput!): OrganizationBankAccountPayload!
  updateOrganizationOffering(input: UpdateOrganizationOfferingInput!): UpdateOrganizationOfferingPayload!
  updateCustomTag(input: UpdateOrganizationTagInput!): OrganizationTagPayload!
  updateZone(input: UpdateOrganizationTagInput!): OrganizationTagPayload!
  updateProductTag(input: UpdateOrganizationTagInput!): OrganizationTagPayload!
  updateOrganizationStripeConnectAccount(input: UpdateOrganizationStripeConnectAccountInput!): OrganizationStripeConnectAccountPayload!
  updateOrganizationXeroConnection(input: UpdateOrganizationXeroConnectionInput!): OrganizationPayload!
}
```

## Planned input shape

The implementation exposes one field-masked input as the organisation setup update surface:

```graphql
input UpdateOrganizationInput {
  clientMutationId: String
  id: String
  customDomain: String
  fieldsToUpdate: [OrganizationPatchField!]!
  name: String
  description: String
  title: String
  subTitle: String
  website: String
  logoUrl: String
  customerFacingTermsAndConditionsUrl: String
  billingCycle: OrganizationBillingCycle
  invoiceDueInDays: Int
  contactEmail: String
  contactPhone: String
  refundNotificationEmails: [String!]
  industrySubCategoryIds: [String!]
  featureImages: [CdnImageFileInput!]
  marketplaceListingMetadata: ListingMetadataInput
}

enum OrganizationPatchField {
  NAME
  DESCRIPTION
  TITLE
  SUB_TITLE
  CUSTOM_DOMAIN
  WEBSITE
  LOGO_URL
  CUSTOMER_FACING_TERMS_AND_CONDITIONS_URL
  BILLING_CYCLE
  INVOICE_DUE_IN_DAYS
  CONTACT_EMAIL
  CONTACT_PHONE
  REFUND_NOTIFICATION_EMAILS
  INDUSTRY_SUB_CATEGORIES
  FEATURE_IMAGES
  MARKETPLACE_LISTING_METADATA
}

// Reuses the existing OrganizationPayload shape.
```

SSO settings use this specialised field-mask shape:

```graphql
input UpdateOrganizationSsoSettingsInput {
  clientMutationId: String
  organizationId: String
  organizationCustomDomain: String
  fieldsToUpdate: [OrganizationSsoSettingsPatchField!]!
  entityId: String!
  loginUrl: String!
  appFederationMetadataUrl: String!
  isActive: Boolean!
}

enum OrganizationSsoSettingsPatchField {
  SSO_SETTINGS
}
```

## Behaviour contract

- Only fields listed in `fieldsToUpdate` are considered for update.
- The old full-replacement behaviour is removed from the GraphQL organisation update path.
- `NAME` applies the `name` value, including valid explicit empty/default values only if the existing business rule allows them.
- `DESCRIPTION` applies the `description` value, including valid explicit empty/default values only if the existing business rule allows them.
- Each other enum value applies only the matching input field and preserves all omitted organisation values.
- Any field outside `OrganizationPatchField` is rejected by the schema or validation.
- A selected field with an invalid value rejects the whole patch and applies no changes.
- A valid no-op patch succeeds and returns the latest organisation details.
- If entity concurrency fails during save, the service reloads the latest organisation and retries only the selected patch fields.
- The payload uses `OrganizationPayload` and must return the latest organisation details for Relay/UI reconciliation after success.
- SSO settings use a single aggregate `SSO_SETTINGS` patch field because the submitted values are validated together against the SSO metadata and certificate checks.
- The public GraphQL update name stays `updateOrganizationSsoSettings`; web callers distinguish a partial update through `fieldsToUpdate`.

## Migrated gRPC update surface

```proto
service OrganizationBillingService {
  rpc UpdateBillingDetails (UpdateBillingDetailsInput) returns (BillingDetails);
}

service OrganizationTagsService {
  rpc UpdateTag (UpdateTagInput) returns (Tag);
  rpc UpdateCustomTag (UpdateTagInput) returns (CustomTag);
  rpc UpdateProductTag (UpdateTagInput) returns (ProductTag);
}

service OrganizationZonesService {
  rpc UpdateZone (UpdateZoneInput) returns (Zone);
}
```

Each migrated gRPC input includes a repeated `fieldsToUpdate` patch-field enum and applies only selected values.

## Required regeneration

- Run `scripts/generate-graphql.sh` after the backend GraphQL schema changes.
- If web Relay operations consume the changed mutation/input shape, regenerate Relay artifacts through the existing web generation flow.
- Do not hand-edit exported schema files or generated Relay artifacts.

## Required contract tests

- Patch single fields such as `NAME`, `DESCRIPTION`, `WEBSITE`, or `CONTACT_EMAIL`; all other organisation fields remain unchanged.
- Patch the full organisation setup form through `updateOrganization`; all selected fields update atomically.
- Reject disallowed or malformed field selections.
- Retry after entity concurrency conflict while preserving omitted fields.
- Accept valid no-op patch and return the latest organisation details.
- Confirm removed `*Patch` GraphQL aliases are no longer exposed or consumed by the web apps.
- Patch SSO settings through `updateOrganizationSsoSettings` and verify the latest organisation details are returned.
- Verify migrated GraphQL specialised update surfaces keep only one public update mutation and require `fieldsToUpdate`.
- Verify migrated gRPC billing, tag, and zone update inputs include explicit field masks.
