# Quickstart: Help Webapps Documentation

## 1. Confirm Feature Context

From the repository root:

```bash
git status --short --branch
cat .specify/feature.json
```

Expected feature directory:

```text
specs/021-help-webapps-docs
```

## 2. Review Source Inventory Inputs

Use the spec and product split docs as the first source set:

```bash
sed -n '1,260p' specs/021-help-webapps-docs/spec.md
sed -n '1,220p' specs/009-split-web-products/spec.md
sed -n '1,220p' specs/020-customer-landing-cleanup/spec.md
```

Review the existing help shells:

```bash
find src/web/apps/webapp-help/src src/web/apps/webapp-teams-help/src src/web/apps/webapp-spaces-help/src -maxdepth 4 -type f | sort
```

Review product route and root page surfaces:

```bash
find src/web/apps/webapp/src/app src/web/apps/webapp/src/rootPages -maxdepth 4 -type f | sort
find src/web/apps/webapp-teams/src/app src/web/apps/webapp-teams/src/rootPages -maxdepth 4 -type f | sort
find src/web/apps/webapp-spaces/src/app src/web/apps/webapp-spaces/src/rootPages -maxdepth 4 -type f | sort
```

## 3. Build the Coverage Inventory

For each app, map every route, detail page, form, status, and major component state to one of:

- help topic
- step-by-step guide
- out-of-scope decision
- content gap

Do not guess unclear flows. Mark them as content gaps.

## 4. Draft Help Content

Use the existing help app structure:

```text
src/web/apps/webapp-help/src/content/
src/web/apps/webapp-teams-help/src/content/
src/web/apps/webapp-spaces-help/src/content/
```

For each app:

- update the app home page
- create topic pages
- create task guides
- update `_meta.ts`
- include screenshot placeholders where needed
- include a visible content-gap page or section

## 5. Review Public Safety

Check that public help does not expose:

- customer personal data
- payment secrets
- security configuration details
- integration secrets
- internal operator-only procedures
- sensitive billing or organization internals

## 6. Verify

Run lint for each help app:

```bash
pnpm --dir src/web/apps/webapp-help lint
pnpm --dir src/web/apps/webapp-teams-help lint
pnpm --dir src/web/apps/webapp-spaces-help lint
```

Run builds for each help app:

```bash
pnpm --dir src/web/apps/webapp-help build
pnpm --dir src/web/apps/webapp-teams-help build
pnpm --dir src/web/apps/webapp-spaces-help build
```

If build fails because sandboxing blocks Turbopack process or port behavior, rerun with the required approval or record the sandbox failure separately.

## 7. Manual Review Checklist

Reviewers should confirm:

- each help app explains what it is for
- every inventory item is mapped to content, out-of-scope, or content gap
- topic pages explain concepts clearly
- task guides include steps and expected results
- screenshot placeholders exist where needed
- content gaps are explicit
- public safety rules pass
- American spelling and grammar are used
- no product app behavior changed

## 8. Review Notes

Record run-specific lint, build, reader-review, public-access, and static-doc diagnostics notes in:

```text
specs/021-help-webapps-docs/source-inventory/review-notes.md
```
