---
id: spaces-products-pricing
title: "Products and pricing"
description: "Create Products, match eligible Resources, and configure the prices and commercial rules customers see."
product: spaces
category: products-and-marketplace
slug: products-and-pricing
articleKind: guide
publicationState: published
evidenceRefs:
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds: []
updatedAt: 2026-07-16
---

Resources are the physical entities customers can book. A Product is the commercial offering built around eligible Resources, with customer-facing information, one or more Prices, and the rules that apply when it is purchased.

## Understand the Product model

The Spaces model connects the workspace to the customer offering:

`Location → Resources → Booking Groups → Product → Price → Booking or Subscription`

Products do not replace Locations or Resources. They describe what customers can purchase and how matching Resources are allocated. See the shared [Products](/docs/shared/marketplace/products) concept for the canonical definition.

## Connect Products to Resources

Booking Groups are created for the Organization and assigned to Resources. When you edit a Product, choose its Booking Groups from the **Booking Groups** field. A Resource is eligible when it has at least one of the Product's selected Booking Groups and is available for the requested time; selecting multiple Booking Groups broadens the matching Resource pool rather than requiring every tag. The same Resource can be eligible for multiple Products.

Booking Groups are different from ordinary Tags. Ordinary Tags help organize and filter workspace data, while Booking Groups connect Resources to commercial Products.

## Create and describe a Product

Use **Add Product** to create a Product. The Product editor lets you define customer-facing information such as:

- Title and subtitle
- Description and included features
- Feature images
- Product type, currency, and amenities
- Booking Groups used to select eligible Resources

Organization marketplace information belongs in [Marketplace setup](/docs/spaces/products-and-marketplace/marketplace-setup), not in the Product editor.

## Configure Prices and commercial terms

Each Product can have multiple pricing options. Purchase cadence choices are **Daily**, **Weekly**, **Fortnightly**, **Monthly**, **Two months**, **Quarterly**, **Four months**, **Five months**, **Six months**, and **Yearly**.

For each pricing option, configure the price, booking duration limits, the number of Resources to reserve, and whether tax is included. The pricing option also carries its billing mode, accepted payment methods, subscription auto-renewal setting, and cancellation policy with its refund rules. Where applicable, choose **Upfront** or **In arrears** billing.

Use **Available days** on an individual Price to limit it to selected calendar days. **Sunday** through **Saturday** are equal choices, not workweek-only weekdays. Leave the selection empty for **Every day**. This rule is checked against the booking start date before the usual opening-hours, matching-Resource, and conflict checks. Customers can see the selected days and are guided away from dates the Price does not allow.

For a **Weekly** Price, you can set the exact number of days the Customer must choose per week. Leave it empty to preserve unrestricted weekly behavior. This value is not a replacement for Available days: Available days define the permitted pool, while the Customer’s selection creates their fixed weekly schedule. Do not use this weekly field for fortnightly or monthly Prices; those cadences will have their own configuration when supported.

Payment-provider connections, bank accounts, payouts, and accounting setup are documented under [Billing and payments](/docs/spaces/billing-and-payments).

## Understand Bookings and Subscriptions

A pricing option is a day-or-longer offer term. Auto-renewal controls whether that term repeats. Customers choose each booking's start and end date/time, and any duration between the minimum and maximum is allowed. Operators manage the resulting activity in [Bookings](/docs/spaces/bookings) and [Subscriptions](/docs/spaces/bookings/subscriptions).

## Activate a Product

Products start inactive. An inactive Product is not available for customer purchase. Complete its customer-facing details, Booking Groups, and at least one pricing option before using **Activate product**. Activation makes the Product available for customer purchase in the marketplace.

## Change an active Product

When pricing changes, Spaces creates a new Product version for future purchases. New customers use the current Product and Price configuration; existing Bookings retain the terms used when they were purchased. Existing auto-renewing Subscriptions continue under their current terms, including their selected available days, until renewal, when the current matching Price configuration applies.

## Next step

Once the Product and its Prices are ready, continue to [Bookings](/docs/spaces/bookings) to manage the reservations customers create through those Products.

## Credit-based pricing

An entitlement pricing option can grant prepaid booking credits. Configure the credit quantity, unit, validity period, and whether unused credits are refundable on that pricing option.
