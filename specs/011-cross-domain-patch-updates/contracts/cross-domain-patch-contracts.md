# Contract: Cross-Domain Patch Update Surfaces

## Scope

Contract guidance for migrating every remaining owned update surface for editable domain state after the organisation
rollout. Migrated public surfaces keep one normal `Update*` GraphQL mutation or gRPC RPC name and carry patch semantics
through explicit allowlisted field selection.

## Current inventory

| Owner       | GraphQL update surfaces found                                                                                                                                                                                                  | gRPC update inputs found                                                       | Consumer notes                                                                                    |
| ----------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------- |
| Booking     | `updatePrivateBooking`, `updateMarketplaceBooking`, `updatePrivateRecurringBooking`                                                                                                                                            | `BookingService.UpdatePrivate`                                                 | Private booking gRPC has Slack consumers.                                                         |
| Customer    | `updateMyCustomerDetails`, `updateCustomerDetails`, `updateMyBillingDetails`                                                                                                                                                   | `CustomerAdminService.Admin_UpdateIdentity`                                    | Web profile and billing screens consume GraphQL details/billing updates.                          |
| Location    | `updateLocation`, `updateLocationOpeningHours`, `updateLocationPhysicalAddress`, `updateLocationRestrictedInformation`, `updateFloorPlan`, `updateResourcePositions`, `updateResource`, `updateLocationResourceAvailableHours` | location core `Update` and `Admin_Update`; location resources `UpdateResource` | Web location/resource/floor-plan editors and Slack location/resource consumers must stay aligned. |
| Marketplace | `updateProduct`                                                                                                                                                                                                                | None identified in current gRPC search                                         | Product editor uses GraphQL.                                                                      |
| Team        | `updateTeam`, `updateTeamMembers`, `updateTeamAndTeamMembers`                                                                                                                                                                  | `TeamService.Update`                                                           | Team update RPC has Slack consumers.                                                              |

The implementation re-scan found core and Microsoft Teams cache/subscriber and repository update paths but no owned
editable GraphQL/gRPC update contract comparable to the table above. Slack owns app handlers and gRPC consumers for the
booking, customer, location, organization, and team contracts; only the booking, customer, location, and team inputs in
the table above change in this rollout. Re-run the inventory if a new remaining-domain editable update surface lands
before closure because the feature scope is domain-complete.

## GraphQL contract pattern

For each migrated GraphQL mutation, preserve the public mutation and normal input family while adding explicit typed
field selection:

```graphql
type Mutation {
  updatePrivateBooking(input: UpdatePrivateBookingInput!): BookingPayload!
}

input UpdatePrivateBookingInput {
  clientMutationId: String
  id: String!
  fieldsToUpdate: [PrivateBookingPatchField!]!
  # existing values remain typed and are applied only when selected
}

enum PrivateBookingPatchField {
  # concrete field or grouped edit-unit values are owned by the booking contract
}
```

The concrete enum names and allowed fields stay domain-specific. Field selection may use aggregate patch fields where
several submitted values must validate together, for example time/resource booking details, product editor sections, or
weekly availability values.

## gRPC contract pattern

Where a remaining domain already exposes an internal update RPC, edit the owning protobuf input rather than creating a
parallel RPC:

```proto
service BookingService {
  rpc UpdatePrivate (UpdatePrivateInput) returns (Booking);
}

enum PrivateBookingPatchField {
  // Concrete allowlisted values are owned by booking.
}

message UpdatePrivateInput {
  string id = 1;
  repeated PrivateBookingPatchField fieldsToUpdate = 2;
  // existing selected values remain in this typed input
}
```

Existing field numbers must remain stable according to protobuf compatibility rules. New field-selection members should
be added without hand-editing generated C# outputs.

## Behaviour contract

- Only selected allowlisted fields or aggregate edit units are applied.
- Empty, default-like, and clear values are meaningful only when their field is selected.
- Omitted values remain unchanged.
- Selected values validate under the owning domain's existing rules.
- Invalid or unsupported field selection rejects the update atomically.
- Valid no-op updates succeed and return the latest details for that update surface.
- On detected concurrency conflict, the owning service reloads the latest state and retries only the selected fields.
- Removed full-replacement paths and temporary patch aliases do not remain beside the migrated public update contract.

## UI autosave contract

