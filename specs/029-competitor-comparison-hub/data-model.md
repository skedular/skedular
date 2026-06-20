# Data Model: Skedular Competitor Comparison Hub

## Entity: Comparison Product

Represents a product shown in comparison data. Skedular is modeled alongside competitors so feature matrix rows use one support-state model.

Fields:

- `id`: stable unique identifier, such as `skedular` or `skedda`.
- `name`: public display name.
- `slug`: URL-safe slug.
- `productKind`: `skedular` or `competitor`.
- `category`: workplace management, coworking management, hybrid workplace, workplace operations, or marketplace/workspace network.
- `publicationStatus`: draft, reviewed, blocked, or published.
- `reviewStatus`: pending, approved, rewritten, blocked, or not-required.
- `summary`: short neutral product summary.
- `bestFor`: buyer or user profile.
- `strengths`: reviewed list of strengths.
- `limitations`: reviewed list of limitations.
- `pricingNotes`: qualitative or reviewed pricing comparison notes.
- `integrationNotes`: reviewed integration comparison notes.

Validation:

- `id` and `slug` must be unique.
- Competitor records cannot be `published` unless competitor claims have evidence notes or approved review status.
- Skedular records cannot claim support for a feature without current repo evidence.

## Entity: Competitor Claim

Represents a publishable statement about a competitor.

Fields:

- `id`: stable unique identifier.
- `competitorId`: related competitor.
- `claimType`: capability, strength, limitation, pricing, integration, best-for, or FAQ.
- `claimText`: concise claim used by generated pages.
- `evidenceNote`: source note, review rationale, or evidence reference.
- `reviewStatus`: pending, approved, rewritten, blocked.
- `publishedPageIds`: generated pages where the claim appears.

Validation:

- Published claims require `evidenceNote` or `reviewStatus = approved`.
- Blocked claims must not render.
- Pending claims may remain in inventory but render as unknown or unpublished content only.

## Entity: Skedular Capability Evidence

Represents a supportable Skedular capability used in the matrix or page copy.

Fields:

- `id`: stable unique identifier.
- `featureId`: related normalized feature when applicable.
- `capabilityName`: public capability label.
- `category`: normalized feature category.
- `supportState`: supported, partially-supported, not-supported, unknown, or not-applicable.
- `sourceRefs`: active spec, help doc, public-web data, route, pricing data, or implemented surface reference.
- `sourceFreshness`: current, needs-review, outdated, or blocked.
- `reviewStatus`: pending, approved, rewritten, blocked.
- `notes`: optional scope note for matrix display.

Validation:

- Published Skedular support states require at least one current source reference.
- Outdated-only evidence cannot mark a feature as supported.
- Unknown is preferred when evidence is incomplete.

## Entity: Feature Category

Represents a grouped section in the normalized matrix.

Fields:

- `id`: stable unique identifier.
- `name`: public category name.
- `description`: short explanation.
- `displayOrder`: integer ordering.
- `features`: ordered normalized feature references.

Required records:

- Workspace Management
- Coworking Management
- Marketplace
- Payments
- Integrations
- Administration
- Analytics
- Developer

Validation:

- Required categories must exist exactly once.
- Display order must be unique.

## Entity: Normalized Feature

Represents a stable feature row shared across all comparison pages.

Fields:

- `id`: stable unique identifier.
- `categoryId`: related feature category.
- `name`: public feature label.
- `description`: optional plain-language definition.
- `displayOrder`: integer order within category.
- `requiredBySpec`: boolean.

Validation:

- Required features from the spec must exist exactly once.
- Feature names must be consistent across hub, individual pages, supporting pages, and tests.

Required feature groups:

- Workspace Management: Desk Booking, Room Booking, Parking Booking, Event Booking, Custom Resources, Floor Plans, Interactive Maps, Booking Rules, Resource Permissions.
- Coworking Management: Member Management, Membership Plans, Recurring Memberships, Billing, Invoicing, Tax Handling, Subscription Management, Community Features.
- Marketplace: Public Listings, Workspace Discovery, Marketplace Inventory, Host Onboarding, Public Booking Pages.
- Payments: Stripe, Stripe Connect, Xero, Manual Invoicing, Weekly Billing, Fortnightly Billing, Monthly Billing.
- Integrations: Slack, Teams, SSO, WorkOS, Calendar Integrations, Access Control.
- Administration: Multi Location, Multi Team, Custom Branding, Custom Domains, White Label.
- Analytics: Occupancy Reporting, Utilization Reporting, Revenue Reporting, Booking Analytics.
- Developer: API, Webhooks.

## Entity: Feature Support

Represents a product's support state for one normalized feature.

Fields:

