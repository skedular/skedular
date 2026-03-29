# Organization Domain Agent Notes

This file is the entry point for AI agents working in `organization/`.

## Purpose

- `organization/` owns organization configuration and finance-adjacent settings used heavily by other domains.

## Important Cross-Domain Relevance

Other domains often depend on organization state for:

- billing cycle
- tax configuration
- bank accounts
- Stripe connect accounts

## Where To Read Next

- `organization/apis/AGENTS.md`
- `organization/domain/AGENTS.md`
- `organization/shared/AGENTS.md`

## Agent Rule

- Changes here can create billing regressions elsewhere, especially in booking and marketplace.
