# Documentation Agent Notes

This file applies to `docs/`.

## Purpose

- `docs/` contains architecture documentation, ADRs (Architecture Decision Records), event catalog, and infrastructure
  documentation for the Skedular platform.

## Contents

| File/Directory            | Purpose                                                              |
|---------------------------|----------------------------------------------------------------------|
| `codebase-overview.md`    | High-level overview of the Skedular codebase and repository layout   |
| `adr-index.md`            | Index of all Architecture Decision Records                           |
| `adr-event-catalog.md`    | ADR and catalog of domain events                                     |
| `architecture/`           | Architecture diagrams and documentation                              |
| `event-catalog/`          | Detailed event catalog for Kafka domain events                       |
| `images/`                 | Images used in documentation                                         |
| `infrastructure/`         | Infrastructure-specific documentation                                |
| `sso-integration.md`      | SSO integration documentation                                        |

## Agent Rule

- Keep documentation accurate and up to date when making code changes that affect architecture, events, or APIs.
- ADRs are append-only historical records; do not modify existing ADRs.
- If a new event is added to `api-definitions/events/`, update the event catalog.
- If an architectural decision is made during a significant change, consider adding an ADR.
