# Feature Specification: Astro Public Website

**Feature Branch**: `023-astro-public-website`
**Created**: 2026-06-04
**Status**: Draft
**Input**: User description: "Add a new public website app using Astro to the repository as the first minimal version in source control, representing Skedular properly at a high level."

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Visitor Gets a First Impression of Skedular (Priority: P1)

A potential customer visits the Skedular public website for the first time. They want to quickly understand what Skedular does, who it is for, and whether it is worth exploring further. They expect a professional, modern page that immediately communicates the core value proposition and gives them a clear next step.

**Why this priority**: This is the primary purpose of the public website — converting curious visitors into sign-up leads. Without this story, the website has no reason to exist.

**Independent Test**: Can be fully tested by building with a configured `PUBLIC_SKEDULAR_SIGNUP_URL`, visiting the root URL, and verifying the page loads with Skedular branding, a headline, a brief description of the product, and at least one clear call-to-action link leading to the configured main app sign-up URL.

**Acceptance Scenarios**:

1. **Given** a visitor opens the public website URL, **When** the page loads, **Then** they see a clearly branded Skedular page with the product name, a headline summarizing the core value, and a brief description of what Skedular does.
2. **Given** a visitor is on the home page, **When** they look for what to do next, **Then** at least one prominent call-to-action (e.g., "Try for Free" or "Get Started") is visible and links to the URL configured through `PUBLIC_SKEDULAR_SIGNUP_URL`.
3. **Given** a visitor wants to understand the product quickly, **When** they read the page, **Then** they can identify that Skedular is for hybrid teams and workspace management, without having to scroll far.
4. **Given** a visitor uses a mobile device, **When** the page renders, **Then** the content is readable and usable without horizontal scrolling.
5. **Given** `PUBLIC_SKEDULAR_SIGNUP_URL` is missing or empty, **When** a developer runs the production build, **Then** the build fails with a clear configuration error rather than producing a page with a broken or fallback CTA.
6. **Given** the home page is rendered in automated tests, **When** accessibility checks run, **Then** no critical axe violations are reported.

---

### User Story 2 - Developer Runs the Public Website Locally (Priority: P2)

A developer working in the monorepo needs to run the public website app locally to make content or style changes. They expect to use the same package manager and tooling conventions they already use for the rest of the web workspace.

**Why this priority**: Without a working local development setup, no one can maintain or extend the site. This story is required for the site to be a first-class citizen of the monorepo.

**Independent Test**: Can be fully tested by a developer cloning the repo, running the standard workspace install command, then running the public website's dev script, and confirming the site is accessible at a local port.

**Acceptance Scenarios**:

1. **Given** a developer has installed monorepo dependencies, **When** they run the public website dev script, **Then** the site starts and is accessible at a local URL.
2. **Given** a developer makes a content change to the home page, **When** they save the file, **Then** the change is reflected in the running local server without a full restart.
3. **Given** a developer runs the build script, **When** it completes, **Then** a production-ready static output is produced in the expected output directory.
4. **Given** a developer runs linting or formatting commands, **Then** the public website app participates correctly in the monorepo-wide lint and format runs.
5. **Given** a developer changes home page content while the dev server is running, **When** the file is saved, **Then** the changed content is served without restarting the process.
6. **Given** workspace dependency versions are mismatched, **When** the relevant workspace validation runs, **Then** it fails clearly rather than silently producing a potentially broken build.

---

### User Story 3 - Maintainer Deploys the Public Website to a Hosting Platform (Priority: P3)

A deployment maintainer or engineer needs to publish the public website to a hosting platform. They expect the built output to be compatible with both Cloudflare Pages and Vercel, with Cloudflare Pages as the primary target.

**Why this priority**: The site is only useful when it is publicly accessible. Deployment flexibility ensures the team is not locked into a single platform, but the immediate target is Cloudflare Pages.

**Independent Test**: Can be fully tested by running the build script, verifying the static output and documented settings are ready for Cloudflare Pages and Vercel, and measuring the Cloudflare-hosted page when a deployed URL is available.

**Acceptance Scenarios**:

1. **Given** a maintainer has built the site, **When** they inspect the output and documented Cloudflare Pages settings, **Then** the site is ready for static deployment without platform-specific code changes.
2. **Given** a maintainer has built the site, **When** they inspect the output and documented Vercel settings, **Then** the same static site is ready for fallback deployment without architectural changes.
3. **Given** the deployment README is read by a new team member, **When** they follow the documented steps, **Then** they can deploy the site to Cloudflare Pages without additional guidance.

---

### Edge Cases

