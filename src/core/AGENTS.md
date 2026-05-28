# Core Domain Agent Notes

This file is the entry point for AI agents working in `core/`.

## Purpose

- `core/` is the shared platform domain behind cross-cutting capabilities used by other domains.

## Replication Boundary

- `core/` should assume that replicated organization, organization-member, customer, and customer-identity state may be
  required for local authorization decisions.
- Do not remove auth-critical replicas in `core/` without a broader authorization redesign.

## Where To Read Next

- `core/apis/AGENTS.md`
- `core/domain/AGENTS.md`
- `core/shared/AGENTS.md`

## Agent Rule

- Changes here can affect many domains. Prefer small edits and verify downstream assumptions.
