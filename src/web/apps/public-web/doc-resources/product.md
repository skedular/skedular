# Products

## Overview

Products define what customers can purchase in **Skedular Spaces**.

A product represents a commercial offering rather than a physical resource. It describes what is being sold, how it is presented to customers, how it is priced, and the rules that apply when it is purchased.

Resources provide the physical inventory, while products provide the customer-facing catalogue.

A single product can contain multiple offers, allowing customers to purchase the same product using different durations and pricing models without requiring duplicate products.

---

# Availability

Products are currently available in:

- **Skedular Spaces**

Products are not used directly in:

- Skedular Teams
- Skedular Host

In Skedular Host, products are created automatically behind the scenes and are managed by the platform rather than directly by the host.

---

# Why Products Exist

Products separate the commercial side of the platform from the physical resources.

Instead of selling individual desks or meeting rooms directly, organizations sell products.

The booking engine then dynamically allocates suitable resources using Product Tags.

This design allows resources to change over time without requiring products to be updated.

---

# How Products Work

A product defines:

- What customers are purchasing.
- Which resources can fulfil the booking.
- Customer-facing information.
- Available offers.
- Pricing.
- Payment rules.
- Cancellation policies.
- Booking rules.

When a customer purchases a product, Skedular automatically allocates suitable resources that match the Product Tags assigned to the product.

---

# Product Information

Every product contains customer-facing information including:

- Title
- Subtitle
- Cover image
- Feature images
- Included features
- Product type
- Currency
- Product Tags
- Amenities

This information is displayed when customers browse your marketplace.

---

# Product Tags

Products use one or more Product Tags to determine which resources are available for booking.

Products never reference individual resources directly.

Instead, they dynamically allocate any available resource that:

- Has one of the required Product Tags.
- Is available during the requested booking period.
- Meets the booking rules.

This allows products to automatically include newly created resources without additional configuration.

---

# Amenities

Products can advertise amenities included with the booking.

Examples include:

- High-speed Wi-Fi
- Shower facilities
- Storage
- Kitchen access
- Air conditioning
- EV charging

Amenities help customers compare products before making a purchase.

---

# Product Types

Products can reserve resources in different ways.

## Resource Product

A resource product books the required number of matching resources.

Examples include:

- Hot Desk
- Dedicated Desk
- Meeting Room
- Parking Space

The booking engine automatically allocates suitable resources.

---

## Event Product

An event product reserves every matching resource.

This is useful when the entire venue needs to be booked.

Examples include:

- Private venue hire
- Corporate events
- Conferences
- Workshops

Once booked, every matching resource becomes unavailable during the booking period.

---

# Offers

Each product can contain multiple offers.

Offers define the different ways customers can purchase the same product.

Examples include:

- Daily
- Weekly
- Fortnightly
- Monthly
- Two Months
- Four Months
- Five Months
- Six Months
- Yearly

Rather than creating separate products for each duration, organizations simply add multiple offers to the same product.

---

# Pricing

Each offer has its own pricing.

Examples include:

| Offer   | Price |
| ------- | ----: |
| Daily   |   $40 |
| Weekly  |  $180 |
| Monthly |  $600 |

Each offer can have completely independent pricing.

---

# Booking Rules

Each offer controls how bookings behave.

Booking rules include:

- Number of resources to reserve.
- Minimum booking duration.
- Maximum booking duration.

Different offers within the same product can use different booking rules.

---

# Payments

Each offer manages its own payment configuration.

Typical settings include:

- Accepted payment methods.
- Billing mode.
- Tax inclusive or exclusive pricing.
- Subscription auto-renewal.

This allows organizations to support different commercial models for the same product.

---

# Cancellation Policies

Every offer has its own cancellation policy.

Supported policy types include:

## No Refund

No refunds are available after purchase.

---

## Full Refund Before Cut-off

Customers receive a full refund if they cancel before the configured cut-off period.

---

## Tiered Refund

Refund amounts vary depending on how close the cancellation is to the booking.

Each offer can use whichever policy best suits the business.

---

# Product Activation

Products are created in a **draft** state.

A draft product is **not visible to customers** and cannot be purchased.

This allows administrators to:

- Complete the product details.
- Upload images.
- Configure Product Tags.
- Create offers.
- Review pricing.
- Configure payment methods.
- Configure cancellation policies.
- Test the product configuration.

Only when the administrator is satisfied with the product should it be activated.

Once activated, the product immediately becomes available in the marketplace and customers can begin making bookings.

If a product remains inactive, it is hidden from customers and cannot be purchased.

---

# Product Versioning

Skedular automatically versions products whenever pricing changes.

This protects existing bookings while allowing organizations to update their commercial offerings.

When the price of an offer changes:

- A new version of the offer is created.
- Future purchases use the new pricing.
- Existing bookings continue using the pricing that was active when they were purchased.

This ensures pricing changes never affect confirmed bookings.

---

# Existing Bookings

Changing product pricing does **not** modify existing bookings.

For example:

A customer purchases a Daily Desk Pass for **$40**.

The following week, the organization updates the price to **$45**.

The existing booking remains at **$40**.

Only new purchases use the updated price.

---

# Existing Subscriptions

Recurring subscriptions also continue using the pricing that was active when they were created.

For example:

A customer purchases a Monthly Desk Membership.

Halfway through the month, the organization increases the monthly price.

The customer's current subscription continues at the original price until the current billing period ends.

When the subscription renews, the new pricing is automatically applied.

This ensures that pricing changes never interrupt existing customer bookings or subscriptions.

---

# Managing Products

Organization owners and administrators can:

- Create products.
- Edit products.
- Activate or deactivate products.
- Delete products.
- Configure Product Tags.
- Manage offers.
- Configure pricing.
- Configure payment settings.
- Configure cancellation policies.
- Update customer-facing information.

---

# Best Practices

For the best experience:

- Keep products in draft until they are fully configured.
- Use multiple offers instead of creating duplicate products.
- Apply Product Tags consistently across resources.
- Write clear customer-facing titles and descriptions.
- Review pricing regularly.
- Configure appropriate cancellation policies.
- Activate products only when they are ready for customers.

---

# Things to Know

- Products are available only in **Skedular Spaces**.
- Products represent commercial offerings, not physical resources.
- Products dynamically allocate resources using Product Tags.
- Products are created as drafts.
- Draft products are not visible to customers.
- Activated products become available immediately.
- A product can contain multiple offers.
- Each offer has its own pricing, payment settings, and cancellation policy.
- Pricing updates automatically create a new product version.
- Existing bookings keep their original pricing.
- Existing subscriptions continue using their current pricing until renewal.
- New bookings and renewed subscriptions automatically use the latest pricing.

---

# Example

A coworking operator creates a product called **Premium Desk**.

The product includes:

- Premium desk access.
- High-speed Wi-Fi.
- Dual monitors.
- Complimentary coffee.

Initially, the product is saved as a draft while the administrator configures pricing and cancellation policies.

Once everything has been reviewed, the product is activated and becomes available for customers to purchase.

Six months later, the operator increases the monthly membership price.

Existing members continue paying the original price until their current billing period ends.

All new customers, and any subscriptions that renew after the pricing change, automatically use the new version of the pricing.

---

# Related Concepts

- Product Tags
- Offers
- Resources
- Bookings
- Organizations
- Locations
- Payments
- Subscriptions
- Cancellation Policies