- Autosave exists only for user-facing screens that consume migrated update surfaces.
- Independent values can autosave as field-level edit units.
- Related values that must validate together autosave as grouped edit units.
- Redundant update buttons are removed for autosaved values.
- Explicit creation, deletion, confirmation, workflow submission, and other non-save actions may remain.
- Saving and failure states are visible at the affected edit area; successful result details reconcile the view.

## Required regeneration

- Run `scripts/generate-graphql.sh` after changed backend GraphQL mutation schemas.
- Regenerate web Relay artifacts through the existing web generation flow after Relay operations or GraphQL schema
  inputs change.
- gRPC source definitions live under `api-definitions/grpc/skedular`; consuming builds regenerate C# outputs from those
  protobufs.
- Do not hand-edit exported GraphQL schema files, Relay artifacts, or generated gRPC output.

## Required contract tests

- Verify each migrated update surface requires explicit allowlisted field selection.
- Verify selected single-field and grouped-edit updates preserve omitted fields.
- Reject invalid or unsupported field selections atomically.
- Accept valid no-op updates and return latest details.
- Retry selected fields after detected concurrency conflicts without overwriting omitted values.
- Verify removed full-replacement or duplicate patch aliases are no longer public after migration.
- Verify changed gRPC consumers build and pass integration coverage with field selection populated.

## Migrated surface completion matrix

| Domain      | Surface                                            | Patch field enum                           | Status     |
| ----------- | -------------------------------------------------- | ------------------------------------------ | ---------- |
| Booking     | `updatePrivateBooking` (GraphQL)                   | `PrivateBookingPatchField`                 | ✓ Complete |
| Booking     | `updateMarketplaceBooking` (GraphQL)               | `MarketplaceBookingPatchField`             | ✓ Complete |
| Booking     | `updatePrivateRecurringBooking` (GraphQL)          | `PrivateRecurringBookingPatchField`        | ✓ Complete |
| Booking     | `BookingService.UpdatePrivate` (gRPC)              | `PrivateBookingPatchField`                 | ✓ Complete |
| Customer    | `updateMyCustomerDetails` (GraphQL)                | `CustomerDetailsPatchField`                | ✓ Complete |
| Customer    | `updateCustomerDetails` (GraphQL, admin)           | `CustomerDetailsPatchField`                | ✓ Complete |
| Customer    | `updateMyBillingDetails` (GraphQL)                 | `CustomerBillingDetailsPatchField`         | ✓ Complete |
| Customer    | `CustomerAdminService.Admin_UpdateIdentity` (gRPC) | `CustomerIdentityPatchField`               | ✓ Complete |
| Location    | `updateLocation` (GraphQL)                         | `LocationPatchField`                       | ✓ Complete |
| Location    | `updateLocationOpeningHours` (GraphQL)             | `LocationOpeningHoursPatchField`           | ✓ Complete |
| Location    | `updateLocationPhysicalAddress` (GraphQL)          | `LocationPhysicalAddressPatchField`        | ✓ Complete |
| Location    | `updateLocationRestrictedInformation` (GraphQL)    | `LocationRestrictedInformationPatchField`  | ✓ Complete |
| Location    | `updateFloorPlan` (GraphQL)                        | `FloorPlanPatchField`                      | ✓ Complete |
| Location    | `updateResourcePositions` (GraphQL)                | `ResourcePositionsPatchField`              | ✓ Complete |
| Location    | `updateResource` (GraphQL)                         | `ResourcePatchField`                       | ✓ Complete |
| Location    | `updateLocationResourceAvailableHours` (GraphQL)   | `LocationResourceAvailableHoursPatchField` | ✓ Complete |
| Marketplace | `updateProduct` (GraphQL)                          | `ProductPatchField`                        | ✓ Complete |
| Team        | `updateTeam` (GraphQL)                             | `TeamPatchField`                           | ✓ Complete |
| Team        | `updateTeamMembers` (GraphQL)                      | `TeamMemberPatchField`                     | ✓ Complete |
| Team        | `updateTeamAndTeamMembers` (GraphQL)               | `TeamPatchField` + `TeamMemberPatchField`  | ✓ Complete |
| Team        | `TeamService.Update` (gRPC)                        | `TeamPatchField`                           | ✓ Complete |

## No-surface domain findings

- **Core**: No owned editable GraphQL or gRPC update contract comparable to the table above. Cache, subscriber, and repository update paths exist but are internal service implementations, not public API surfaces. Out of scope.
- **Microsoft Teams (msteams)**: No owned editable update surface. Consumes booking, customer, location, and team contracts as a downstream subscriber. Out of scope.
