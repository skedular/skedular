# Data Model: Organization Patch Updates

## Organisation

Represents the customer-owned organisation record.

### Relevant fields

- `id`: Existing organisation identifier used to target the update.
- `customDomain`: Existing organisation domain identifier alternative used to target the update.
- `name`: Patchable organisation name.
- `description`, `title`, `subTitle`: Patchable organisation listing metadata values.
- `customDomain`, `website`, `logoUrl`, `customerFacingTermsAndConditionsUrl`: Patchable organisation identity and public URL values.
- `contactEmail`, `contactPhone`, `refundNotificationEmails`: Patchable contact and notification values.
- `industrySubCategoryIds`, `featureImages`, `marketplaceListingMetadata`, `billingCycle`, `invoiceDueInDays`: Patchable setup values supported by the typed patch contract.
- Existing entity concurrency token: Persistence-layer concurrency protection used by the existing .NET entity model; not exposed through the GraphQL patch API.

### Validation rules

- A partial update must identify exactly one existing organisation by `id` or `customDomain`.
- Selected values must follow the same validation and authorisation rules as their equivalent organisation setup updates.
- Fields outside the patch allowlist must be rejected.
- Omitted organisation fields must remain unchanged.
- Explicit clear/default values must be applied only when the selected field allows that value.

## Patch Field Selection

Represents caller intent for which fields are included in the partial update.

### Fields

- `fieldsToUpdate`: The selected organisation fields the caller intends to update, represented by the `OrganizationPatchField` enum.
- Allowed values: the explicit `OrganizationPatchField` enum values for editable organisation setup fields.

### Validation rules

- `fieldsToUpdate` must not include unknown, deprecated, or unsupported organisation fields.
- `fieldsToUpdate` may include unchanged values; those are accepted as no-op updates.
- `fieldsToUpdate` must be checked before applying updates so disallowed fields cannot be partially persisted.

## Patch Update Request

Represents a field-masked organisation update request carrying both values and caller intent.

### Fields

- `clientMutationId`: GraphQL mutation correlation value when the update arrives through GraphQL.
- `id` or `customDomain`: Target organisation lookup value.
- `fieldsToUpdate`: Patch field selection, represented by `OrganizationPatchField` enum values.
- Each patchable input value is optional and applied only when its matching enum value is selected.
- Migrated gRPC update inputs use the same field-mask concept with repeated patch-field enums.

### State transitions

```text
Submitted
├── Rejected: organisation not found
├── Rejected: unauthorised
├── Retried: entity concurrency conflict detected, latest organisation reloaded, selected fields reapplied
├── Rejected: invalid or disallowed field selection
├── Rejected: invalid selected value
├── Accepted: selected values already match persisted values, latest organisation details returned
└── Applied: one or more selected values changed, latest organisation details returned
```

### Validation rules

- Validate the target organisation before applying changes.
- Validate authorisation before applying changes.
- If the persistence layer reports a concurrency failure, reload the latest organisation and retry only the selected patch fields.
- Validate the entire selected field set before applying any value.
- Apply all selected valid changes atomically.

## Update Result

Represents the mutation result returned to the caller.

### Validation rules

- A result must include the latest organisation details so the caller can refresh and reconcile the displayed organisation state.
- Field-level validation failures must identify the selected field that failed.
- Logs must record the processing branch without logging sensitive payload values.
