# Help Content Contract

This contract defines the expected shape and quality gates for the three help webapps.

## Scope

Applies to:

- `src/web/apps/webapp-help`
- `src/web/apps/webapp-teams-help`
- `src/web/apps/webapp-spaces-help`

The contract covers MDX help pages, metadata files used for navigation, screenshot placeholders, content gaps, and review evidence.

## Required Page Types

### Help Home Page

Each help app must have a home page that includes:

- one clear purpose statement
- primary audience
- main app responsibilities
- what belongs in the other help apps
- link or navigation path into documentation

### Topic Page

Each product area topic page must include:

- title
- audience
- purpose
- when to use this area
- common tasks
- important statuses or states
- ownership boundary
- links to task guides
- links to related content gaps if the area is incomplete

### Task Guide

Each task guide must include:

- title
- intended reader
- starting point
- prerequisites or conditions
- ordered steps
- expected result
- relevant states, permissions, policy constraints, or configuration dependencies
- screenshot placeholder when visual guidance is needed
- content gap reference when a branch cannot be documented safely

### Content Gap Entry

Each content gap must include:

- title
- owning help app
- affected product area or workflow
- source path or missing source
- reason the content cannot be safely completed
- review needed
- expected resolution

### Screenshot Placeholder

Screenshot placeholders must be clear and easy to replace later.

Allowed placeholder format:

```mdx
> Screenshot needed: [short label]
> Capture later: [what screen, form, status, or step should be shown]
```

## Coverage Rules

- Every reviewed route, detail page, form, status, and major component state must map to a help topic, task guide, out-of-scope decision, or content gap.
- Every topic and guide must cite or be traceable to the source inventory.
- Unclear, risky, transitional, or insufficiently supported workflows must become content gaps instead of guessed help.
- Customer help must not document private organization or marketplace operator administration as customer-owned work.
- Teams help must not document marketplace discovery, storefronts, customer subscriptions, marketplace refunds, payment setup, or product publishing as Teams-owned work.
- Spaces help must not document private team management or customer personal self-service as Spaces-owned work.

## Public Safety Rules

The help centers are public. Content must not expose:

- customer personal data
- payment secrets or payment provider credentials
- security configuration secrets
- internal operator-only procedures
- integration tokens or webhook secrets
- details that weaken account, billing, organization, or integration security

Security-sensitive areas should explain user-facing behavior and direct readers to approved admin or support channels where needed.

## Writing Rules

- Use American spelling and grammar.
- Use short headings and plain sentences.
- Prefer practical wording over marketing language.
- Avoid unexplained internal terms.
- Use the same names for shared concepts across all three help apps.
- Explain app boundaries when a term appears in more than one app.
- Do not leave placeholder text except explicit screenshot placeholders.

## Review Contract

Before acceptance, reviewers must confirm:

- all source inventory items are mapped
- major workflow coverage is complete or safely marked as a gap
- screenshot placeholders exist where needed
- public safety rules pass
- product boundaries are accurate
- the help reads simply and directly
- all affected help apps lint and build
