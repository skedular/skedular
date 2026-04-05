# MsTeams Domain Agent Notes

This file is the entry point for AI agents working in `msteams/`.

## Purpose

- `msteams/` owns Microsoft Teams integration behavior.

## Replication Boundary

- `msteams/` intentionally replicates organization, location, and team state for Azure-tenant and Teams-channel routing.
- Those replicas are used to decide which tenant/team/channel receives organization, location, or team updates.
- `msteams/` also relies on replicated auth-related state where needed; do not remove those replicas unless the routing
  model is redesigned first.
- Workflow-driven rebuilds are only candidates for derived projections that are neither auth-critical nor
  routing-critical.

## Where To Read Next

- `msteams/apis/AGENTS.md`
- `msteams/domain/AGENTS.md`
- `msteams/shared/AGENTS.md`

## Agent Rule

- Treat external integration contracts carefully and prefer behavior-preserving changes.
