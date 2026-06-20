# Research: Skedular Competitor Comparison Hub

## Decision: Implement in Existing Astro Public-Web App

**Decision**: Build the comparison hub and generated comparison pages inside `src/web/apps/public-web`.

**Rationale**: The current public website is already an Astro static app with shared layout, SEO helpers, structured-data support, content inventory, dynamic `/compare/[slug]` routing, and Vitest/JSDOM build validation. The feature is public product-discovery content, not authenticated app behavior.

**Alternatives considered**:

- Next.js product app: rejected because the comparison hub belongs to the static public website and does not need authentication, Relay, or app runtime state.
- Separate CMS: rejected because the spec requires a single repository-owned source of truth and reviewable data artifacts.
- Backend API: rejected because comparison content is static, review-driven, and does not need runtime persistence.

## Decision: Replace Existing Comparison Data Instead of Migrating It

**Decision**: Remove the existing one-off comparison implementation and rebuild the comparison set from the new shared dataset. Do not redirect or alias legacy comparison URLs.

**Rationale**: The clarified spec requires a clean start. Existing comparison records are too shallow for the required page sections, feature matrix, FAQ schema, evidence state, and all-or-nothing publication gate.

**Alternatives considered**:

- Preserve the existing route as an alias: rejected by clarification.
- Redirect old URLs: rejected by clarification.
- Incrementally mutate the existing Skedda page: rejected because it preserves the one-off pattern the feature is meant to replace.

## Decision: Use Static Data Modules for Dataset, Matrix, and Page Targets

**Decision**: Represent competitors, Skedular evidence, normalized features, support states, FAQ entries, structured-data inputs, and page targets in TypeScript data modules under `src/web/apps/public-web/src/data`.

**Rationale**: Existing public-web content already uses typed data files for pages, pricing, content inventory, CTAs, routes, and SEO. Static data modules make generated routes, sitemap entries, tests, and review inventories deterministic.

**Alternatives considered**:

- Hardcode content directly in Astro pages: rejected because it would duplicate claims and make future competitor additions page-specific.
- Markdown-only pages: rejected because feature matrix validation, evidence review status, and generated page targets need structured fields.
- JSON files: considered viable, but TypeScript modules better match existing public-web patterns and give stronger local type checking.

## Decision: Publication Requires Evidence or Review Status

**Decision**: Treat the user-provided competitor list as seed data, but require every publishable competitor claim to carry an evidence note or explicit review status.

**Rationale**: Competitor claims create reputational and legal risk. The spec also requires no unsupported competitor limitations. A review/evidence gate lets implementation start from the seed dataset while preventing unsupported claims from becoming visible.

**Alternatives considered**:

- Publish seed data directly: rejected because it could publish stale or unsupported competitor claims.
- Require live external verification for every claim during implementation: rejected because it creates a network/process dependency beyond the static-site scope and is better represented as evidence/review metadata.

## Decision: Conservative Skedular Capability Baseline

**Decision**: Skedular support states in the feature matrix must be backed by current repo evidence, preferring active specs, help docs, current public-web data, split app routes, pricing data, and implemented contracts over older public website drafts.

**Rationale**: The user noted that the older public website draft may be outdated. The comparison pages must not overstate Skedular capabilities.

**Alternatives considered**:

- Use the old public website draft as source of truth: rejected by user feedback.
- Require production telemetry or live app data: rejected because the feature is static public-site content planning and current repo evidence is sufficient for publication gating.

## Decision: Route Supporting SEO Pages Under `/compare`

**Decision**: Generate all supporting best-software and alternatives pages under `/compare`.

**Rationale**: The comparison hub owns the comparison information architecture. Keeping supporting SEO pages under `/compare` simplifies canonical paths, sitemap expectations, internal linking, and tests.

**Alternatives considered**:

- Top-level SEO routes: rejected because they fragment the comparison section.
- Mixed routes: rejected because it creates extra canonical/link rules with no clear benefit.

## Decision: All-or-Nothing Publication Gate

**Decision**: Publish the comparison section only when `/compare`, all 10 individual comparison pages, and all 6 supporting pages pass validation together.

**Rationale**: The comparison hub is an index of a complete page set. Partial publication would create missing links, uneven SEO coverage, and more complex publication status rules.

**Alternatives considered**:

- Publish incrementally: rejected by clarification.
- Publish hub plus Skedda first: rejected by clarification and incomplete against the required page set.

## Decision: Generated Pages Share One Flexible Route

**Decision**: Use `/compare/index.astro` for the hub and a generated `/compare/[slug].astro` route for both competitor pages and supporting SEO pages.

**Rationale**: Existing public-web already uses `/compare/[slug].astro` for comparison pages. Extending one generated route keeps route generation and validation simple while still allowing page-type specific sections from data.

**Alternatives considered**:

- Separate route folders for alternatives and best-software pages: rejected because all supporting pages live under `/compare` and can be differentiated by page target type.
- One static Astro file per page: rejected because adding competitors should require data changes only.

## Decision: Structured Data Comes From Visible Page Data

**Decision**: Generate FAQ schema only when FAQs are visible on the page, and derive structured data from the same page target data used for rendering.

**Rationale**: The spec requires structured data to match visible content. Existing `StructuredData.astro` already accepts supplied graph data, so page targets can provide a graph while tests validate visible-content alignment.

**Alternatives considered**:

- Hidden FAQ schema for SEO only: rejected by the spec.
- Manual JSON-LD per page: rejected because it would duplicate generated page content.

## Decision: Observability Through Build and Test Diagnostics

**Decision**: For this static feature, satisfy logging/observability requirements with actionable build/test diagnostics and review inventories rather than runtime structured logs.

**Rationale**: The public-web comparison pages are statically generated and do not add server-side business workflows. Invalid data, missing evidence, incomplete page sets, and route/metadata failures are best caught before deploy.

**Alternatives considered**:

- Runtime request logging: rejected because the app is static and the repo feature does not own CDN/server logging.
- No diagnostics: rejected by the constitution and spec logging requirements.
