# Customer Domain Agent Notes

This file is the entry point for AI agents working in `customer/`.

## Purpose

- `customer/` owns customer-facing identity and customer domain state used by other domains.

## Replication Boundary

- Customer and customer-identity data are intentionally replicated into other domains because many domains perform local authorization and membership-aware access checks.
- Changes to customer shape, identity semantics, or replication assumptions can break authorization behavior outside `customer/`.

## Where To Read Next

- `customer/apis/AGENTS.md`
- `customer/domain/AGENTS.md`
- `customer/shared/AGENTS.md`

## Agent Rule

- Be careful with changes to customer identity or lookup behavior because other domains depend on it.
