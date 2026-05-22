# Research: Cross-Domain Patch Updates

## Decision: Reuse the organisation field-mask contract rule

**Rationale**: The clarified spec makes the organisation migration the reference pattern. Each migrated surface keeps
one normal public `Update*` name while explicit field selection changes the semantics from full replacement to partial
update. That keeps GraphQL mutations and gRPC RPCs discoverable without keeping a second `*Patch` family alive.

**Alternatives considered**:

- Keep full-replacement and patch contracts in parallel: rejected because the full-replacement path retains the
  overwrite risk the rollout is meant to remove.
- Rename every migrated public surface to `Patch*`: rejected because the behaviour is a safer update contract, not a
  second business capability.
- Decide contract naming per domain: rejected because it would fragment the migration pattern before tasks begin.

## Decision: Migrate all owned editable update surfaces, not only screen-backed GraphQL mutations

**Rationale**: Code inventory shows remaining-domain update behaviour on both GraphQL and gRPC surfaces. Booking,
customer, location, marketplace, and team own editable update contracts today. Booking, customer, location, and team
also expose internal update RPCs under `api-definitions/grpc/skedular`, and Slack currently calls several booking,
location, customer, and team update RPCs. Autosave is a UI concern, but omitted-value safety must reach the owned
backend update contracts.

**Alternatives considered**:

- Migrate only web GraphQL mutations: rejected because internal update callers can still send full-object updates.
- Migrate only user-facing screens first: rejected because the clarified spec includes all existing editable update
  surfaces after organisation.
- Build a shared generic patch service across domains: rejected because validation, events, cache invalidation, and
  permissions remain owned by each domain.

## Decision: Current contract inventory is domain-owned and implementation must re-scan before tasks close

**Rationale**: The current GraphQL schema inventory identifies booking private, marketplace, and recurring booking
updates; customer details and billing updates; location, physical address, opening hours, restricted information,
floor plan, resource, and resource availability updates; marketplace product updates; and team/team-member updates.
The gRPC inventory identifies booking private booking, customer admin identity, location core/resource, and team update
inputs. Current scans found Slack as a consumer of affected gRPC contracts and did not identify comparable owned
editable update contracts in core or Microsoft Teams schemas. A final implementation scan is required because the spec
scope is domain-complete rather than limited to this first search result.

**Alternatives considered**:

- Treat the first search inventory as exhaustive: rejected because mutation/RPC naming and Slack app handlers can hide
  indirect consumers.
- Treat cache refresh or subscriber helper methods as editable update contracts: rejected because they are derived-state
  maintenance rather than public editable domain state.

## Decision: Use explicit allowlisted field selection everywhere

**Rationale**: Every migrated surface must distinguish omitted values from explicit empty, default, or clear values.
Allowlisted field enums or equivalent typed field selection make selected-field application testable and reject fields
that a surface does not support. The organisation rollout already proves the approach with `fieldsToUpdate`.

**Alternatives considered**:

- Infer intent from non-null input values: rejected because clear/default operations become ambiguous.
- Compare full input values to the latest stored object: rejected because callers still cannot express intentional clear
  versus omission.
- Use ad hoc per-domain string masks: rejected because typed allowlists integrate with generated GraphQL and gRPC
  contracts more safely.

## Decision: Preserve grouped edit and validation boundaries

**Rationale**: Remaining web screens include multi-field editors such as bookings, products, resources, locations, and
billing forms. Some values must validate together before persistence. Autosave therefore applies to coherent edit units:
independent values can save individually, while related fields remain grouped and still use field selection for the
selected aggregate fields.

**Alternatives considered**:

- Autosave every form control independently: rejected because related booking/product/location values can be invalid
  mid-edit.
- Keep all current update buttons for grouped editors: rejected because the clarified spec removes redundant update
  buttons for autosaved values.

## Decision: Retry selected fields after detected concurrency conflicts

**Rationale**: The organisation migration already relies on existing entity concurrency handling. Reusing reload and
retry for selected fields keeps omitted values protected while avoiding a new caller version token across every
remaining public contract.

**Alternatives considered**:

- Reject all conflicting autosaves: rejected because it creates unnecessary retry burden for inline edits.
- Let the latest full payload win: rejected because it reintroduces stale-field replacement.
- Add a cross-domain public concurrency version field first: rejected because existing entity-level concurrency already
  provides the detection point needed by this rollout.

## Decision: Regeneration and testing follow changed surface type

**Rationale**: GraphQL mutation changes affect exported schemas, gateway composition, integration test schemas, and web
Relay artifacts. gRPC protobuf changes are sourced under `api-definitions/grpc/skedular` and generated by consumers at
build time. The repository rules require generated outputs to come from the source contract and require repository or
query-layer assertions for integration test persistence checks.

**Alternatives considered**:

- Hand-edit exported GraphQL schemas or Relay files: rejected by repository policy.
- Skip internal gRPC integration coverage: rejected because changed update RPC inputs are contract changes.
