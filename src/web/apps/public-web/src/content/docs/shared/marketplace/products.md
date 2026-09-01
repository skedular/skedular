---
id: shared-products
title: "Products"
description: "Create customer-facing offers that combine eligible Resources with pricing, booking rules, and listing details."
product: shared
category: marketplace
slug: products
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/marketplace.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - shared-resources
  - shared-bookings
  - shared-subscriptions
updatedAt: 2026-08-07
---

<div class="documentation-concept-support"><strong>Managed in</strong><span>✅ Skedular Spaces</span><span>⚪ Skedular Teams</span><span>⚪ Skedular Host</span></div>

## Overview

A Product is the commercial offer that a Skedular Spaces operator presents to customers. It combines customer-facing listing details, Resource eligibility rules, pricing options, booking rules, payment methods, and cancellation terms. Customers choose a Product, and Skedular uses its eligibility rules to determine which Resources can fulfil the resulting Booking.

Products belong to an Organization, not to a single Location. Product eligibility is organization-scoped through Booking Groups, so the matching Resources can be maintained across the operator's Locations.

<aside class="documentation-callout" aria-label="Product core rule"><strong>Core rule</strong><p>A Product defines the commercial offer. Eligible Resources fulfil the Bookings created from that offer.</p></aside>

## Products and Resources

<div class="documentation-concept-grid"><div><strong>🪑 Resource</strong><small>The actual bookable entity, such as a desk, room, parking space, or equipment.</small></div><div><strong>🛒 Product</strong><small>The customer-facing offer that defines how eligible Resources can be accessed or booked.</small></div><div><strong>📅 Booking</strong><small>The reservation created when a customer purchases or books through a Product.</small></div></div>

Customers choose Products, while Skedular allocates or books eligible Resources according to the Product configuration.

## Product relationships

<div class="documentation-organization-context" aria-label="Product relationships"><div class="documentation-context-root">🏢 <a href="/docs/shared/core-concepts/organizations">Organization</a></div><div class="documentation-context-tree"><div><span>🛒 Product</span><div class="documentation-context-child"><span>🏷 Booking Groups</span><span>💰 Prices</span><span>📝 Listing details</span><span>📅 Booking rules</span></div></div><div><span>🏷 Booking Groups</span><div class="documentation-context-child"><a href="/docs/shared/core-concepts/resources">🪑 Eligible Resources</a><div class="documentation-context-child"><a href="/docs/shared/core-concepts/bookings">📅 Bookings</a></div></div></div><a href="/docs/shared/marketplace/customers">👤 Customers</a><a href="/docs/shared/marketplace/subscriptions">🔁 Subscriptions</a></div></div>

Booking Groups are Organization-level, marketplace-specific tags. A Product must have at least one Booking Group, and Resources with matching Booking Groups are eligible to fulfil that Product. Normal Resource Tags are general-purpose Organization labels. Changing a normal Resource Tag does not affect Product eligibility; changing a Booking Group assignment can change which Resources can fulfil a Product.

## Product details

Product versions hold the customer-facing listing metadata, feature images, currency, Booking Groups, amenities, and pricing options. Listing metadata includes the title, subtitle, and description used in the marketplace. Feature images support the customer-facing presentation. Amenities are organization tags attached to the Product version and are separate from Booking Groups.

## Pricing

A Product can contain multiple pricing options. Each option defines a price, purchase cadence, accepted payment methods, billing mode, duration limits, cancellation policy, and whether subscription auto-renewal is supported.

Operators choose a purchase term of Daily, Weekly, Fortnightly, Monthly, TwoMonths, Quarterly, FourMonths, FiveMonths, SixMonths, or Yearly. Customers choose the booking start and end date/time; any duration within the configured minimum and maximum is allowed, with no cadence-based increment.

Products define what is sold and under which commercial terms. [Commerce](/docs/shared/commerce) documents how resulting charges are billed, paid, invoiced, and settled.

## Booking and subscription behavior

A purchase term repeats only when auto-renewal is enabled. Disabled auto-renewal makes a Daily offer a single one-day offer; enabled auto-renewal renews it daily. The same applies to longer terms. Organization billing cycles can split longer terms for invoice and resource-booking slices without replacing the purchase term.

A Product's pricing option determines whether the purchase is one-time or recurring; see [Subscriptions](/docs/shared/marketplace/subscriptions) for the recurring lifecycle. Event Products are a separate Product type for fixed-time event bookings and cannot use subscription auto-renewal.

## Cancellation policies

