# Implementation Plan: Persisted Duration Display Units

**Branch**: `043-duration-display-units` | **Date**: 2026-08-21 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/043-duration-display-units/spec.md`

## Summary

Persist the selected display unit for every persisted, user-editable minute-based configuration discovered by a repository-wide audit. Keep existing minute values and conversion/rounding unchanged. Start with Marketplace pricing, then extend the shared duration editor and every identified Host/Spaces or other domain editor. The research inventory is a required design artifact and implementation scope control.

## Technical Context

**Language/Version**: C#/.NET 10; TypeScript 6; React 19; Next.js 16  
**Primary Dependencies**: Existing Marketplace models/events/GraphQL, `@skedular/ui` duration input, Relay-generated clients, Host and Spaces editors  
**Storage**: Existing persisted configuration boundaries owned by each applicable domain; no standalone preference store and no display-unit replication across domains
**Testing**: C# unit/integration tests; Vitest and React Testing Library; GraphQL schema/integration validation  
**Target Platform**: Skedular backend services and web applications  
**Project Type**: Multi-domain web application and service monorepo  
**Performance Goals**: No additional per-keystroke network or logging overhead; existing editor responsiveness preserved  
**Constraints**: Display metadata is optional, additive, presentation-only, and must not alter canonical minute values or calculations  
**Scale/Scope**: All repository occurrences of persisted, user-editable minute/hour configuration, with Marketplace as the first confirmed domain

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — Additive source contract/model/event changes use owning definitions first; regenerate GraphQL, event, and Relay artifacts through repository scripts. No generated schema is hand-edited.
- [x] **II. Domain Boundaries** — Each owning domain stores its own display metadata. Other domains receive no display-unit replication unless a concrete independent need is proven; canonical cross-domain data continues through existing public models/events/contracts. Persisted display-unit values use explicit source/model mappings and never reflection parsing.
- [x] **III. Testing** — Unit tests cover conversion and mapping first; focused integration/schema tests cover JSON compatibility and contract wiring. No raw EF assertions or duplicated end-to-end scenarios.
- [x] **IV. Frontend** — Reuse the shared `@skedular/ui` duration input, update Host/Spaces editor state and Relay operations, regenerate artifacts, avoid reloads, use American English, and record that no public customer documentation change is expected because this is editor-only metadata.
- [x] **V. Pattern Consistency** — Metadata follows existing persisted JSON and shared enum/detail mapping patterns; no separate preference store is introduced.
- [x] **VI. Logging** — Add only actionable warnings/errors for invalid metadata or contract/persistence failures; do not log routine unit changes or per-keystroke edits.

## Project Structure

```text
api-definitions/
├── events/skedular/marketplace_v1_value.proto
└── graphql/skedular/v1/
src/shared/Api.Shared.Services/Models/
src/marketplace/shared/Marketplace.Shared/
src/marketplace/apis/Marketplace.Api/
src/web/packages/ui/src/duration-input.tsx
src/web/apps/webapp-host/src/components/
src/web/apps/webapp-spaces/src/components/
specs/043-duration-display-units/
```

**Structure Decision**: Complete the repository-wide audit as a blocking gate, then keep canonical duration models and display metadata inside each owning domain’s persistence and same-domain GraphQL contract. Do not replicate display metadata across domains. Centralize display conversion in the shared UI duration input and update every editor identified by the research inventory. Record excluded occurrences in `research.md` rather than adding metadata to operational values.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|---|---|---|
| None | The repository-wide audit expands touched domains/editors, but follows existing ownership boundaries and shared UI patterns. | A Marketplace-only change would leave equivalent persisted editor fields inconsistent. |
