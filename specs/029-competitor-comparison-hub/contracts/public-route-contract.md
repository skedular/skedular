# Contract: Public Routes and SEO

## Route Contract

The comparison section must expose exactly one hub route and generated page routes under `/compare`:

```text
/compare
/compare/<comparison-or-supporting-slug>
```

Required generated paths are listed in [comparison-content-contract.md](./comparison-content-contract.md).

Rules:

- `/compare` is the primary index for comparison content.
- Every listed page on `/compare` must navigate to its generated canonical path.
- Every generated page must link back to `/compare`.
- Existing one-off comparison URLs are removed with no redirect, alias, or preserved legacy rendering.

## Metadata Contract

Every generated page must include:

- Unique `<title>`.
- Unique meta description.
- Canonical URL using its `/compare` path.
- Open Graph title, description, URL, and image.
- Twitter image metadata.
- Robots metadata derived from publication status.

Rules:

- Published pages are indexable.
- Draft, blocked, or incomplete pages are not included in the published comparison section.
- Duplicate metadata blocks publication.

## Sitemap and LLMs Contract

Rules:

- Published generated comparison pages appear in the sitemap through the public page inventory.
- `/compare` and generated pages appear in public page/LLM inventories when published.
- Legal/privacy routes remain excluded from sitemap as currently configured.
- Supporting SEO pages use `/compare` canonical paths.

## Structured Data Contract

Generated comparison pages may use:

- SoftwareApplication/Product-style structured data for Skedular comparison context.
- FAQPage when visible FAQs exist.
- BreadcrumbList.
- ItemList for hub or supporting list pages when rendered visibly.

Rules:

- Structured data must be generated from visible page content.
- FAQPage must not be emitted when no visible FAQ section exists.
- Breadcrumb entries must resolve to public routes.

## Accessibility and Layout Contract

Every generated page must preserve existing public-web page standards:

- Exactly one H1.
- Header, main, and footer landmarks.
- Descriptive links.
- Keyboard-reachable navigation.
- No horizontal scrolling at common mobile widths.
- CTA links with `data-cta-id`.
- American English visible copy.

## No Legacy Redirect Contract

The implementation must not add or preserve redirect data, route aliases, or static pages for removed one-off comparison URLs.

Validation must check:

- Removed legacy comparison paths are not present in generated static output.
- Removed legacy comparison paths are not listed in comparison page targets.
- Removed legacy comparison paths are not listed as redirects.
- Removed legacy comparison paths are not linked from the hub, sitemap, navigation, footer, or generated pages.
