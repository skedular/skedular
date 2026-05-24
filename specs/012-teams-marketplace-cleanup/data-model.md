# Data Model: Remove Marketplace from Web App Teams

**Feature**: 012-teams-marketplace-cleanup
**Date**: 2026-05-24

---

## Overview

This feature removes marketplace-related data from the webapp-teams frontend. No
new entities are introduced. No backend schema changes are made. This document
describes the existing entities whose frontend representations are being removed or
trimmed, and the filtering rule applied to the booking list.

---

## Entities Being Removed from the webapp-teams Frontend

### Product Tag

**What it is**: A marketplace-defined label used to classify resources and products
for customer discovery on the public co-working marketplace.

**Current surface in webapp-teams**:

- Displayed as chips on resource cards and in the resource management list
- Selectable via `MultipleChoicesProductTags` autocomplete in resource add/edit/bulk-import forms
- Displayed as chips in the floor plan editor (add/edit floor plan)
- Managed via `EditProductTag` / `DeleteProductTag` options in the more-actions menu
- Stored on the `resource.productTags` connection in GraphQL fragments

**After this feature**: All product tag UI surfaces are removed. The field remains
in the backend schema and data but is never fetched or displayed in webapp-teams.

---

### Marketplace Listing Metadata

**What it is**: An organisation's public marketplace title and subtitle — content
used when the organisation lists itself on the public co-working marketplace
directory.

**Current surface in webapp-teams**:

- Fetched in `organizationAdminSetupSectionQuery` as `marketplaceListingMetadata { title subTitle }`
- Editable inline via the organisation admin setup section
- Used as a title fallback in the organisation admin header label

**After this feature**: Removed from the admin setup section fragment and patch
mutation input. The field remains in the backend schema; webapp-teams simply does
not fetch or display it.

---

### Marketplace Booking

**What it is**: A booking made via the public marketplace flow, linking a customer
to a resource at an external co-working space. Identified in the booking fragment
by a non-null `marketplaceBooking` sub-object.

**Current surface in webapp-teams**:

- Fetched in the booking card fragment (`booking.marketplaceBooking`)
- Used to conditionally show marketplace-specific delete/cancel actions
- Used to branch labels ("Cancel series" vs "Remove series")
- `marketplaceBookingSubscriptions` fetched separately in `my-bookings.tsx` and
  `organization-bookings.tsx` to build a lookup map of active subscriptions

**After this feature**:

- The booking list in webapp-teams renders only private organisation bookings.
  Any booking node where `marketplaceBooking` is non-null is skipped at render time.
- All marketplace booking mutation fragments and subscription lookup logic are
  removed from the components.
- The `marketplaceBooking` field selector may be retained in the fragment only as
  a boolean filter predicate at the list level, then fully removed from the card.

---

### Marketplace Booking Subscription

**What it is**: A recurring marketplace subscription, linking an active cadence
purchase to a set of generated recurring bookings via the marketplace flow.

**Current surface in webapp-teams**:

- Fetched via `marketplaceBookingSubscriptions(first: 100, ...)` in
  `my-bookings.tsx`, `organization.tsx`, and `organization-bookings.tsx`
- Used to populate a `recurringMarketplaceSubscriptionIds` lookup map
- Passed to `MyBookingCard` to determine if a recurring booking's subscription can
  be cancelled via a marketplace mutation
- `marketplaceBookingSubscriptionCancellationModes` fetched for UI mode labelling

**After this feature**: All fetching and usage of marketplace booking subscriptions
is removed. The queries no longer request these fields.

---

## State / Filtering Rules

### Booking list filter (new rule)

| Booking node `marketplaceBooking` value | Rendered in webapp-teams booking list?           |
| --------------------------------------- | ------------------------------------------------ |
| `null` (private booking)                | Yes — displayed with all private booking actions |
| non-null (marketplace booking)          | No — skipped at render; not displayed            |

This filter is applied at the component level in `my-bookings.tsx` when iterating
over booking edges to render `MyBookingCard` components.

---

## Validation Rules Removed

The following validation/schema fields are removed from resource forms:

| Component                    | Field removed             | Validation rule removed  |
| ---------------------------- | ------------------------- | ------------------------ |
| `add-resource-dialog.tsx`    | `productTagIds`           | `array().nullable()`     |
| `edit-resource.tsx`          | `productTagIds`           | `array().nullable()`     |
| `bulk-add-resources-row.tsx` | `productTagIds` (per-row) | Conditional render guard |

---

## No New Migrations or Schema Changes

This feature makes no backend schema changes. No database migrations are needed.
The GraphQL schema is unchanged. Only frontend query fragments and mutation
definitions are modified (removed).
