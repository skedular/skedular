# Customer Domain Test Agent Notes

This file covers `customer/domain/`.

## Agent Rule

- Use constructor injection and existing wiring patterns.
- Prefer system tests for cross-domain scenarios instead of faking neighbors here.
- Keep customer Aspire dependency readiness in `Customer.Domain.AppHost/AppHost.cs`; referenced resources should be
  paired with `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.
