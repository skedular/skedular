# skedular Development Guidelines

Auto-generated from all feature plans. Last updated: 2026-06-04

## Active Technologies

- GitHub Actions YAML on `ubuntu-latest`; Bash shell in existing Skedular composite actions; Terraform HCL for infrastructure workspaces; Dockerfile-based app builds + GitHub Actions, `actions/checkout@v6`, Docker BuildKit, `docker/metadata-action`, `docker/login-action`, `docker/build-push-action`, `hashicorp/setup-terraform@v4`, `actions/github-script`, existing `.github/actions/build-test-push`, `.github/actions/lint-validate-infrastructure`, `.github/actions/deploy-infrastructure` (024-merge-ci-cd-pipelines)
- N/A for application data; workflow-local changed-file lists, coverage outputs, Docker image tags, and Terraform state backends already configured by current workspaces (024-merge-ci-cd-pipelines)

- TypeScript 6.0.3, Astro static site, Node.js 22 via `src/web` workspace + Astro, TypeScript, pnpm 11.5.1 workspace, Turborepo 2.9.x; app-local formatting/lint tooling consistent with current web apps (023-astro-public-website)
- N/A - static website source files only (023-astro-public-website)

- TypeScript 6.0.3; React 19.2.6; Next.js 16.2.6 App Router; backend C# .NET 10 only if GraphQL/domain contract changes are needed + Relay 21, `react-relay`, MUI 9, `@skedular/ui`, `@skedular/shared`, WorkOS AuthKit, Leaflet/react-leaflet for map browsing, existing marketplace GraphQL schema and generated Relay artifacts (020-customer-landing-cleanup)
- No new persistence planned for the first cleanup/design slice; uses existing marketplace booking, subscription, location, organization, and customer data via GraphQL. Any new durable cleanup inventory can start as feature documentation/task artifact unless implementation requires product-owned persistence. (020-customer-landing-cleanup)

- C# .NET 10 (backend); TypeScript 6 / React 19 / Next.js 16 App Router (frontend) (007-resource-availability-dashboard)
- PostgreSQL via EF Core — no new migration required for the query path; the existing `DailyResourceAvailabilitySnapshot` table (from 006) supports analytics but the dashboard queries live booking data at request time via gRPC (007-resource-availability-dashboard)
- C# .NET 10 (backend); TypeScript 6 / React 19 / Next.js 16 App Router (frontend) + HotChocolate (GraphQL), Entity Framework Core, `Enterprise.Shared.Database` repository pattern, Relay, MUI v9, `mui-rff` Autocomplete (008-bulk-resource-import)
- PostgreSQL via EF Core — no new migration; reuses existing `Resource` and `OrganizationTagResource` tables (008-bulk-resource-import)

- C# .NET 10 (backend), TypeScript 6 / React 19 / Next.js 16 App Router (frontend) + HotChocolate (GraphQL), Entity Framework Core, Relay, MUI v9, `mui-rff` Autocomplete, `useSearchParams`/`useRouter` (Next.js) (005-subscription-landing-page-filter)
- PostgreSQL — no new migrations; filtering via existing indexed `Status` and `MarketplaceBooking.PaymentStatus` columns (005-subscription-landing-page-filter)
- C# on .NET 10 + Temporal (workflows/activities), HotChocolate (GraphQL), Entity Framework Core, gRPC (booking data via `BookingService.BookingServiceClient`), `Enterprise.Shared.Database` repository pattern, `IRepositoryFactory`, `IWorkflowIdService` (006-desk-availability-analytics)
- PostgreSQL via EF Core — new `DailyDeskAvailabilitySnapshot` table; new migration required in `src/location/shared/Location.Shared/Database/Migrations/` (006-desk-availability-analytics)

- TypeScript (Next.js web apps), Terraform HCL + `next`, `react`, `@skedular/ui`, Terraform AWS/Vercel/Google providers (002-split-ui-products)
- S3 Terraform backend + DynamoDB locking (per workspace state key) (002-split-ui-products)
- C# on .NET 10 + Entity Framework Core, `Enterprise.Shared.Database` repository bases, domain shared repository factories, HotChocolate pagination helpers, `Microsoft.Extensions.Logging` (003-remove-shared-specification)
- PostgreSQL via EF Core domain DbContexts (003-remove-shared-specification)
- TypeScript 6, React 19 + Next.js 16 (App Router), Relay, MUI v9, pnpm workspaces, Turborepo (004-modularize-webapp-products)
- N/A — frontend only (004-modularize-webapp-products)

- C# on .NET 10 + `Microsoft.Extensions.Logging`, Enterprise.Shared logging/hosting extensions, (001-team-domain-logging)

## Project Structure

```text
src/
	team/
		apis/
		shared/
		processors/
```

## Commands

# Add commands for C# on .NET 10

## Code Style

C# on .NET 10: Follow standard conventions

## Recent Changes

- 024-merge-ci-cd-pipelines: Added GitHub Actions YAML on `ubuntu-latest`; Bash shell in existing Skedular composite actions; Terraform HCL for infrastructure workspaces; Dockerfile-based app builds + GitHub Actions, `actions/checkout@v6`, Docker BuildKit, `docker/metadata-action`, `docker/login-action`, `docker/build-push-action`, `hashicorp/setup-terraform@v4`, `actions/github-script`, existing `.github/actions/build-test-push`, `.github/actions/lint-validate-infrastructure`, `.github/actions/deploy-infrastructure`

- 023-astro-public-website: Added TypeScript 6.0.3, Astro static site, Node.js 22 via `src/web` workspace + Astro, TypeScript, pnpm 11.5.1 workspace, Turborepo 2.9.x; app-local formatting/lint tooling consistent with current web apps

- 020-customer-landing-cleanup: Added TypeScript 6.0.3; React 19.2.6; Next.js 16.2.6 App Router; backend C# .NET 10 only if GraphQL/domain contract changes are needed + Relay 21, `react-relay`, MUI 9, `@skedular/ui`, `@skedular/shared`, WorkOS AuthKit, Leaflet/react-leaflet for map browsing, existing marketplace GraphQL schema and generated Relay artifacts

<!-- MANUAL ADDITIONS START -->

## Web Package Boundaries (004-modularize-webapp-products)

- **`@skedular/ui`** (`src/web/packages/ui/`): Design system. Typography wrappers, layout primitives, commons components, theme. Must NEVER import from `@skedular/shared`.
- **`@skedular/shared`** (`src/web/packages/shared/`): Shared runtime. Providers, hooks, utils, cookie-consent, MUI helpers, image uploaders. MAY import from `@skedular/ui`.
- **Typography Rule**: Use `@skedular/ui` wrappers (e.g. `BodyIconTypography`, `SmallIconTypography`) — never `@mui/material/Typography` directly in app or page components.
- **Webapp**: Imports both packages. `@/libs/providers/` only contains `integrated-platform-hook.tsx` (MS Teams, deferred).
<!-- MANUAL ADDITIONS END -->

<!-- SPECKIT START -->

For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan

<!-- SPECKIT END -->
