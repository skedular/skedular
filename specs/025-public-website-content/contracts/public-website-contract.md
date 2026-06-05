# Contract: Public Website Content Integration

## Route Contract

The first implementation must expose, at minimum, these route families:

```text
/
/teams
/spaces
/pricing
/resources
/resources/<resource-slug>
/support
/support/<support-slug>
/features/<feature-slug>
/compare/<comparison-slug>
```

Route requirements:

- Every published route has one `h1`, meaningful landmark structure, title metadata, description metadata, canonical path, and at least one relevant CTA.
- Every route is reachable from primary navigation, footer navigation, index pages, sitemap data, or a redirect.
- Every current public blog/support URL resolves to a published route or redirect target in the first implementation.
- Draft comparison routes are published in the first implementation.

## Environment URL Contract

The public website must require exactly these destination URL variables:

| Variable | Purpose | Used By |
|----------|---------|---------|
| `PUBLIC_SKEDULAR_APP_URL` | App/search/booking destination | Search Workspace, booking, app-forwarding actions |
| `PUBLIC_SKEDULAR_SIGNUP_URL` | Sign-up/login destination | Login, Sign Up, Get Started style actions |
| `PUBLIC_SKEDULAR_DEMO_URL` | Demo/contact destination | Book Demo, Contact Sales style actions |

Rules:

- Missing or empty values fail validation/build with clear variable-specific errors.
- No staging or production destination domain is hardcoded in source code, content data, tests, README commands, or deployment configuration.
- Local examples may use non-production example domains.
- Build diagnostics must not print full URL values.

## Content Inventory Contract

The implementation must maintain reviewable inventories for:

- Current public website pages/posts/support pages.
- Draft coverage items from every heading and major bullet group in `public-website-content-draft.md`.
- Primary pages and route metadata.
- Comparison pages and competitor claim review.
- Redirects from current public URLs to first-implementation destinations.

Minimum fields:

```text
id
source
sourceType
decision
destination
status
reviewNotes
```

## Editorial Contract

All public copy must:

- Use American spelling and grammar.
- Sound friendly, professional, plainspoken, specific, and human-written.
- Avoid generic AI-sounding phrasing, empty superlatives, repetitive sentence structures, and unsupported filler claims.
- Keep public booking language clear for visitors; avoid forcing public bookers to understand internal "marketplace" terminology.
- Distinguish Skedular, Teams, Spaces, public booking, and host/operator paths.

## Pricing Contract

The pricing page must represent the draft's pricing strategy:

- Teams: active-user pricing model and draft tier structure.
- Spaces: location subscription model and draft tier structure.
- Public Booking: no subscription for bookers; users pay only for bookings.
- Hosts: commission model and draft commission range.

Rules:

- Pricing values are centralized in content data.
- Repeated pricing references must read from the same source.
- Pricing copy remains easy to update if business approval changes before launch.

## Comparison Page Contract

Every comparison page must include:

- Competitor name.
- Search-focused title and description.
- Neutral explanation of Skedular's positioning.
- Reviewed capability comparison content.
- No copied competitor language.
- No unverified competitor claims.
- Relevant CTA sourced from configured URL variables or internal routes.

## SEO and AI Discoverability Contract

Every primary page, feature page, resource page, support page, and comparison page must include:

- Unique title.
- Unique description.
- Canonical path.
- Meaningful heading hierarchy.
- Machine-readable content structure.
- Sitemap inclusion or explicit redirect.
- Structured data candidates where appropriate:
  - Organization
  - Product
  - FAQ
  - Breadcrumb

## Accessibility Contract

The implementation must preserve or improve:

- Keyboard navigation.
- Semantic landmarks.
- Exactly one page-level heading per route.
- Descriptive links.
- Alternative text for meaningful images.
- Decorative images hidden from assistive technology.
- Focus indicators.
- Readable contrast.
- Responsive layouts without horizontal scrolling.
- No critical automated axe violations in tested output.

## Performance Contract

The public website must remain static-first:

- No runtime server dependency.
- Minimal JavaScript.
- Optimized images where used.
- Stable layout for hero/search/product/resource sections.
- Build output remains compatible with Cloudflare Pages direct upload.

Target deployed measurements:

- Lighthouse Performance 95+.
- Lighthouse Accessibility 95+.
- Lighthouse SEO 100.
- Lighthouse Best Practices 100.
- LCP under 1.5s.
- INP under 200ms.
- CLS under 0.05.

## Review Contract

Before launch, the implementation must provide evidence of:

- Complete draft coverage.
- Full current blog/support migration.
- Comparison page competitor claim review.
- Pricing content review.
- Product capability claim review.
- Human-quality editorial review.
- Accessibility review.
- Link and redirect validation.
- Environment URL validation.
