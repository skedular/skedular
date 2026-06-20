# Data Model: Skedular Documentation Center

## Documentation Article

One public guide rendered at a stable `/docs` address.

| Field | Rules |
| --- | --- |
| `id` | Globally unique, stable identifier. |
| `title`, `description` | Required, unique among published pages, concise, and written in American English. |
| `product` | `teams`, `spaces`, `host`, or `shared`. Shared articles use the separate `/docs/shared/...` route family and are visibly labeled cross-product. |
| `category` | Required product category; must belong to the selected product taxonomy. |
| `slug` | Lowercase, URL-safe, unique within its product/category. |
| `publicationState` | `published`, `draft`, `future`, `content-gap`, or `withdrawn`. Only `published` is indexable. Withdrawn articles retain a non-indexable retirement route or a verified replacement redirect so a previously published address is not a dead end. |
| `articleKind` | `landing`, `guide`, `reference`, `faq`, `best-practice`, or `placeholder`. |
| `evidenceRefs` | One or more reviewed repository/public-content/approved-artifact references for published or placeholder articles. |
| `terminologyRefs` | One or more terms from the maintained documentation glossary for published or placeholder articles; records the terminology review that applies to the article. |
| `relatedArticleIds` | Zero or more valid article IDs; cross-product links must be intentionally labeled. |
| `replacementArticleId` | Required for a withdrawn article when a verified replacement exists; otherwise its retirement route links to the documentation home. |
| `previousId`, `nextId` | Optional category-order navigation targets. |
| `updatedAt` | Review date used for discovery freshness. |
| `body` | Markdown guidance; placeholders require verified purpose, scope, next step, and related content. |

**Validation rules**

- A published article has exactly one canonical address and one product/category context.
- Published and placeholder articles cannot make a claim absent from `evidenceRefs`.
- A withdrawn, future, draft, or content-gap article is excluded from sitemap, robots eligibility, and LLM page lists. A withdrawn article may retain only a non-indexable retirement route at its former address, or redirect to `replacementArticleId`; it is never a crawlable live guide.
- Slug collisions, duplicate titles/descriptions for public pages, orphaned relationships, or relationships across unknown products fail validation.

## Documentation Category

An ordered group of articles beneath a product landing page.

| Field | Rules |
| --- | --- |
| `id`, `label`, `description`, `product`, `order` | Required. |
| `articleIds` | Ordered unique references to published product articles. |
| `emptyState` | Required when no live capability applies; explains the product boundary without claiming a feature. |

Every product begins with: Getting Started, Core Features, Bookings, Settings, Integrations, FAQs, and Best Practices. Additional categories can be added without changing existing URLs.

## Product Documentation Hub

| Product | Audience | Verified initial coverage |
| --- | --- | --- |
| Teams | Private workplace administrators and members | Private organizations; locations/resources/zones/floor plans; private bookings; teams/users; availability/analytics; access/settings; Slack, Microsoft Teams, and enterprise sign-in. |
| Spaces | Commercial workspace operators | Marketplace setup; locations/resources/zones/floor plans; products/pricing/publishing; bookings/subscriptions/customers; public-safe payments, bank accounts, refunds, and Xero; analytics/settings/access; Slack, Microsoft Teams, and enterprise sign-in. |
| Host | Independent hosts | Onboarding/organization; place/listing lifecycle; details, pricing, availability, rules, cancellation, media, amenities, drafts/publication; bookings/renters; payment connection, commissions, analytics, and settings. |

## Capability Inventory Item

Tracks the coverage obligation behind every live feature discovered during research.

| Field | Rules |
| --- | --- |
| `capability`, `product`, `sourceRef`, `sensitivity` | Required; sensitivity is `normal`, `public-safe`, `unclear`, or `future`. |
| `coverageDecision` | Exactly one of `article`, `placeholder`, `shared-article`, `content-gap`, or `exclude`. |
| `articleId` | Required for `article`, `placeholder`, or `shared-article`. |
| `reason` | Required for `content-gap` and `exclude`. |

## State Transitions

```text
Capability: discovered -> evidenced -> covered
                                  -> content-gap
                                  -> excluded

Article: draft -> published -> withdrawn (replacement redirect or non-indexable retirement route)
              -> future
              -> content-gap
```

Publishing validates route uniqueness, required metadata, evidence, product/category membership, related links, and discovery eligibility before the article enters the sitemap and LLM inventory.
