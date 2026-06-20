# Documentation Content Contract

## Public URL Contract

| Page | Canonical address |
| --- | --- |
| Documentation home | `/docs` |
| Product landing | `/docs/<product>` |
| Category listing | `/docs/<product>/<category>` |
| Article | `/docs/<product>/<category>/<article-slug>` |
| Shared-concept article | `/docs/shared/<category>/<article-slug>` |
| Reserved future roots | `/docs/api`, `/docs/release-notes`, `/docs/versions` |

`<product>` is initially `teams`, `spaces`, or `host`. `shared` is reserved for clearly labeled cross-product concepts and does not create a fourth product landing. Category and article segments use lowercase hyphenated slugs. Existing public documentation paths are immutable after publishing; title changes do not change an address.

## Required Article Contract

Every published article must supply:

```text
id, title, description, product, category, slug, articleKind,
publicationState, evidenceRefs, terminologyRefs, relatedArticleIds, updatedAt
```

Every rendered article must supply:

- one H1 and ordered subordinate headings;
- canonical URL, description, robots directive, social metadata, and appropriate article/breadcrumb structured data;
- breadcrumb trail from Documentation through product and category;
- previous/next navigation when the category has an ordered sequence;
- at least one relevant next step or related article unless it is a product landing;
- readable main content and navigation in the public website's responsive and color-mode design.

## Publication and Discovery Contract

| State | Route | Sitemap/robots/LLM inventory | Required behavior |
| --- | --- | --- | --- |
| `published` | Generated | Included | Meets all metadata, evidence, and navigation validation. |
| `draft` | Not generated | Excluded | May exist in the repository only. |
| `placeholder` article kind + `published` state | Generated | Included | Has verified scope, next step, evidence, and related content; never empty. |
| `future`, `content-gap` | Not generated | Excluded | Does not imply live product availability. |
| `withdrawn` | Non-indexable retirement route or verified replacement redirect | Excluded | Retains a safe path for a previously published Documentation Center URL without implying live availability. |

## Initial Information Architecture Contract

Each product has the required categories below. The first category includes the full guide listed; the remaining items establish the initial useful-placeholder coverage map.

### Teams

- **Getting Started**: Set up a private workplace, add locations/resources, organize people, and create the first private booking.
- **Core Features**: Organizations, locations, resources, zones, floor plans, teams, users, attendance, availability dashboard, analytics.
- **Bookings**: Create, view, and manage private bookings; understand access-dependent availability.
- **Settings**: Organization administration, access, notifications, and enterprise sign-in entry points.
- **Integrations**: Slack and Microsoft Teams setup/use guidance, with safe sign-in context.
- **FAQs**: Private-workplace boundaries, resources, bookings, floor plans, analytics, and integrations.
- **Best Practices**: Resource naming, workplace rollout, and availability hygiene.

### Spaces

- **Getting Started**: Create a marketplace organization, model locations/resources, create an offer, prepare availability, then continue to payment and publishing setup.
- **Core Features**: Marketplace setup, locations/resources/zones/floor plans, products/pricing, marketplace publishing, customers, analytics.
- **Bookings**: Marketplace and operator-created bookings, subscriptions, and public-safe refund guidance.
- **Settings**: Organization access, settings, customer administration, bank account and payment-connection guidance.
- **Integrations**: Slack, Microsoft Teams, enterprise sign-in, and public-safe Xero accounting guidance.
- **FAQs**: Marketplace, offers, subscriptions, publishing, payments, and customer operations.
- **Best Practices**: Product catalog, availability, publishing, and customer-operations guidance.

### Host

- **Getting Started**: Create a Host organization, add a place, configure pricing/policies, connect payments, and progress from private draft to published listing.
- **Core Features**: Place/listing details, availability, booking rules, cancellation, media, amenities, draft/publication lifecycle.
- **Bookings**: Bookings, renters, payments, cancellations, and refunds at a public-safe level.
- **Settings**: Organization, host settings, and payment connection.
- **Integrations**: Payment connection guidance; categories with no verified live integration explain the Host boundary.
- **FAQs**: Place-first model, listing drafts, publication, pricing, and commission questions.
- **Best Practices**: Listing completeness, pricing, availability, media, and booking management.

## Safety Contract

Public articles must not expose credentials, customer records, payment or accounting internals, security configuration, provider-specific failure mechanics, or assumptions not supported by evidence. An unclear detail uses a public-safe explanation and support next step, or remains a non-published content gap.
