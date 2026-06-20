# Contract: Comparison Content Data

## Required Products

The shared dataset must include Skedular and these competitors:

```text
skedda
officernd
nexudus
gable
robin
officely
envoy
kadence
archie
deskbird
```

Rules:

- Each product has a unique `id`, `slug`, and display name.
- Competitor seed facts must be represented as structured data, not page-local prose.
- Publishable competitor claims require an evidence note or explicit approved review status.
- Skedular support states require current repo evidence.

## Required Feature Matrix

The feature matrix must include all categories and features from [data-model.md](../data-model.md).

Rules:

- Every normalized feature belongs to exactly one category.
- Every product-feature pair uses one of: supported, partially-supported, not-supported, unknown, or not-applicable.
- Unknown is valid and preferred over unsupported claims.
- Matrix notes explain scope differences, not marketing opinion.

## Required Page Targets

Individual comparison pages:

```text
/compare/skedular-vs-skedda
/compare/skedular-vs-officernd
/compare/skedular-vs-nexudus
/compare/skedular-vs-gable
/compare/skedular-vs-robin
/compare/skedular-vs-officely
/compare/skedular-vs-envoy
/compare/skedular-vs-kadence
/compare/skedular-vs-archie
/compare/skedular-vs-deskbird
```

Supporting pages:

```text
/compare/best-coworking-software
/compare/best-workspace-management-software
/compare/best-desk-booking-software
/compare/skedda-alternatives
/compare/officernd-alternatives
/compare/nexudus-alternatives
```

Rules:

- Page targets use `/compare` canonical paths.
- No page target may rely on a hardcoded comparison claim outside the shared data.
- Publication is all-or-nothing for the hub, individual comparison pages, and supporting pages.

## Individual Comparison Page Contract

Every individual comparison page must render:

- Overview
- Feature Matrix
- Pricing Comparison
- Integration Comparison
- Best For
- Limitations
- Why Teams Choose Skedular
- FAQ
- CTA

Rules:

- Section content comes from page targets, feature matrix, product records, FAQ entries, and reviewed claims.
- Pages must link back to `/compare`.
- Pages must link to relevant supporting pages when relationships are defined.
- Page copy uses American spelling and grammar.

## Supporting Page Contract

Every supporting page must render:

- Search-intent specific overview.
- Relevant product or competitor group from the shared dataset.
- Feature or selection criteria derived from the normalized matrix.
- Links to detailed competitor comparison pages.
- FAQ section when FAQ entries are configured.
- CTA.

Rules:

- Supporting pages must not introduce independent competitor facts.
- Empty or unpublished product groups block publication.

## Evidence and Review Contract

Published content must satisfy:

- 100% of Skedular capability claims have current source references.
- 100% of competitor claims have an evidence note or approved review status.
- Blocked claims do not render.
- Pending competitor support states render as unknown or are omitted from published copy.
- Content inventory identifies which source records generated each page.

## FAQ and Structured Data Contract

Rules:

- FAQ schema is emitted only when matching FAQ text is visible on the page.
- Structured data derives from the same page target and FAQ entries as visible content.
- Breadcrumb structured data must include `/compare` for comparison and supporting pages.
- Structured data must not include unsupported claims or hidden content.

## Validation Contract

Automated validation must fail when:

- A required route is missing.
- `/compare` does not link to every required generated page.
- A comparison entry from `/compare` points to the wrong route.
- A duplicate slug, page id, title, description, or canonical path exists.
- A published competitor claim lacks evidence/review status.
- A Skedular supported claim lacks current evidence.
- FAQ schema contains non-visible FAQ text.
- Any generated page lacks required metadata, canonical path, H1, CTA, or expected sections.
- A legacy comparison URL is preserved through redirect, alias, or generated page target.
