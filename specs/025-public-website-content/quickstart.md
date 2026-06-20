# Quickstart: Public Website Content Integration

## Prerequisites

- Node.js 22
- pnpm 11.5.1
- Public destination URL variables for local validation:

```bash
export PUBLIC_SKEDULAR_APP_URL=https://app.example.test
export PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up
export PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book
```

## Install

From the repository root:

```bash
pnpm --dir src/web install
```

## Run Locally

```bash
PUBLIC_SKEDULAR_APP_URL=https://app.example.test \
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up \
PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book \
pnpm --dir src/web/apps/public-web dev
```

Expected local URL:

```text
http://localhost:15008
```

## App-Local Validation

```bash
PUBLIC_SKEDULAR_APP_URL=https://app.example.test \
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up \
PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book \
pnpm --dir src/web/apps/public-web test
```

```bash
PUBLIC_SKEDULAR_APP_URL=https://app.example.test \
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up \
PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book \
pnpm --dir src/web/apps/public-web check
```

```bash
pnpm --dir src/web/apps/public-web lint
```

```bash
PUBLIC_SKEDULAR_APP_URL=https://app.example.test \
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up \
PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book \
pnpm --dir src/web/apps/public-web build
```

## Workspace Validation

From repository root:

```bash
pnpm --dir src/web check:workspace-version-sync
```

```bash
PUBLIC_SKEDULAR_APP_URL=https://app.example.test \
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up \
PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book \
pnpm --dir src/web build
```

```bash
PUBLIC_SKEDULAR_APP_URL=https://app.example.test \
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up \
PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book \
pnpm --dir src/web test
```

```bash
pnpm --dir src/web lint
```

## Required Manual Review

Review before launch:

- Every heading and major bullet group in `src/web/apps/public-web/public-website-content-draft.md` has a coverage decision.
- Every current public blog/support URL has a published destination or redirect to a published replacement.
- Every draft comparison page is published with neutral, reviewed competitor claims.
- Pricing page includes the draft Teams tiers, Spaces tiers, public booking terms, and host commission range.
- All public outbound app/search, login/sign-up, and demo/contact URLs come from the three required environment variables.
- Copy reads friendly, professional, human-written, and specific to Skedular's audience.
- User-facing copy uses American spelling and grammar.
- Public pages do not expose private metadata, access instructions, unreleased roadmap commitments, or sensitive values.

## Deployment Review

For each staging/production deployment:

- Confirm environment-specific values are supplied for:
  - `PUBLIC_SKEDULAR_APP_URL`
  - `PUBLIC_SKEDULAR_SIGNUP_URL`
  - `PUBLIC_SKEDULAR_DEMO_URL`
- Confirm build output does not log full destination URL values.
- Confirm route redirects for migrated current public URLs.
- Confirm deployed Lighthouse/Core Web Vitals targets where a deployed URL is available.
