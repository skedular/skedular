# Review Notes

## Static-Doc Diagnostics Decision

This feature adds static public MDX help content and Nextra navigation metadata only. It does not add a new runtime business workflow, API endpoint, background job, event consumer, persistence path, or client-side product workflow. Existing platform, lint, and build diagnostics are therefore the correct diagnostics boundary for this slice.

Documentation decisions are recorded in the source inventory and content gap register so app-boundary and uncertainty decisions are traceable.

## Reader Review Sample

Sample task classification used for first-version review:

| Task | Expected help app | Result |
| --- | --- | --- |
| "I want to book a desk from a public marketplace." | Customer Help | Covered by Customer overview, Products, and Customer guides. |
| "I need to add a private workplace location for my company." | Teams Help | Covered by Teams overview, Bookings/locations/resources, and Teams guides. |
| "I need to publish a co-working product and manage refunds." | Spaces Help | Covered by Spaces overview, Commerce operations, and Spaces guides. |

Result: the three overview pages identify the correct help app for customer, private organization, and marketplace operator tasks.

## Public Access Review

The help pages are written as public documentation. They avoid customer personal data, payment secrets, security configuration details, integration secrets, and internal operator procedures.

## Verification Results

| Check | Result | Notes |
| --- | --- | --- |
| `pnpm --dir src/web/apps/webapp-help lint` | Passed | No ESLint output. |
| `pnpm --dir src/web/apps/webapp-teams-help lint` | Passed | No ESLint output. |
| `pnpm --dir src/web/apps/webapp-spaces-help lint` | Passed | No ESLint output. |
| `pnpm --dir src/web/apps/webapp-help build` | Passed outside sandbox | Sandboxed build hung at Turbopack production build stage. Escalated run completed. Nextra warned about missing Git timestamps for new uncommitted MDX files. |
| `pnpm --dir src/web/apps/webapp-teams-help build` | Passed outside sandbox | Nextra warned about missing Git timestamps for new uncommitted MDX files. |
| `pnpm --dir src/web/apps/webapp-spaces-help build` | Passed outside sandbox | Nextra warned about missing Git timestamps for new uncommitted MDX files. |

## Final Review

- Source inventory maps every reviewed route group to help content, an out-of-scope note, or a content gap.
- The three home pages and overview pages identify the correct help app for customer, private organization, and marketplace operator tasks.
- Public help avoids sensitive customer data, payment secrets, security configuration details, integration secrets, and internal operator procedures.
- Screenshot placeholders follow the agreed `Screenshot needed:` wording.
- Help copy uses simple wording and American spelling.

## Detail Expansion Notes

After the initial implementation, the help centers were expanded with additional detail sections:

- Customer Help added states and policies, troubleshooting, glossary, review checklists, and FAQ.
- Teams Help added access and permissions, troubleshooting, glossary, review checklists, and FAQ.
- Spaces Help added payment and refund safety, troubleshooting, glossary, review checklists, and FAQ.

These sections keep detailed guidance easy to scan instead of placing everything inside the task guides.

## Five Additional Expansion Iterations

The help centers were then expanded through five additional detail passes:

1. Added page references and practical examples for Customer, Teams, and Spaces.
2. Added action-reference pages for common customer, admin, and operator actions.
3. Added support handoff pages with safe templates and triage notes.
4. Added screenshot capture plans for each help app.
5. Added review QA matrices for product, support, engineering, and copy review.

## Detail Expansion Verification

| Check | Result | Notes |
| --- | --- | --- |
| Expanded MDX word count | 16,469 words | Across 36 help content pages. |
| `pnpm --dir src/web/apps/webapp-help lint` | Passed | No ESLint output after expansion. |
| `pnpm --dir src/web/apps/webapp-teams-help lint` | Passed | No ESLint output after expansion. |
| `pnpm --dir src/web/apps/webapp-spaces-help lint` | Passed | No ESLint output after expansion. |
| `pnpm --dir src/web/apps/webapp-help build` | Passed | Generated 16 static pages. Nextra warned about missing Git timestamps for new uncommitted MDX files. |
| `pnpm --dir src/web/apps/webapp-teams-help build` | Passed | Generated 16 static pages. Nextra warned about missing Git timestamps for new uncommitted MDX files. |
| `pnpm --dir src/web/apps/webapp-spaces-help build` | Passed | Generated 16 static pages. Nextra warned about missing Git timestamps for new uncommitted MDX files. |

## Five-Iteration Expansion Verification

| Check | Result | Notes |
| --- | --- | --- |
| Expanded MDX word count | 23,691 words | Across 57 help content files. |
| `pnpm --dir src/web/apps/webapp-help lint` | Passed | No ESLint output after five-iteration expansion. |
| `pnpm --dir src/web/apps/webapp-teams-help lint` | Passed | No ESLint output after five-iteration expansion. |
| `pnpm --dir src/web/apps/webapp-spaces-help lint` | Passed | No ESLint output after five-iteration expansion. |
| `pnpm --dir src/web/apps/webapp-help build` | Passed | Generated 22 static pages. Nextra warned about missing Git timestamps for new uncommitted MDX files. |
| `pnpm --dir src/web/apps/webapp-teams-help build` | Passed | Generated 22 static pages. Nextra warned about missing Git timestamps for new uncommitted MDX files. |
| `pnpm --dir src/web/apps/webapp-spaces-help build` | Passed | Generated 22 static pages. Nextra warned about missing Git timestamps for new uncommitted MDX files. |
