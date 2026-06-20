# Skedular Spaces Trial Implementation Inventory

## Contract and generated surfaces

| Source | Generated or derived surfaces | Required command | Order |
|---|---|---|---|
| `api-definitions/events/skedular/organization_v1_value.proto` | Protobuf Organization event classes in `src/shared/Api.Shared.Clients/obj` | `api-definitions/events/generate.sh` | Run immediately after the protobuf change and before producer/consumer code changes. Generated event classes are not committed. |
| Organization and Booking GraphQL resolver/type code | Per-API `schema.graphqls`, composed `api-definitions/graphql/skedular/v1/schema.graphql`, gateway/Fusion outputs, integration-test schemas | `scripts/generate-graphql.sh` | Run after each server contract slice and before Relay consumer changes. |
| Composed GraphQL schema | Relay artifacts for `webapp`, `webapp-spaces`, and `webapp-teams` | `src/web/apps/webapp/scripts/generate.sh` | Run after GraphQL composition and before editing consumers. |
| All contract sources | All generated surfaces above plus OpenAPI clients when applicable | `make generate` | Final drift check only; no OpenAPI source change is planned for this feature. |

Never hand-edit exported GraphQL schemas, Relay artifacts, or protobuf-generated event classes.

## Booking creation and commitment boundaries

All API boundaries delegate to Booking-owned services. They must never call another domain API synchronously. Organization commercial inputs arrive through the Organization event subscriber and are evaluated from Booking's local projection.

| Path | Entry point | Owning enforcement gate | Required tests |
|---|---|---|---|
| Administrator private booking | `src/booking/apis/Booking.Api/GraphQL/Booking/RootMutation.cs` → `src/booking/apis/Booking.Api/Services/PrivateBookingService.cs` | `src/booking/shared/Booking.Shared/Services/SpacesBookingQuotaService.cs` before `PrivateBookingService.AddAsync` persists or reserves resources | Booking API and shared service exact-boundary tests; GraphQL integration test |
| Customer marketplace booking | `src/booking/apis/Booking.Api/GraphQL/Booking/RootMutation.cs` → `src/booking/apis/Booking.Api/Services/MarketplaceBookingService.cs` | Access-first decision before `MarketplaceBookingService.AddAsync` persists or starts payment/resource work | Booking API/shared tests, customer stale-client test, GraphQL integration test |
| Internal gRPC private booking | `src/booking/apis/Booking.Api/Grpc/BookingGrpcService.cs` | Same API/shared private booking service gate; no bypass in the gRPC adapter | gRPC integration test at exact expiry |
| Private recurring definition | `src/booking/apis/Booking.Api/GraphQL/RecurringBooking/RootMutation.cs` | Recurring service validates access before creating a new recurring commitment | Recurring mutation/service tests |
| Private recurring materialization | `src/booking/shared/Booking.Shared/Activities/PrivateRecurringBookingIntegrations.cs` | Re-evaluate access for every generated booking instance before `PrivateBookingService.AddAsync` | Activity tests for active, exact-expiry, cleanup, and no replacement commitment |
| Marketplace subscription definition | `src/booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/RootMutation.cs` | Subscription service validates access before starting the workflow | Mutation/service tests |
| Marketplace subscription materialization | `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs` | Re-evaluate access for every generated marketplace booking before `MarketplaceBookingService.AddAsync` | Activity/workflow tests for active, exact-expiry, cancellation, and renewal suppression |
| Booking jobs and Temporal workflows | Registrations in `src/booking/jobs/Booking.Jobs/Program.cs` | Activities above are the durable enforcement boundary; workflow retries cannot bypass them | Temporal integration tests and structured retry/denial log assertions |
| Import paths | No dedicated Booking import creator exists at feature start | Any future importer must call the same Booking-owned service gate; direct repository creation is prohibited | Inventory regression search plus service/integration tests if an importer is discovered during implementation |
| Automation and external adapters | GraphQL, gRPC, and Temporal/job adapters listed above | Adapter delegates to a gated Booking service or activity; no direct persistence | Boundary inventory regression search and representative adapter tests |

## Non-booking operational boundaries

| Domain | Boundary | Decision source |
|---|---|---|
| Location | `src/location/apis/Location.Api/Services/Authorization/OrganizationOfferingService.cs` and Location/Resource/FloorPlan mutations | Location's local Organization projection populated by its processor subscriber |
| Marketplace | `src/marketplace/apis/Marketplace.Api/Services/ProductService.cs` product create/update/activate boundaries | Marketplace's local Organization projection populated by its processor subscriber; delete/deactivate remain protective |
| Organization | Organization-owned operational mutations | Organization's authoritative subscription state; account, billing, support read, and upgrade remain allowed |
| Public storefront | Booking subgraph extension of federated `Organization` | Booking's local projection; neutral UI hint only, followed by authoritative mutation-time re-evaluation |

## Verification search

Before completion, search for direct booking persistence and every call to `PrivateBookingService.AddAsync` or `MarketplaceBookingService.AddAsync`. Any new path must be added to this inventory and covered by the shared access gate.
