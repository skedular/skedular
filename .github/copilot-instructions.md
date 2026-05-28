# unityhubio Development Guidelines

Auto-generated from all feature plans. Last updated: 2026-05-13

## Active Technologies

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

- 008-bulk-resource-import: Added C# .NET 10 (backend); TypeScript 6 / React 19 / Next.js 16 App Router (frontend) + HotChocolate (GraphQL), Entity Framework Core, `Enterprise.Shared.Database` repository pattern, Relay, MUI v9, `mui-rff` Autocomplete
- 007-resource-availability-dashboard: Added C# .NET 10 (backend); TypeScript 6 / React 19 / Next.js 16 App Router (frontend)

- 006-desk-availability-analytics: Added C# on .NET 10 + Temporal (workflows/activities), HotChocolate (GraphQL), Entity Framework Core, gRPC (booking data via `BookingService.BookingServiceClient`), `Enterprise.Shared.Database` repository pattern, `IRepositoryFactory`, `IWorkflowIdService`

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
at specs/017-move-sources-to-src/plan.md

<!-- SPECKIT END -->
