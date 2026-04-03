# Organization Domain Test Agent Notes

This file covers `organization/domain/`.

## Agent Rule

- Use this area for organization-domain integration tests.
- For booking-and-organization billing scenarios, prefer system tests.
- Xero token-refresh logic and org Xero maintenance rules should usually be covered with shared-layer unit tests first.
  Use organization-domain integration tests only when you need the real host wiring.
