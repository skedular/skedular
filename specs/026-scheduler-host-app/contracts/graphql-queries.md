# GraphQL Contracts: Skedular Host

Host uses the composed GraphQL schema and canonical Organization, Location, Marketplace, and Booking operations. It does not add a Host REST API.

## Organization

```graphql
query HostOrganizations { myOrganizations(types: [HOST]) { uniqueId name } }
```

Create through `addOrganization` with `type: HOST`. Admin ownership verification uses the Organization admin REST boundary because it is an operator action. `OrganizationOfferingDetails.hostCommissionPercentage` exposes the active offering rate.

## Location management

```graphql
query HostLocations($organizationId: String!) {
  myLocations(organizationId: $organizationId) {
    id name timezone physicalAddress { multilinesFormattedAddress } products { id }
  }
}

mutation AddHostLocation($input: AddLocationInput!) {
  addLocation(input: $input) { location { id name } }
}

mutation UpdateHostLocation($input: UpdateLocationInput!) {
  updateLocation(input: $input) { location { id name } }
}

mutation UpdateHostLocationAddress($input: UpdateLocationPhysicalAddressInput!) {
  updateLocationPhysicalAddress(input: $input) { location { id } }
}
```

Host locations use `LocationType.MARKETPLACE`. Resource operations are intentionally absent from Host UI surfaces.

## Product management

```graphql
query HostLocationProducts($locationId: String!) {
  location(id: $locationId) {
    id name
    products { id inactive type { name } listingMetadata { title } pricingOptions { price bookingCadence } }
  }
}

mutation AddHostProduct($input: AddProductInput!) {
  addProduct(input: $input) { product { id } }
}

mutation UpdateHostProduct($input: UpdateProductInput!) {
  updateProduct(input: $input) { product { id } }
}
```

Host Product creation requires the Product Tag provisioned for the selected Host Location in `tagIds`. The service forces `EVENT`, the Host UI uses card-only pricing, and the existing booking engine matches the Product Tag to the Location's hidden Entire Location Resource.

## Booking history and commission

```graphql
query HostBookingHistory($organizationId: String!) {
  bookings(first: 100, where: { organizationId: $organizationId }) {
    edges {
      node {
        id from until involvedLocations { name }
        marketplaceBooking {
          totalAmount
          hostCommissionRatePercentage
          hostCommissionAmount
          hostPayoutAmount
          paymentStatus { name }
        }
      }
    }
  }
}
```

Guest purchases use canonical `addMarketplaceBooking`. Host bookings accept card payment only and use Stripe Connect Checkout with the Host commission as the application fee.

## Public discovery

`marketplaceLocations` returns only verified public organizations. Clients distinguish Host listings through `organization.type.type == HOST` and may filter the returned map collection by organization type.
