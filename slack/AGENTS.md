# Slack Domain Agent Notes

This file is the entry point for AI agents working in `slack/`.

## Purpose

- `slack/` owns Slack integration behavior.

## Replication Boundary

- `slack/` intentionally replicates organization, location, and team state for workspace/channel routing.
- Those replicas are used to decide which Slack workspace/channel receives organization, location, or team updates, including daily attendance-style posts.
- `slack/` also relies on replicated auth-related state where needed; do not remove those replicas unless the routing model is redesigned first.
- Workflow-driven rebuilds are only candidates for derived projections that are neither auth-critical nor routing-critical.

## Where To Read Next

- `slack/apis/AGENTS.md`
- `slack/domain/AGENTS.md`
- `slack/shared/AGENTS.md`

## Agent Rule

- Prefer safe, contract-preserving changes over refactors for style alone.
