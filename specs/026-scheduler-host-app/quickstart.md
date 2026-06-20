# Quickstart: Skedular Host

## Generate and build

```bash
make generate
dotnet build src/Skedular.slnx --no-restore
cd src/web/apps/webapp-host
pnpm relay
pnpm build
```

## Validation flow

1. Create an Organization with `addOrganization(input: { type: HOST, ... })`.
2. Confirm its default offering is `HOST_STANDARD_V1`, catalog version is `HOST_V1`, and commission is 5%.
3. Create a draft Location through `addLocation` using `type: MARKETPLACE`.
4. Wait for Location provisioning, then create a Product through `addProduct`, passing the Location-provisioned Product Tag in `tagIds`, one or more pricing options, and card as the accepted payment method.
5. Confirm the Marketplace service forces Product type `EVENT`.
6. Confirm Location creation starts the provisioning workflow, which creates one Product Tag through Organization and one hidden Entire Location Resource carrying that tag.
7. Verify a second Product with another cadence reuses the same Resource.
8. Verify direct Resource add/update/delete/activate/deactivate operations are rejected for Host organizations.
9. Verify Product activation fails while ownership is unverified, succeeds after admin verification, and public `marketplaceLocations` excludes unverified organizations.
10. Purchase the Product through canonical `addMarketplaceBooking` using card payment. Verify overlapping full-place bookings conflict.
11. Confirm Booking persists the 5% rate, commission amount, and Host payout and sends the commission as Stripe Checkout's application fee.
12. Open `webapp-host` and verify Relay-backed Location/Product management and booking/commission history.

## Focused checks

```bash
dotnet test src/organization/apis/Organization.Api.UnitTests/Organization.Api.UnitTests.csproj --no-restore
dotnet test src/organization/shared/Organization.Shared.UnitTests/Organization.Shared.UnitTests.csproj --no-restore
dotnet test src/location/shared/Location.Shared.UnitTests/Location.Shared.UnitTests.csproj --no-restore
dotnet test src/location/apis/Location.Api.UnitTests/Location.Api.UnitTests.csproj --no-restore
dotnet test src/marketplace/apis/Marketplace.Api.UnitTests/Marketplace.Api.UnitTests.csproj --no-restore
dotnet test src/booking/shared/Booking.Shared.UnitTests/Booking.Shared.UnitTests.csproj --no-restore
pnpm --dir src/web/apps/webapp-host test:e2e
```

Integration tests inspect persistence through repositories, never through a DbContext.
