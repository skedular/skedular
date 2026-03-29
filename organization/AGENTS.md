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
- precomputed organization analytics and usage-related state
- replicated authorization state such as organization membership and related access checks

## Where To Read Next

- `organization/apis/AGENTS.md`
- `organization/domain/AGENTS.md`
- `organization/shared/AGENTS.md`

## Booking-Derived State

- `organization/` no longer stores replicated booking rows.
- `organization/` no longer exposes or persists a local `HasFutureBooking` concept.
- If some future feature needs to know whether an organization has future bookings, ask the booking domain directly instead of rebuilding the old local flag.
- Organization analytics and booking-derived usage snapshots are precomputed locally from booking-owned source data.
- Booking events should be treated as invalidation/recompute triggers, not as payloads for organization-side booking persistence.

## Replication Boundary

- Cross-domain replication is still allowed when the replicated data is needed for authorization or membership-aware access decisions.
- In practice, organization, organization members, customer, and customer identity are expected to remain replicated across domains when those domains enforce local access rules.
- The removal target is booking-derived or other passive denormalized state that is not required for authorization, routing, or ownership decisions.

## Agent Rule

- Changes here can create billing regressions elsewhere, especially in booking and marketplace.
