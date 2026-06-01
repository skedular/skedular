# Research: Help Webapps Documentation

## Decision: Use the existing three Nextra help apps

**Rationale**: The repository already contains `webapp-help`, `webapp-teams-help`, and `webapp-spaces-help` with Nextra, MDX routing, and content directories. Reusing them keeps the work focused on help content and navigation rather than introducing another documentation surface.

**Alternatives considered**:

- Create one shared help app with app sections: rejected because the product split expects three separate help projects.
- Move help into the product apps: rejected because help projects already exist and should remain independently reviewable.
- Introduce a new docs framework: rejected because it adds unnecessary migration risk.

## Decision: Build source inventory before writing help

**Rationale**: The spec requires help content to be grounded in existing specs, code, and UI pages. The inventory must include product split specs, customer cleanup spec, current help shells, route trees, root pages, visible navigation areas, forms, statuses, and major component states.

**Alternatives considered**:

- Draft from product assumptions first: rejected because unclear workflows could be documented incorrectly.
- Only inspect routes: rejected because routes alone do not capture forms, statuses, and component states.
- Block until every workflow is manually demoed: rejected because unclear flows can be listed as content gaps.

## Decision: Use topic pages plus task guides

**Rationale**: The first version must be comprehensive. Topic pages explain concepts and ownership, while task guides explain how to complete a workflow. This separates "what is this?" from "how do I do it?" and keeps pages easier to read.

**Alternatives considered**:

- Single overview page per app: rejected as too shallow.
- One large functionality page per app: rejected because it would become difficult to scan.
- Task guides only: rejected because users also need product boundaries and concept explanations.

## Decision: Treat every reviewed route, detail page, form, status, and major component state as a workflow candidate

**Rationale**: The clarification phase made coverage intentionally broad. Planning and tasks must create an inventory that maps each candidate to a help topic, task guide, out-of-scope decision, or content gap.

**Alternatives considered**:

- Only top-level routes: rejected because it misses detail pages, forms, and states users need help with.
- Every component file: rejected because the spec targets major component states, not internal implementation fragments.

## Decision: Use screenshot placeholders, not final screenshots

**Rationale**: The user wants to capture screenshots later. Placeholders let content authors reserve the right spots and labels without blocking the first documentation build on visual capture.

**Alternatives considered**:

- No screenshots or placeholders: rejected because future screenshot work would have no planned anchor.
- Capture all screenshots now: rejected because this plan phase is not implementation and screenshot capture can happen after content shape stabilizes.

## Decision: Public help with sensitive-detail guardrails

**Rationale**: The help centers should be readable without sign-in, but Teams and Spaces cover admin-heavy workflows. Public content must explain user-facing behavior without exposing sensitive customer data, payment secrets, security configuration details, internal operator procedures, or information that weakens billing, integration, or organization security.

**Alternatives considered**:

- Restrict all help to signed-in users: rejected because normal help centers should be public.
- Restrict only Teams and Spaces help: rejected for first version because content can be written safely with guardrails.
- Internal-only docs: rejected because the feature is for help webapps, not internal runbooks.

## Decision: Mark unclear or risky flows as content gaps

**Rationale**: The help must not guess. If a workflow cannot be explained accurately from the source inventory, it should be listed as a content gap with the missing source or review need.

**Alternatives considered**:

- Skip unclear flows silently: rejected because coverage tracking would be incomplete.
- Draft best guesses and fix later: rejected because public help could become misleading.
- Block all implementation on every unclear flow: rejected because content gaps provide a safe forward path.

## Decision: Verify through lint/build and structured content review

**Rationale**: This feature changes static help content. The right verification is help app lint/build plus review that content matches the source inventory, avoids sensitive details, uses American English, includes screenshot placeholders, and records content gaps.

**Alternatives considered**:

- Unit tests for every MDX page: rejected unless implementation adds executable behavior.
- End-to-end product tests: rejected because product workflows are not changed.
- No automated verification: rejected because help app builds must still pass.
