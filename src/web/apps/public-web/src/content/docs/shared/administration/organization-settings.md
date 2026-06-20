---
id: shared-organization-settings
title: "Organization Settings"
description: "Manage the settings that control how your Organization is configured and presented across Skedular."
product: shared
category: administration
slug: organization-settings
articleKind: reference
publicationState: published
evidenceRefs:
  - doc-resources/organization.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds:
  - shared-organizations
  - shared-locations
  - shared-availability
updatedAt: 2026-07-15
---

## Overview

Organization Settings contain the configuration that Organization owners and administrators can manage for the Organization as a whole. These settings are managed centrally so the current Organization's shared identity, contact details, and operating context stay consistent across its workflows. Product-specific and specialized configuration lives in dedicated Administration, Marketplace, or Commerce guides.

<div class="documentation-concept-support"><strong>Available in</strong><span>✅ Skedular Teams</span><span>✅ Skedular Spaces</span><span>✅ Skedular Host</span></div>

<aside class="documentation-callout" aria-label="Administration orientation"><strong>Looking for the Organization concept?</strong><p>See <a href="/docs/shared/core-concepts/organizations">Organizations</a> to understand ownership, membership, and structure. This guide focuses on the settings administrators can configure for an existing Organization.</p></aside>

## Settings Available

### Organization identity

Administrators can update the Organization name and the identity details shown across the product. These values describe the current Organization and do not change the Locations, Resources, or Bookings it owns.

### Images and presentation

Administrators can manage the Organization logo and feature images used in customer-facing and marketplace presentation. These assets describe the Organization's presentation; they do not change Resource Availability or Booking rules.

### Website and contact details

The settings workflow supports Organization contact information such as a website, contact email, and contact phone. These details provide an operational point of contact for the current Organization.

### Industry and customer-facing details

Skedular Spaces organization administration includes industry classifications and a customer-facing terms and conditions URL. These settings support how the Organization is presented to customers and are separate from product pricing and payment configuration.

### Refund notification recipients

Authorized administrators can manage the Organization's refund-notification email recipients. This controls who receives refund-related operational notifications; it does not decide whether a Booking is refundable.

### Related configuration

Some Organization-level configuration has its own workflow and should be managed in its canonical guide:

- [Locations](/docs/shared/core-concepts/locations) own address, time zone, opening-hour, and Resource context.
- [Commerce](/docs/shared/commerce) covers payment methods, billing, payouts, and accounting connections.

## Who Can Manage Organization Settings

Only active Organization owners and administrators with a valid organization session can change Organization Settings. Members can view the Organization but cannot modify these settings.

## Product Differences

<div class="documentation-concept-grid"><div><strong>🧑‍💼 Skedular Teams</strong><small>Organization Settings manage the identity and operational contact details for a private workplace Organization.</small></div><div><strong>🛒 Skedular Spaces</strong><small>Organization Settings also manage customer-facing presentation, contact, industry, and terms details for marketplace Organizations.</small></div><div><strong>🏠 Skedular Host</strong><small>Organization Settings manage identity, contact, and presentation details. Location and place configuration is documented separately.</small></div></div>

## Managing Organization Settings

<div class="documentation-concept-workflow"><span><b>1</b>Open Organization Administration</span><span><b>2</b>Open Organization Settings</span><span><b>3</b>Update a setting</span></div>

Profile fields are saved through inline, debounced controls. Image uploads apply through their upload controls. Changes apply to the current Organization; there is no single universal Save step.

## Configuration Scope

Organization Settings apply only to the Organization currently being managed. A user who belongs to multiple Organizations must select the correct Organization before making a change. Updating one Organization does not automatically update any other Organization.

## Best Practices

- Confirm the current Organization before editing shared settings.
- Keep Organization identity and contact details accurate.
- Use clear, current images for customer-facing presentation.
- Review notification recipients when operational responsibilities change.

## Frequently Asked Questions

### What are Organization Settings?

They are the Organization-level configuration controls used to manage shared identity, contact, presentation, and related operational details.

### Who can change Organization Settings?

Active Organization owners and administrators with a valid organization session can change Organization Settings.

### Do these settings affect every Location?

Organization-level settings apply to the current Organization. Location-specific settings, such as address, time zone, and opening hours, remain managed by each Location.

### Do Organization Settings affect other Organizations?

No. A change applies only to the Organization currently selected.

### Where are payment and accounting settings managed?

Payment, billing, payout, and accounting configuration belongs in the relevant Commerce or Marketplace workflow rather than being duplicated here.

### Does changing Organization Settings change a Location?

No. Location address, time zone, opening hours, and Resource configuration remain managed on the relevant Location.

## Related Documentation

- [Administration](/docs/shared/administration)
- [Organizations](/docs/shared/core-concepts/organizations)
- [Locations](/docs/shared/core-concepts/locations)
- [Commerce](/docs/shared/commerce)