Each pricing option uses one cancellation policy: no cancellation, full refund before a cutoff, or tiered refunds. Tiered policies contain ordered time thresholds and refund percentages. The policy is part of the Product pricing configuration, while the cancellation and refund workflow is handled by Booking and Commerce.

## Changes after purchase

Changing an eligible marketplace Booking does not create a new purchase or replace its commercial terms. The Booking keeps its original Product version, Price, quantity, payment state, invoice and refund history, and cancellation terms. A customer or authorized operator can only use a new date, time, or Resource that the purchased Product can still fulfill; selecting a replacement Resource does not permit switching to a different Product or Price.

Whether a Booking can be changed depends on its payment state, start time, permissions, and current availability. The Product cancellation cutoff applies to cancellation and refunds, not to an otherwise eligible Booking change. See [Bookings](/docs/shared/core-concepts/bookings) for the change flow and [Subscriptions](/docs/shared/marketplace/subscriptions) for recurring Booking occurrences.

## Billing and payment configuration

Pricing options require accepted payment methods and a billing mode. Supported billing modes include upfront and in arrears. Payment collection, invoices, payouts, Stripe, Xero, and bank-account workflows belong to [Commerce](/docs/shared/commerce) and are not duplicated here.

## Product lifecycle

<div class="documentation-concept-workflow"><span><b>1</b>Create the Product</span><span><b>2</b>Add listing details and Booking Groups</span><span><b>3</b>Add pricing and commercial rules</span><span><b>4</b>Activate the Product</span><span><b>5</b>Customers book or subscribe</span></div>

Products are versioned so changes do not silently rewrite an offer already used by customers. Updating listing metadata, images, tags, pricing, or other Product version fields creates a new Product version. New customers use the active version. Existing Bookings and active Subscriptions retain the commercial version used when they were created; renewal re-evaluates the current Product pricing and version for the next cycle. Activation and deactivation are operator controls; inactive Products remain available to administrators but are not offered to customers through the active marketplace experience.

## Managing Products in Skedular Spaces

1. Open the organization's Products area in Skedular Spaces.
2. Create or select a Product and add its customer-facing listing details.
3. Add at least one Booking Group so eligible Resources can be found.
4. Configure one or more pricing options, payment methods, billing mode, and cancellation policy.
5. Activate the Product when its configuration is complete.

## Best practices

- Use clear customer-facing titles and descriptions.
- Add Booking Groups that match the Resources customers should be able to book.
- Keep the purchase term, available days, and minimum/maximum booking duration aligned with the service you provide.
- Test cancellation terms and payment methods before activation.
- Review the active Product version after changing tags, images, or pricing.

## Things to Know

- Products are managed directly in Skedular Spaces.
- Products belong to an Organization and are not owned by a single Location.
- Products define the commercial offer; they do not directly own Resources.
- Booking Groups determine which Resources are eligible to fulfil a Product.
- Resources are the entities ultimately reserved by Bookings.
- A Product can have multiple pricing options.
- Pricing configuration determines single Booking or Subscription behavior.
- Cancellation policies are configured per pricing option.
- Product changes create new versions when versioned fields are updated.

## Frequently Asked Questions

### What is the difference between a Product and a Resource?

A Product is the commercial offer. A Resource is the desk, room, parking space, or other entity that a Booking ultimately reserves.

### How does a Product know which Resources can be booked?

Booking Groups define eligibility. Resources with matching Booking Groups can fulfil Bookings created from that Product.

### Can a Product have multiple Prices?

Yes. A Product can have multiple pricing options, each with its own cadence, booking rules, payment methods, billing mode, and cancellation policy.

### When does a Product create a Booking instead of a Subscription?

Credit-entitlement offers use `NOT_SET` for purchase cadence and are governed by credit quantity, validity, available days, and booking duration limits. They do not renew as subscriptions.

### Can I edit a Product after activation?

Yes. Changes to versioned Product fields create a new Product version. The active version is the one offered to new customers.

### Does Skedular Host use Products?

Host users do not manage Products directly. Host manages its place-first commercial experience through the Host workflow.

### Do Products handle payments?

Products define accepted payment methods and commercial terms. Commerce handles payment collection, billing, invoicing, payouts, and settlement.

## Related Documentation

- [Marketplace](/docs/shared/marketplace)
- [Resources](/docs/shared/core-concepts/resources)
- [Bookings](/docs/shared/core-concepts/bookings)
- [Customers](/docs/shared/marketplace/customers)
- [Subscriptions](/docs/shared/marketplace/subscriptions)
- [Commerce](/docs/shared/commerce)