- `productId`: related comparison product.
- `featureId`: related normalized feature.
- `state`: supported, partially-supported, not-supported, unknown, or not-applicable.
- `note`: optional scope explanation.
- `evidenceRefs`: source/evidence references.
- `reviewStatus`: pending, approved, rewritten, blocked, or not-required.

Validation:

- A published comparison page must not render blocked support entries.
- Skedular supported/partially-supported states require current evidence references.
- Competitor supported/partially-supported/not-supported states require evidence or approved review status.
- Unknown can render when a support state is intentionally not asserted.

## Entity: Comparison Page Target

Represents an individual competitor comparison page.

Fields:

- `id`: stable unique identifier.
- `slug`: URL slug, such as `skedular-vs-skedda`.
- `path`: canonical path under `/compare`.
- `pageType`: `competitor-comparison`.
- `competitorId`: primary competitor.
- `title`: unique metadata title.
- `description`: unique metadata description.
- `overview`: page overview text.
- `pricingComparison`: generated from pricing notes and shared data.
- `integrationComparison`: generated from integration notes and matrix data.
- `bestFor`: generated best-fit summary.
- `limitations`: reviewed competitor limitations.
- `whySkedular`: Skedular positioning bullets.
- `faqIds`: visible FAQs for page and FAQ schema.
- `primaryCtaId`: CTA reference.
- `relatedPageIds`: hub, alternatives, and related comparison links.
- `publicationStatus`: draft, reviewed, blocked, or published.

Validation:

- Required competitor comparison paths must exist.
- Every published comparison page must include Overview, Feature Matrix, Pricing Comparison, Integration Comparison, Best For, Limitations, Why Teams Choose Skedular, FAQ, and CTA sections.
- Page target data must generate visible content and structured-data inputs from the same source.

## Entity: Supporting Page Target

Represents a best-software or alternatives page under `/compare`.

Fields:

- `id`: stable unique identifier.
- `slug`: URL slug.
- `path`: canonical path under `/compare`.
- `pageType`: best-software or alternatives.
- `focusCategoryIds`: feature categories emphasized.
- `includedProductIds`: products included in the page.
- `title`: unique metadata title.
- `description`: unique metadata description.
- `intro`: page intro.
- `selectionCriteria`: criteria derived from feature matrix and buyer intent.
- `faqIds`: visible FAQs for page and FAQ schema.
- `relatedPageIds`: hub, comparison, product, pricing, and CTA links.
- `publicationStatus`: draft, reviewed, blocked, or published.

Validation:

- Required supporting paths must exist under `/compare`.
- Supporting pages must use the same competitor dataset and feature matrix as individual comparison pages.
- Empty supporting pages must block publication.

## Entity: FAQ Entry

Represents a visible FAQ and optional FAQ schema item.

Fields:

- `id`: stable unique identifier.
- `question`: visible question.
- `answer`: visible answer.
- `relatedPageIds`: pages where the FAQ appears.
- `claimRefs`: related Skedular or competitor claims.
- `schemaEligible`: boolean.
- `reviewStatus`: pending, approved, rewritten, blocked.

Validation:

- FAQ schema is emitted only for visible, approved, schema-eligible FAQs.
- FAQ answers must not contain unsupported competitor or Skedular claims.

## Entity: Structured Data Definition

Represents JSON-LD inputs for a generated page.

Fields:

- `pageId`: related page target.
- `types`: SoftwareApplication, FAQPage, BreadcrumbList, ItemList, or WebPage.
- `graph`: derived graph values.
- `sourceRefs`: page target, FAQ, breadcrumb, and visible content references.

Validation:

- Structured data must match visible page content.
- FAQPage graph cannot include hidden FAQs.
- Breadcrumb paths must resolve to generated pages or existing public routes.

## Entity: Content Inventory Entry

Represents reviewable generated content coverage.

Fields:

- `id`: stable unique identifier.
- `pageId`: related page.
- `sourceDataRefs`: competitor, feature, FAQ, evidence, and page target references.
- `metadataStatus`: drafted, reviewed, approved, or published.
- `contentStatus`: drafted, reviewed, approved, or published.
- `reviewNotes`: optional reviewer notes.
- `validationStatus`: pending, passing, blocked.

Validation:

- Every generated page must have a content inventory entry.
- Publication readiness requires all required pages to have passing validation.

## State Transitions

Claim lifecycle:

```text
seeded -> evidence-added -> reviewed -> approved -> published
                       |          |
                       v          v
                    rewritten   blocked
```

Page lifecycle:

```text
draft -> generated -> reviewed -> validation-passing -> published
                |          |
                v          v
             blocked    rewritten
```

Comparison section publication:

```text
legacy-removed -> dataset-complete -> pages-generated -> validation-passing -> published
```

Publication cannot advance past `pages-generated` unless `/compare`, all required individual comparison pages, and all required supporting pages exist and validate together.