- What happens when the visitor's browser blocks JavaScript? The page must still display meaningful content and CTAs as it is a primarily static marketing page.
- What happens if the monorepo workspace configuration is changed by another developer? The public website app must not break other apps and must remain correctly registered in the workspace.
- What happens during a build if shared package dependencies are mismatched? The build must fail fast with a clear error rather than silently producing a broken output.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The repository MUST contain a new public website app located under `src/web/apps/public-web` (or a name aligned with the repository naming convention, see Assumptions).
- **FR-002**: The app MUST provide a single publicly accessible home page that represents Skedular at a high level, including the product name, a headline, a brief product description, and at least one call-to-action linking to the main Skedular app URL configured through `PUBLIC_SKEDULAR_SIGNUP_URL`.
- **FR-003**: The home page content MUST be accurate, minimal, and consistent with the current Skedular brand identity — professional tone, focused on hybrid workspace management, and free of placeholder text.
- **FR-004**: The app MUST be registered as a workspace package within the existing monorepo workspace configuration so it participates in shared install, build, lint, and format pipelines.
- **FR-005**: The app MUST provide scripts for local development, production build, local preview of the production build, and any relevant validation (e.g., type check or lint).
- **FR-006**: The app's build output MUST be a static site (no server-side runtime required) compatible with edge/CDN hosting platforms.
- **FR-007**: The app MUST be deployable to Cloudflare Pages using its standard static deployment flow; deployment to Vercel MUST also be feasible without architectural changes.
- **FR-008**: The app MUST include a README describing how to run it locally and how to deploy it to Cloudflare Pages or Vercel.
- **FR-009**: The app MUST NOT require changes to any existing app's configuration, dependencies, or build pipeline, unless strictly necessary for workspace registration in shared configuration files.
- **FR-010**: The app's linting and formatting configuration MUST be consistent with the conventions used by the other web apps in the monorepo.
- **FR-011**: The production build MUST fail with a clear configuration error when `PUBLIC_SKEDULAR_SIGNUP_URL` is missing or empty; the app MUST NOT use a silent fallback CTA URL.
- **FR-012**: The app MUST include automated Vitest and Testing Library coverage for the home page, including its visible core-purpose content, configured CTA URL, missing-CTA build failure, and automated axe accessibility results.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: The public website MUST emit structured page-level build metadata (page count, output size) during the production build so build failures are immediately diagnosable.
- **LOG-002**: The website MUST be structured to allow future integration of a page analytics provider (e.g., Cloudflare Web Analytics or Vercel Analytics) without architectural rework — the README MUST note where to add analytics once a provider is chosen.
- **LOG-003**: Any build-time warnings or errors MUST surface clearly in CI output and MUST NOT be silently suppressed.
- **LOG-004**: The app MUST NOT log or expose sensitive environment variable values in static output or build artifacts.

### Key Entities _(include if feature involves data)_

- **Public Website App**: A self-contained web application within the monorepo workspace, producing a static output deployable to an edge hosting platform. Registered as a workspace package. Contains scripts, configuration, source pages, and a README.
- **Home Page**: The single page produced by this feature. Contains Skedular brand identity, headline, product description, key value propositions, and at least one call-to-action. No placeholders. No server-side data requirements.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: A developer familiar with the monorepo can run the public website locally within 5 minutes of checking out the repository, using only documented commands.
- **SC-002**: The production build completes without errors or warnings and produces a self-contained static output.
- **SC-003**: The static output is ready to deploy to Cloudflare Pages without platform-specific code changes using a standard static deployment workflow.
- **SC-004**: The home page loads in under 2 seconds on a standard broadband connection from the deployed Cloudflare hosting environment, measured after a Cloudflare URL is available.
- **SC-005**: The page passes an automated axe accessibility check and manual review with no critical violations (e.g., missing image alt text, poor contrast).
- **SC-006**: A first-time visitor can identify Skedular's core purpose within 10 seconds of the page loading, based on the visible headline and description.
- **SC-007**: No existing web app build, test, or lint pipeline is broken as a result of adding the new app.

## Assumptions

- The app is named `public-web` inside `src/web/apps/public-web` to match the user's stated preference. The existing `webapp-` prefix convention applies to product SaaS apps; a public marketing site is a distinct category and does not require that prefix. If the team prefers `webapp-public` for consistency, this can be decided at plan time.
- Astro is the chosen framework for this app. This is a technology constraint explicitly provided by the project stakeholder, not derived by the spec. All technology-agnostic requirements above describe what must be achieved; the plan will specify how Astro satisfies them.
- The site is treated as a mostly static public marketing website with no back-end runtime requirements for the initial version. Server-side rendering or API routes are out of scope for v1.
- Deployment configuration files (e.g., `wrangler.toml` for Cloudflare, `vercel.json` for Vercel) may be added as starter templates but are not required to be fully production-configured in this feature — the README is sufficient for v1.
- Content rewrite and full WordPress migration are explicitly out of scope. The home page will use accurate but minimal content derived from the current public website branding.
- No deep content research will be done as part of this feature. Proper website copy will be addressed in a separate follow-up task.
- The monorepo uses pnpm as the package manager and Turborepo for task orchestration. The new app must integrate with these conventions.
- No new shared packages (`@skedular/ui`, `@skedular/shared`) are expected to be consumed by the public website in v1, since it uses Astro rather than the React/MUI/Relay stack used by the product apps.
- Mobile responsiveness is required at a basic level (no horizontal scrolling, readable text). Full responsive design with breakpoints is deferred to a later content/design pass.
- Analytics integration is deferred to a later task; the app architecture must make it easy to add later.
- The main app sign-up destination is deployment-specific and is supplied through the public build-time environment variable `PUBLIC_SKEDULAR_SIGNUP_URL`; no default URL is assumed.
- An actual Cloudflare deployment is not required in this feature. Cloudflare performance measurement is required once a deployed URL is available, and an unavailable URL must be recorded as an environment limitation rather than replaced with a local-preview measurement.
