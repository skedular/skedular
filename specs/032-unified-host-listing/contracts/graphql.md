# GraphQL Contracts: Unified Host Listing

**Feature**: 032-unified-host-listing
**Date**: 2026-07-08

## Contract Strategy

The Host unified listing flow reuses existing GraphQL schema fields and mutations.
No new backend aggregation query/mutation is introduced for this feature.

Frontend coordination rules:
- Load location and linked listing configuration using existing location/product fields.
- Save location fields through existing Location mutations.
- Save pricing/rules/visibility through existing Product mutations.
- Use readiness signal to unlock listing configuration after location creation.

## Existing GraphQL Types (Reused)

### LocationDetails
Schema: LOCATION_API, CUSTOMER_API, TEAM_API

```graphql
type LocationDetails {
  id: ID\!
  name: String\!
  timezone: String
  openingHours: OpeningHours\!
  listingMetadata: ListingMetadata\!
  physicalAddress: LocationPhysicalAddressDetails
  resources: [Resource\!]\!
  products: [ProductDetails\!]\!
}
```

### ProductDetails
Schema: LOCATION_API, MARKETPLACE_API

```graphql
type ProductDetails {
  id: ID\!
  inactive: Boolean\!
  currency: CurrencyDetails\!
  pricingOptions: [ProductPricing\!]\!
  listingMetadata: ListingMetadata\!
  amenities: [OrganizationTagDetails\!]\!
  featureImages: [CdnImageFile\!]\!
}
```

## Frontend Query/Mutation Shape (Existing APIs)

### Listing Entry Query (existing fields)

```graphql
query HostListingQuery($locationId: String\!) {
  location(id: $locationId) {
    id
    name
    timezone
    physicalAddress {
      multilinesFormattedAddress
    }
    products {
      id
      inactive
      listingMetadata {
        title
        published
        marketplaceVisible
      }
      pricingOptions {
        id
        billingMode
        bookingCadence
        cancellationPolicyType
      }
    }
  }
}
```

### Location Update (existing mutation)

```graphql
mutation UpdateLocation($input: UpdateLocationInput\!) {
  updateLocation(input: $input) {
    location {
      id
      name
      timezone
    }
  }
}
```

### Listing Configuration Update (existing mutation)

```graphql
mutation UpdateProduct($input: UpdateProductInput\!) {
  updateProduct(input: $input) {
    product {
      id
      listingMetadata {
        published
        marketplaceVisible
      }
      pricingOptions {
        id
        billingMode
      }
    }
  }
}
```

## Product Readiness Signal

Hosts should not see Product terminology, but the UI must know when listing configuration becomes available.
The readiness signal is keyed by `locationId` and returns full linked product payload when available.

```graphql
subscription HostListingProductReady($locationId: String\!) {
  listingProductReady(locationId: $locationId) {
    locationId
    product {
      id
      inactive
      listingMetadata {
        title
        published
        marketplaceVisible
      }
      pricingOptions {
        id
        billingMode
        bookingCadence
      }
    }
  }
}
```

If the runtime host schema does not yet expose `listingProductReady`, frontend fallback is polling existing
`location(id) { products { id } }` until the first linked product is available.
