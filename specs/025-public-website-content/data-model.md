# Data Model: Public Website Content Integration

## Entity: Public Page

Represents a generated public website route.

Fields:

- `id`: stable unique identifier.
- `path`: public route path, beginning with `/`.
- `title`: unique browser/search title.
- `description`: unique search/social summary.
- `audience`: visitor group such as public booker, Teams buyer, Spaces operator, host, search visitor, or support reader.
- `pageType`: home, product, pricing, resource, support, feature, comparison, legal, redirect target, or utility.
- `sourceRefs`: draft sections, current public URLs, or reviewed reference inputs.
- `primaryCtaId`: reference to a call-to-action.
- `secondaryCtaIds`: optional list of supporting calls-to-action.
- `canonicalPath`: canonical route path.
- `metadataStatus`: missing, drafted, reviewed, or approved.
- `contentStatus`: inventory, drafted, rewritten, reviewed, approved, or published.
- `structuredDataTypes`: candidate structured-data types such as Organization, Product, FAQ, or Breadcrumb.
- `requiresClaimReview`: boolean.
- `requiresCompetitorReview`: boolean.
- `requiresPricingReview`: boolean.

Validation:

- `path`, `title`, and `description` must be unique across published pages.
- Every primary page must have a `primaryCtaId`.
- Published comparison pages must have `requiresCompetitorReview = false` after review or an approved review record.
- Published pricing pages must include centralized pricing references, not duplicated hardcoded values.

## Entity: Product Path

Represents a public product journey.

Fields:

- `id`: `skedular`, `teams`, `spaces`, `hosts`, or another approved path.
- `displayName`: public label.
- `audience`: target buyer/user.
- `positioning`: short human-written value statement.
- `capabilityGroups`: ordered groups of capabilities.
- `pricingModelRef`: optional link to pricing model.
- `primaryPageId`: related public page.
- `primaryCtaId`: related call-to-action.

Validation:

- Product path copy must not require public bookers to understand "marketplace".
- Teams and Spaces must remain distinct: Teams for private workplace management, Spaces for operator/business management.

## Entity: Resource Article

Represents migrated blog/support content or a new resource page.

Fields:

- `id`: stable slug-like identifier.
- `sourceUrl`: current public URL or draft source.
- `destinationPath`: new public route.
- `title`: public article title.
- `summary`: article summary.
- `publishedDate`: original or reviewed date when applicable.
- `topicTags`: search and content topics.
- `migrationDecision`: publish, rewrite, merge, or redirect.
- `redirectTargetPath`: required when merged or redirected.
- `contentStatus`: inventory, drafted, rewritten, reviewed, approved, or published.
- `claimReviewStatus`: not required, pending, approved, or blocked.

Validation:

- Every current public blog/support URL must have a first-implementation `destinationPath` or `redirectTargetPath`.
- Redirect targets must be published pages.
- Outdated content must be rewritten before publication.

## Entity: Comparison Page

Represents a draft comparison page such as `skedular-vs-skedda`.

Fields:

- `id`: stable identifier.
- `path`: public route path under `/compare/`.
- `competitorName`: named alternative.
- `searchIntent`: query intent targeted by the page.
- `skedularPositioning`: neutral positioning statement.
- `claimList`: reviewed comparison claims.
- `primaryCtaId`: related call-to-action.
- `metadataStatus`: missing, drafted, reviewed, or approved.
- `competitorReviewStatus`: pending, approved, or blocked.

Validation:

- All comparison candidates listed in the draft must be published in first implementation.
- Competitor claims must be factual, neutral, and review-approved.
- If a competitor claim cannot be verified, the page must use neutral positioning rather than omit the page.

## Entity: Draft Coverage Item

Represents section-by-section coverage of `src/web/apps/public-web/public-website-content-draft.md`.

Fields:

- `id`: stable item id.
- `heading`: draft heading text.
- `sourceLineStart`: source line when available.
- `sourceLineEnd`: optional source line.
- `contentType`: page, feature, capability, pricing, SEO, accessibility, performance, technical constraint, future item, or note.
- `decision`: publish, rewrite, merge, redirect, technical-planning, future-planning, or exclude.
- `destinationRef`: public page, resource article, comparison page, planning artifact, or review checklist item.
- `verificationStatus`: pending, reviewed, approved, or blocked.

Validation:

- Every heading and major bullet group in the draft must have a coverage item before implementation starts.
- Future items must not be published as current capabilities.

## Entity: Migration Decision

Represents how a current public URL or draft item is handled.

Fields:

- `source`: current URL, draft section, or reference page.
- `sourceType`: blog, support, page, feature, comparison, pricing, future, or technical.
- `decision`: publish, rewrite, merge, redirect, technical-planning, future-planning, or exclude.
- `reason`: concise explanation.
- `destination`: new path or artifact.
- `owner`: reviewer or owning function when known.
- `status`: pending, approved, blocked, or complete.

Validation:

- Blog/support decisions cannot be `exclude` unless there is a published equivalent or redirect to a published replacement.
- Excluded non-blog/support items require a reason.

## Entity: Capability Claim

Represents a public claim about Skedular, an integration, pricing, competitor, or product capability.

Fields:

- `id`: stable identifier.
- `claimText`: public-facing claim.
- `claimType`: product capability, integration, pricing, security, competitor, performance, or roadmap.
- `sourceRefs`: draft/current-product/reference inputs.
- `reviewStatus`: pending, approved, rewritten, blocked.
- `publishedPageIds`: pages where the claim appears.

Validation:

- Current-state claims must be verified before publication.
- Future claims must be routed to future planning or written as future/roadmap only when approved.
- Competitor claims require competitor review.

## Entity: Public Destination URL

Represents required outbound URLs provided by public environment variables.

Fields:

- `envName`: public environment variable name.
- `purpose`: app/search, login/sign-up, or demo/contact.
- `required`: always true for this feature.
- `localExample`: non-production example value for documentation and tests.
- `validationError`: clear build/check error text.

Required records:

- `PUBLIC_SKEDULAR_APP_URL`: app/search/booking destination.
- `PUBLIC_SKEDULAR_SIGNUP_URL`: sign-up/login destination; existing pattern to preserve.
- `PUBLIC_SKEDULAR_DEMO_URL`: demo/contact destination.

Validation:

- Missing or empty values fail clearly during validation/build.
- Source code and content must not hardcode staging or production destination domains for these actions.
- Logs/build diagnostics must not print full destination values.

## Entity: Call-to-Action

Represents a public action.

Fields:

- `id`: stable identifier.
- `label`: visible CTA text.
- `purpose`: search, book, demo, login, sign-up, contact, learn-more, support, or community.
- `destinationType`: internal route, public destination URL, email, or external community.
- `destinationRef`: path or environment-variable reference.
- `audience`: intended visitor.

Validation:

- Search/book actions use `PUBLIC_SKEDULAR_APP_URL`.
- Login/sign-up actions use `PUBLIC_SKEDULAR_SIGNUP_URL`.
- Demo/contact sales actions use `PUBLIC_SKEDULAR_DEMO_URL`.
- Link text must be descriptive and accessible.

## State Transitions

Content lifecycle:

```text
inventory -> drafted -> reviewed -> approved -> published
                 |          |
                 v          v
              rewritten   blocked
```

Migration lifecycle:

```text
discovered -> decision-recorded -> destination-created -> link-validated -> complete
```

Claim lifecycle:

```text
identified -> pending-review -> approved -> published
                         |
                         v
                      rewritten/blocked
```
