---
id: documentation-authoring-readme
title: "Documentation content authoring"
description: "Internal instructions for maintaining Skedular documentation."
product: shared
category: core-concepts
slug: documentation-authoring-readme
articleKind: reference
publicationState: draft
evidenceRefs:
  - doc-resources/*.md
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds: []
updatedAt: 2026-07-14
---

# Documentation content authoring

Each published article needs reviewed evidence, a `docs-glossary:v1` terminology reference, a clear product/category scope, and links to the next useful guidance. Use American English and describe only verified behavior.

The reviewed source inventory is maintained in `src/data/documentation-source-map.ts`. It maps every Markdown file in `doc-resources` to the article(s) that use it. Shared concepts include organizations, locations, resources, bookings, availability, floor plans, tags, zones, analytics, and users. Skedular Teams adds private teams and workplace access; Skedular Spaces adds products, subscriptions, billing, payments, and accounting integrations; Skedular Host keeps a place-first listing workflow.

Use **resource** for bookable inventory. Keep **tags** (labels) distinct from **zones** (location groupings), and keep **availability** (what can be booked) distinct from **analytics** (what happened).

Keep sensitive payment, identity, accounting, and integration details out of public articles. A withdrawn published address must redirect to a verified replacement or render a non-indexable retirement page. Future API, release-note, locale, version, media, and search entries must use reserved paths without changing published addresses.
