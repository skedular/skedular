# unityhubio Development Guidelines

Auto-generated from all feature plans. Last updated: 2026-04-19

## Active Technologies
- TypeScript (Next.js web apps), Terraform HCL + `next`, `react`, `@skedular/ui`, Terraform AWS/Vercel/Google providers (002-split-ui-products)
- S3 Terraform backend + DynamoDB locking (per workspace state key) (002-split-ui-products)
- C# on .NET 10 + Entity Framework Core, `Enterprise.Shared.Database` repository bases, domain shared repository factories, HotChocolate pagination helpers, `Microsoft.Extensions.Logging` (003-remove-shared-specification)
- PostgreSQL via EF Core domain DbContexts (003-remove-shared-specification)

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
- 003-remove-shared-specification: Added C# on .NET 10 + Entity Framework Core, `Enterprise.Shared.Database` repository bases, domain shared repository factories, HotChocolate pagination helpers, `Microsoft.Extensions.Logging`
- 002-split-ui-products: Added TypeScript (Next.js web apps), Terraform HCL + `next`, `react`, `@skedular/ui`, Terraform AWS/Vercel/Google providers

- 001-team-domain-logging: Added C# on .NET 10 + `Microsoft.Extensions.Logging`, Enterprise.Shared logging/hosting extensions,

<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->
