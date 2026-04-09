# Skedular System Tests Notes

This file applies to `system/Skedular.SystemTests`.

## Purpose

- End-to-end system tests that drive the entire running platform through real API clients.
- No fake services are used; all domain APIs, workers, and infrastructure run as real processes via Aspire.

## When To Add Tests Here

- Add tests here when the scenario requires real cross-domain interactions, real Temporal workflows, or real Kafka event
  flow between domains.
- For single-domain scenarios, prefer the respective `{Domain}.Domain.IntegrationTests` project.

## Test Structure

- Tests drive the system through real API clients generated from the gateway schema.
- Do not instantiate domain internal services directly in these tests.
- Assert outcomes through API responses, persisted state read via API queries, or observable side effects.
- Tests are organized under `Gateway/` by the GraphQL surface they exercise.

## Agent Rule

- Keep tests scenario-driven and API-surface-oriented.
- Do not bypass the API surface by accessing internal domain services directly.
- Read the parent `system/AGENTS.md` for overall system test rules.
