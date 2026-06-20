# Quickstart: Validate the Documentation Center

## Prerequisites

From the repository root, install workspace dependencies as needed and provide the public-web URL variables described in [public-web README](../../src/web/apps/public-web/README.md).

```bash
pnpm --dir src/web/apps/public-web test
pnpm --dir src/web/apps/public-web check
pnpm --dir src/web/apps/public-web lint
```

Run the production build with the required public URL variables from the README:

```bash
pnpm --dir src/web/apps/public-web build
```

## Automated Validation

1. Run the documentation catalog tests. Confirm every published article has a unique address, valid product/category, evidence reference, metadata, and valid relationship targets.
2. Run the existing public-site content tests. Confirm the generated `/docs` home, product landing pages, category pages, and representative articles have one H1, canonical URL, description, robots directive, landmarks, and documentation navigation.
3. Confirm published documentation appears in `sitemap.xml` and the LLM-readable index, while draft, future, content-gap, and withdrawn items do not.
4. Confirm the main and footer/resource navigation expose Documentation and resolve to `/docs`.
5. Confirm a production build has no broken internal documentation links or duplicate static paths.

## Manual Review

1. Open `/docs`, `/docs/teams`, `/docs/spaces`, and `/docs/host`. Verify that each product boundary is clear and each Getting Started guide is reachable within two selections.
2. Follow the three Getting Started journeys. Cross-check every step against the relevant current product route or approved evidence reference.
3. Open at least one feature placeholder per product. Verify it names a verified capability, provides a useful next step, and does not make an unsupported claim.
4. On a narrow viewport and with keyboard-only navigation, traverse primary navigation, breadcrumbs, category links, previous/next controls, and related articles. Verify visible focus and logical reading order.
5. Review documentation in both supported color preferences. Verify text, links, cards, and hierarchy remain legible.
6. Check a representative payment/refund/integration article for secrets, sensitive instructions, provider internals, or claims beyond its evidence. It must instead provide public-safe guidance and a support next step.
7. Run the guided-usability review with at least 10 readers who are new to the relevant product. For each reader, record whether they identify the correct product and open its Getting Started guide within 30 seconds, then whether they can name the next setup action after reading that guide without help. Compare the recorded results with SC-003 (90%) and SC-004 (85%).
8. Verify that a withdrawn documentation fixture is absent from sitemap, robots, and LLM output, while its former path either redirects to a verified replacement or renders a non-indexable retirement page with a link to the documentation home.

## Expected Outcome

The public-web build produces a discoverable, canonical `/docs` route family with three product landing pages, required categories, complete Getting Started guidance, evidence-backed initial feature coverage, and no documentation routes in Teams, Spaces, or Host applications.
