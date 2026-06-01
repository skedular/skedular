# Data Model: Help Webapps Documentation

## Source Inventory

Represents the reviewed source material used to decide what help content is required.

**Fields**

- `app`: Customer, Teams, or Spaces
- `sourceType`: spec, route, root page, navigation area, form, status, component state, existing help page
- `sourcePath`: repository-relative path or spec reference
- `workflowName`: plain-language name of the workflow or state
- `audience`: customer, private organization admin, marketplace operator, support reviewer, or mixed
- `ownership`: Customer, Teams, Spaces, shared, out of scope, or content gap
- `riskLevel`: normal, sensitive, unclear, transitional
- `notes`: concise explanation of why it is included or excluded

**Validation Rules**

- Every inventory item must have exactly one `ownership`.
- Items marked `unclear` or `transitional` must map to a content gap unless reviewed and approved.
- Sensitive items must not expose secrets or internal-only instructions in public help.

## Help Webapp

Represents one public help center.

**Fields**

- `name`: Customer help, Teams help, or Spaces help
- `purposeStatement`: one short explanation of what the app is for
- `audience`: primary readers for this help app
- `productBoundaries`: what belongs in this app and what belongs elsewhere
- `topicGroups`: ordered list of product areas
- `publicAccess`: always true for this feature

**Relationships**

- Contains many Help Topics.
- Contains many Task Guides.
- Contains many Content Gaps.

**Validation Rules**

- Must be readable without sign-in.
- Must explain when a reader should use another help app.
- Must avoid sensitive customer, payment, security, integration, and internal operator details.

## Help Topic

Represents a concept or product area page.

**Fields**

- `title`: plain-language topic title
- `app`: owning help app
- `productArea`: group such as bookings, subscriptions, locations, resources, teams, products, refunds, payments, integrations, settings, or analytics
- `audience`: intended reader
- `purpose`: what the topic helps the reader understand
- `commonTasks`: common workflows linked from the topic
- `importantStates`: relevant statuses or states
- `ownershipBoundary`: what this app owns and what belongs elsewhere
- `sourceReferences`: inventory items supporting the topic

**Relationships**

- Belongs to one Help Webapp.
- May link to many Task Guides.
- May reference many Source Inventory items.

**Validation Rules**

- Must use simple, human wording.
- Must include ownership guidance when similar concepts appear in other apps.
- Must not use generic marketing language or placeholder text.

## Task Guide

Represents a step-by-step guide for a workflow.

**Fields**

- `title`: plain-language guide title
- `app`: owning help app
- `workflow`: route, detail page, form, status, or major component state covered
- `startingPoint`: where the user begins
- `steps`: ordered user-facing steps
- `expectedResult`: what the user should see or accomplish
- `importantStates`: statuses or branches that affect the guide
- `screenshotPlaceholders`: zero or more screenshot placeholder anchors
- `sourceReferences`: inventory items supporting the guide

**Relationships**

- Belongs to one Help Webapp.
- Usually belongs under one Help Topic.
- References one or more Source Inventory items.

**Validation Rules**

- Must not describe best guesses.
- Must include a screenshot placeholder when visual guidance is needed.
- Must state when a workflow depends on policy, configuration, permissions, or product state.
- Must route unclear or risky branches to Content Gaps.

## Screenshot Placeholder

Represents a planned screenshot location.

**Fields**

- `label`: short name for the future screenshot
- `guide`: owning Task Guide
- `captureTarget`: page, form, state, or step to capture later
- `captionDraft`: plain-language caption explaining what the future image should show
- `status`: placeholder, captured, or not needed

**Validation Rules**

- Placeholder text must be explicit enough for later capture.
- Placeholder text is the only accepted placeholder content in drafted help pages.

## Content Gap

Represents a workflow or detail that cannot be safely documented yet.

**Fields**

- `title`: plain-language gap title
- `app`: owning help app
- `sourceReference`: inventory item or missing source that caused the gap
- `reason`: unclear, risky, transitional, insufficient source, sensitive, or out of scope
- `neededReview`: product, engineering, support, security, or screenshot capture
- `expectedResolution`: what must happen before help can be completed

**Relationships**

- Belongs to one Help Webapp.
- May be linked from a Help Topic or Task Guide.

**Validation Rules**

- Must be used instead of guessed help content.
- Must be visible in the first-version help inventory.

## Review Checklist

Represents the acceptance checks for the completed help content.

**Fields**

- `coverageComplete`: whether every inventory item maps to content, out-of-scope, or gap
- `plainLanguage`: whether pages use simple headings and clear writing
- `publicSafety`: whether sensitive details are avoided
- `appBoundaryAccuracy`: whether content points to the correct app
- `screenshotPlaceholdersComplete`: whether needed screenshot placeholders are present
- `americanEnglish`: whether copy follows American spelling and grammar

**Validation Rules**

- Product, engineering, support, and security-sensitive review must pass before implementation is accepted.

## State Transitions

### Inventory Item

```text
Discovered -> Classified -> Covered
                         -> Out of scope
                         -> Content gap
```

### Task Guide

```text
Planned -> Drafted -> Reviewed -> Accepted
                 -> Content gap
```

### Screenshot Placeholder

```text
Placeholder -> Captured -> Accepted
Placeholder -> Not needed
```

### Content Gap

```text
Open -> Reviewed -> Resolved
Open -> Deferred
```
