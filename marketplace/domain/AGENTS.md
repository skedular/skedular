# Marketplace Domain Test Agent Notes

This file covers `marketplace/domain/`.

## Agent Rule

- Use this area for marketplace-domain integration tests.
- For marketplace plus booking or organization end-to-end scenarios, use system tests.
- Keep marketplace Aspire dependency readiness in `Marketplace.Domain.AppHost/AppHost.cs`; referenced resources should
  be paired with `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.
