# Quickstart: Unified Host Listing Validation

**Feature**: 032-unified-host-listing  
**Date**: 2026-07-07

## Prerequisites

1. Backend services running (Aspire or Docker Compose)
2. Test organization with Skedular Host type
3. Test user with host permissions
4. GraphQL client (GraphiQL, curl, or test code)

## Validation Scenarios

### Scenario 1: Create New Listing (Unified Form)

**Objective**: Verify a new listing can be created via the frontend-led unified flow.

**Setup**:

```bash
# Get organization ID
gh api graphql -f query='query { activeOrganization { id } }' | jq '.data.activeOrganization.id'
```

**Test Steps**:

1. **Create location** using the existing Location mutation:

```graphql
mutation {
  createLocation(
    input: {
      name: "Test Host Listing"
      type: "Private"
      timezone: "America/New_York"
      openingHours: { days: [{ day: Monday, open: true, from: "09:00", to: "17:00" }] }
      featureImages: []
    }
  ) {
    clientMutationId
    location {
      id
      name
    }
  }
}
```

2. **Resolve the linked Product using existing fields/queries available to the host app**

- If the hidden product draft already exists, fetch it and continue.
- If it does not, use the existing frontend product creation path without introducing a new backend API.

3. **Update product configuration** using the existing Product mutation:

```graphql
mutation {
  updateProduct(
    input: {
      productId: "<product-id>"
      pricingOptions: [{ billingMode: PerBooking, bookingCadence: Default, cancellationPolicyType: Flexible, isTaxInclusive: false }]
      listingMetadata: { published: true, marketplaceVisible: true }
    }
  ) {
    clientMutationId
    product {
      id
      pricingOptions {
        billingMode
      }
      listingMetadata {
        published
      }
    }
  }
}
```

**Expected Result**:

- Location created with specified fields
- Hidden Resource/Product linkage remains backend-owned
- Listing/product configuration is persisted through existing Product APIs coordinated by the frontend
- The host user never needs to navigate to a separate Product management screen

---

### Scenario 2: View Unified Listing (List Page)

**Objective**: Verify listing cards show combined location + product info.

**Test Steps**:

1. **Query locations with products using the existing host query shape**:

```graphql
query {
  locations {
    nodes {
      id
      name
      physicalAddress {
        city
      }
      products {
        id
        pricingOptions {
          billingMode
          cancellationPolicyType
        }
        inactive
      }
    }
  }
}
```

**Expected Result**:

- Each location includes `products` array with pricing info
- Card displays: name, address, pricing mode, availability status

---

### Scenario 3: Edit Existing Listing (Unified Form)

**Objective**: Verify editing coordinates existing Location and Product updates from one host screen.

**Setup**: Use ID from Scenario 1 result.

**Test Steps**:

1. **Update location fields**:

```graphql
mutation {
  updateLocation(input: { locationId: "<location-id>", name: "Updated Listing Name" }) {
    location {
      id
      name
    }
  }
}
```

2. **Update product configuration**:

```graphql
mutation {
  updateProduct(
    input: {
      productId: "<product-id>"
      pricingOptions: [{ billingMode: Subscription, bookingCadence: Monthly, cancellationPolicyType: Moderate, isTaxInclusive: true }]
      listingMetadata: { published: false, marketplaceVisible: true }
    }
  ) {
    product {
      id
      pricingOptions {
        billingMode
        cancellationPolicyType
      }
      listingMetadata {
        published
      }
    }
  }
}
```

**Expected Result**:

- Location fields updated correctly
- Product configuration updated (billing mode changed to Subscription)

---

### Scenario 4: Navigation - No Product Page

**Objective**: Verify Products navigation is removed from Skedular Host.

**Test Steps**:

1. **Check sidebar for Products menu item**
   - Navigate to `/locations`
   - Inspect sidebar DOM
   - Confirm no "Products" link exists

2. **Verify direct URL behavior**

```bash
# Try accessing old product page (should redirect or 404)
curl -I https://host.example.com/products
```

**Expected Result**:

- No Products navigation item in sidebar
- Direct access returns redirect to locations or 404

---

### Scenario 5: Validation Errors

**Objective**: Verify validation errors show for both Location and Product fields.

**Test Steps**:

1. **Submit invalid data through the separate existing mutations coordinated by the frontend**:

```graphql
mutation {
  updateLocation(
    input: {
      locationId: "<location-id>"
      name: "" # Invalid: empty
      openingHours: { days: [{ day: Monday, open: true, from: "25:00", to: "17:00" }] } # Invalid time
    }
  ) {
    location {
      id
    }
  }
}
```

```graphql
mutation {
  updateProduct(
    input: {
      productId: "<product-id>"
      pricingOptions: [
        {
          billingMode: PerBooking
          minDurationMinutes: 300
          maxDurationMinutes: 60 # Invalid: min > max
        }
      ]
    }
  ) {
    product {
      id
    }
  }
}
```

**Expected Result**:

- Validation errors from both existing APIs are surfaced coherently by the unified frontend
- The user does not need to infer whether an error came from the Location or Product domain

---

## Backend Validation

### Temporal Workflow Verification

```csharp
// Verify hidden entities/relationships still come from existing backend behavior
await VerifyWorkflowCompletedAsync("CreateListingWorkflow", locationId);

// Check entities exist
var resource = await context.Resources.FindAsync(locationId);
var productTag = await context.OrganizationTags.FirstOrDefaultAsync(t => t.ProductId == productId);
var product = await context.Products.FindAsync(productId);

resource.Should().NotBeNull();
productTag.Should().NotBeNull();
product.Should().NotBeNull();
```
