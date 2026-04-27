# unityhubio Development Guidelines

Auto-generated from all feature plans. Last updated: 2026-04-27

## Active Technologies

- TypeScript (Next.js web apps), Terraform HCL + `next`, `react`, `@skedular/ui`, Terraform AWS/Vercel/Google providers (002-split-ui-products)
- S3 Terraform backend + DynamoDB locking (per workspace state key) (002-split-ui-products)
- C# on .NET 10 + Entity Framework Core, `Enterprise.Shared.Database` repository bases, domain shared repository factories, HotChocolate pagination helpers, `Microsoft.Extensions.Logging` (003-remove-shared-specification)
- PostgreSQL via EF Core domain DbContexts (003-remove-shared-specification)
- TypeScript 6, React 19 + Next.js 16 (App Router), Relay, MUI v9, pnpm workspaces, Turborepo (004-modularize-webapp-products)
- N/A — frontend only (004-modularize-webapp-products)

- C# on .NET 10 + `Microsoft.Extensions.Logging`, Enterprise.Shared logging/hosting extensions, (001-team-domain-logging)

## Project Structure

```text
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

- 004-modularize-webapp-products: Added TypeScript 6, React 19 + Next.js 16 (App Router), Relay, MUI v9, pnpm workspaces, Turborepo
- 003-remove-shared-specification: Added C# on .NET 10 + Entity Framework Core, `Enterprise.Shared.Database` repository bases, domain shared repository factories, HotChocolate pagination helpers, `Microsoft.Extensions.Logging`
- 002-split-ui-products: Added TypeScript (Next.js web apps), Terraform HCL + `next`, `react`, `@skedular/ui`, Terraform AWS/Vercel/Google providers

<!-- MANUAL ADDITIONS START -->

## Web Package Boundaries (004-modularize-webapp-products)

- **`@skedular/ui`** (`web/packages/ui/`): Design system. Typography wrappers, layout primitives, commons components, theme. Must NEVER import from `@skedular/shared`.
- **`@skedular/shared`** (`web/packages/shared/`): Shared runtime. Providers, hooks, utils, cookie-consent, MUI helpers, image uploaders. MAY import from `@skedular/ui`.
- **Typography Rule**: Use `@skedular/ui` wrappers (e.g. `BodyIconTypography`, `SmallIconTypography`) — never `@mui/material/Typography` directly in app or page components.
- **Webapp**: Imports both packages. `@/libs/providers/` only contains `integrated-platform-hook.tsx` (MS Teams, deferred).
<!-- MANUAL ADDITIONS END -->
