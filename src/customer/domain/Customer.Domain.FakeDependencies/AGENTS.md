# Customer Domain Fake Dependencies Notes

This file applies to `customer/domain/Customer.Domain.FakeDependencies`.

## Purpose

- This project hosts customer-owned fake external dependencies for customer-domain integration tests.
- Keep it as an Aspire-hosted executable so the customer domain can talk to it over real network boundaries later.
- It is currently scaffold-only and does not host domain-specific fake APIs yet.

## Boundary

- Put customer-specific fake dependency behavior and scenarios here when integration tests need them.
- Keep generic test helpers in `shared/Testing.Shared.IntegrationTests`.
- Keep generic local infrastructure bootstrapping in `shared/Infrastructure.Shared`.
