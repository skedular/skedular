# Marketplace Domain Agent Notes

This file is the entry point for AI agents working in `marketplace/`.

## Purpose

- `marketplace/` owns listing, catalog, and marketplace-facing purchase flows that connect closely with booking and
  organization.

## Replication Boundary

- `marketplace/` should preserve replicated organization, organization-member, customer, and customer-identity state
  when that state is part of local authorization or ownership checks.
- Product or listing projections may still be candidates for workflow-driven rebuilds, but auth-critical replicas are
  not cleanup targets by default.

## Where To Read Next

- `marketplace/apis/AGENTS.md`
- `marketplace/domain/AGENTS.md`
- `marketplace/shared/AGENTS.md`

## Agent Rule

- Marketplace changes often affect booking, pricing, and checkout assumptions. Check cross-domain effects.
