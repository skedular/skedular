# Research: Skedular Documentation Center

## Decision: Deliver documentation in `public-web` only

**Rationale**: The public web app already owns public navigation, canonical URL generation, SEO metadata, sitemap, robots, LLM-readable pages, and the static deployment. The clarification explicitly excludes separate documentation route trees in Teams, Spaces, and Host.

**Alternatives considered**:

- Restore the former product-specific help apps: rejected because they are not present in the current application tree and would split discovery and maintenance.
- Add a documentation route to each product app: rejected by clarification and would duplicate SEO/navigation work.

## Decision: Use an Astro built-in Markdown content collection plus a typed documentation catalog

**Rationale**: Existing marketing pages use typed data modules and static dynamic routes. A docs collection preserves that static model while letting hundreds of independently reviewable guides carry typed front matter. The catalog owns product/category order, route validation, evidence references, and relationships that cannot safely be inferred from a document body.

**Alternatives considered**:

- Put all documentation bodies in one TypeScript array: rejected as impractical for long-form, hundreds-page authoring.
- Add a third-party documentation platform: rejected because it adds a new public surface and does not reuse public-web SEO/layout behavior.
- Use a runtime CMS: rejected because static, repository-reviewed content is sufficient for the requested foundation.

## Decision: Use one `/docs` static route family

**Rationale**: A hub at `/docs`, product landing paths such as `/docs/teams`, and product/category/article paths provide durable, readable addresses. A catch-all static route can render the typed catalog consistently while exact route validation prevents collisions.

**Alternatives considered**:

- Put docs below `/resources`: rejected because documentation is task guidance, not editorial resource content.
- Use flat `/docs/<article>` paths: rejected because category and product context would be lost at scale.

## Decision: Make the initial library evidence-led and safe by default

**Rationale**: Current product routes, public product data, the completed Help feature inventory, and completed Host work establish a defensible capability baseline. Articles may explain only verified behavior. Sensitive or ambiguous payment, refund, identity, accounting, and integration details need safe summaries or explicit gaps rather than invented instructions.

**Alternatives considered**:

- Write broad marketing-style feature pages first: rejected because they could imply unsupported behavior.
- Wait for a full live-product walkthrough: rejected because the catalog can state a verified purpose, scope, and next step while tracking unsupported detail as a gap.

## Decision: Treat useful placeholders as published, structured articles

**Rationale**: The specification requires a page for every discovered feature. A placeholder is useful only when it has a verified summary, product/category context, prerequisites when known, evidence, next step, and related articles. It is indexed only when it meets the normal article contract.

**Alternatives considered**:

- Empty "coming soon" pages: rejected because they harm reader trust and search quality.
- Hide all incomplete guides: rejected because it breaks inventory coverage and navigation continuity.

## Decision: Extend existing discovery primitives instead of building separate SEO files

**Rationale**: `publicPages` already drives sitemap eligibility and canonical robots behavior. `SiteLayout` applies standard metadata and structured data, while `llms.txt` uses the same published inventory. Documentation should join those sources rather than maintaining a disconnected list.

**Alternatives considered**:

- Hand-maintained docs sitemap/robots lists: rejected because they can drift from article publication state.
- Index every source Markdown file automatically: rejected because draft/future/sensitive content must remain excluded.

## Decision: Verify compiled output, catalog validity, and reader navigation

**Rationale**: Existing public-site tests build the static site and inspect compiled HTML for metadata, canonical URLs, landmarks, and discovery files. Documentation adds catalog-level tests for unique routes/evidence and rendered-page tests for navigation and SEO; manual review covers keyboard flow, responsive layout, and color modes.

**Alternatives considered**:

- Only run a build: rejected because a valid build can still publish duplicate, unlinked, or non-indexable articles.
- Product E2E tests: rejected because the feature does not change product workflows.
