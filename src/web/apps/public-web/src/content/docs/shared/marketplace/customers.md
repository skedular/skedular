---
id: shared-customers
title: "Customers"
description: "Understand the customers connected to Products, Bookings, and Subscriptions in Skedular Spaces."
product: shared
category: marketplace
slug: customers
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/customer.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - shared-products
  - shared-bookings
  - shared-subscriptions
updatedAt: 2026-07-17
---

<div class="documentation-concept-support"><strong>Available in</strong><span>✅ Skedular Spaces</span></div>

## Overview

A Customer is a registered Skedular user who signs in and purchases a Product from a Skedular Spaces operator. The Customer is then associated with the Booking or Subscription created by that purchase.

## Customer and Organization membership

Having a Skedular account does not make someone a member of the Organization they purchase from. Organization membership is a separate relationship for people who belong to and manage the Organization. The same person can be both a Customer and an Organization member, but one relationship does not create the other. See [Users](/docs/shared/core-concepts/users) for the account and platform identity model.

## How someone becomes a Customer

1. The person creates a Skedular account and signs in.
2. They choose and purchase a Product from a Spaces operator.
3. They become the Customer associated with the resulting Booking or Subscription.
4. They remain separate from the operator's Organization membership.

## Customer relationships

<div class="documentation-organization-context" aria-label="Customer journey"><div class="documentation-context-root">👤 Registered Skedular account<div class="documentation-context-child"><span>🛒 Purchases an Organization's Product</span><div class="documentation-context-child"><a href="/docs/shared/core-concepts/bookings">📅 Booking</a><a href="/docs/shared/marketplace/subscriptions">🔁 Subscription</a></div></div></div></div>

The Organization owns the Product and manages the workspace. Purchasing it gives the Customer access to the associated commercial workflow, not membership in the Organization.

## Customer information in Spaces

Spaces does not currently have a dedicated Customer management screen. Operators encounter customer information through relevant Booking and Subscription workflows and can see only the identity and details those workflows expose. Customers manage their own personal account information; Organization membership is managed separately.

## Customer activity and Commerce

Payments, invoices, refunds, payouts, and other financial activity belong to the Commerce workflows associated with a Booking or Subscription, rather than to customer management itself. See [Commerce](/docs/shared/commerce) for those workflows.

## Frequently Asked Questions

### What is a Customer in Skedular?

A Customer is a registered Skedular user who purchases a Product from a Spaces operator.

### Does a Customer need a Skedular account?

Yes. The person must create an account and sign in before purchasing a Product.

### Is a Customer an Organization member?

No. Purchasing a Product does not add someone to the operator's Organization. Membership is a separate relationship.

### Can the same Customer purchase from multiple Organizations?

Yes. A registered customer can purchase Products from different Organizations, with each purchase creating its own Booking or Subscription context.

### Where can operators see customer information?

Operators see the customer identity and other details exposed by the relevant Booking and Subscription workflows. Spaces does not currently provide a separate Customer management screen.

## Related Documentation

- [Users](/docs/shared/core-concepts/users)
- [Marketplace](/docs/shared/marketplace)
- [Products](/docs/shared/marketplace/products)
- [Bookings](/docs/shared/core-concepts/bookings)
- [Subscriptions](/docs/shared/marketplace/subscriptions)
- [Commerce](/docs/shared/commerce)
