# Implementation Plan: Skedualr Host App

**Branch**: `026-scheduler-host-app` | **Date**: 2026-06-28 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/026-scheduler-host-app/spec.md`

## Summary

This feature adds a new web application ("Skedualr Host") for individuals who want to rent out their physical/virtual spaces. The Host app is a presentation-layer abstraction on top of the existing Skedualr Spaces booking engine, designed to simplify the listing process for individual hosts (Airbnb-style). Key aspects:

- New "Host" organization type (alongside Private and Marketplace)
- Host creates Locations and Products; system auto-creates underlying Resources
- Full-place booking only (entire Location reserved, not sub-resources)
- 5% commission on booking value charged to Hosts
- Listings appear on the same map as Spaces with a distinct badge

## Technical Context

**Language/Version**: C# .NET 10 (backend); TypeScript 6 / React 19 / Next.js 16 App Router (frontend)  
**Primary Dependencies**: MUI, Relay (GraphQL), EF Core PostgreSQL, Kafka, Temporal  
**Storage**: PostgreSQL via EF Core — existing `Location`, `Product`, `Resource` models reused  
**Testing**: Unit tests (xUnit/NUnit), integration tests (repository-layer assertions), E2E (Playwright)  
**Target Platform**: Web application (Next.js App Router)
**Project Type**: Web application (frontend + backend)
**Performance Goals**: Map load time <2s, listing pages <1s p95, booking API <500ms p95  
**Constraints**: Reuse existing booking engine — no new booking logic; Host never sees Resources directly  
**Scale/Scope**: 10k+ concurrent hosts, unlimited Locations per host, same map performance as Spaces

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — Does this feature touch `api-definitions/` or any generated surface?
      **YES** — New `OrganizationType.Host` enum value and offering-owned `hostCommissionPercentage` contract. Generator: `make generate` regenerates API contracts.

- [x] **II. Domain Boundaries** — Does this feature cross domain ownership lines?
      **PARTIALLY** — Hosts interact with Location (owned by location domain) and Booking (owned by booking domain). Cross-domain paths use public service/event interfaces:
      - Location service for auto-Resource creation
      - Booking service for full-place reservations

- [x] **III. Testing** — What test tier is required?
      **ALL TIERS**:
      - Unit: Host org creation, Resource auto-creation, commission calculation
      - Integration: End-to-end flows using repository-layer (no raw DbContext)
      - E2E: Playwright for critical user journeys (onboarding → listing → booking)

- [x] **IV. Frontend** — Does this feature include web changes?
      **YES**:
      - New `webapp-host` app under `src/web/apps/`
      - Relay colocation (fragments with components)
      - No hand-edited Relay artifacts
      - Typography wrappers from `@skedular/ui`
      - American spelling in user-facing copy

- [x] **V. Pattern Consistency** — Does this feature introduce a new pattern?
      **NO NEW PATTERNS**:
      - Reuses existing `webapp-spaces` structure
      - Uses same auth, product creation flow, map integration
      - Deviation: Host org type + auto-Resource logic (justified below)

- [x] **VI. Logging** — Does this feature add or change behavior?
      **YES** — Structured logging for:
      - Host organization creation/verification
      - Location/Product creation
      - Auto-Resource creation (with correlation ID)
      - Booking completion with commission calculation
      - Verification rejection / un-verify warnings

### Pattern Deviation Justification

**Deviation**: New "Host" UX pattern where Resource is invisible to user.

**Why Needed**: The existing Spaces flow requires users to understand and manage Resources — this is too complex for individual hosts. Auto-Resource creation is necessary for the MVP.

**Simpler Alternative Rejected Because**: A true simplification would require significant refactoring of the booking engine. The presentation-layer approach (auto-creating Resource behind scenes) minimizes risk while achieving the goal.

---

## Project Structure

### Documentation (this feature)

```text
specs/026-scheduler-host-app/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
src/
├── web/apps/webapp-host/          # New Host web app
│   ├── src/
│   │   ├── app/                   # Next.js App Router pages
│   │   ├── components/            # React components
│   │   │   └── commons/           # Shared wrappers (extends @skedular/ui)
│   │   ├── clients/               # HTTP API clients
│   │   ├── queries/               # Relay GraphQL files
│   │   │   └── __generated__/     # Generated artifacts (DO NOT EDIT)
│   │   ├── libs/                  # Auth, providers, theme, utils
│   │   └── types/                 # TypeScript types
│   ├── tests/                     # Vitest, Playwright tests
│   └── infrastructure/            # Terraform (later phase)
├── location/shared/
│   └── Location.Shared/
│       └── Services/
│           └── AutoResourceService.cs  # NEW: Auto-creates Resources for Host Products
└── customer/shared/
    └── Customer.Shared/
        └── Models/
            └── OrganizationType.cs      # EXTEND: Add Host enum value

tests/
├── contract/                      # Integration tests
│   └── HostFeatureTests/
└── unit/                          # Unit tests
    └── Location.Shared.UnitTests/

backend/
└── [existing structure — new code in shared modules]

frontend/
└── webapp-host/                   # New app (see src/web/apps/webapp-host)
```

**Structure Decision**: Single project with new web app subdirectory.

- **Backend changes**: Minimal. Extends existing `OrganizationType` enum, adds `AutoResourceService.cs` for Host Products.
- **Frontend changes**: New `webapp-host` Next.js app sharing `@skedular/ui`, `@skedular/shared`, and GraphQL contracts with other apps.
- **No separate backend/frontend split**: All new logic is in shared modules + one web app.

---

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation                  | Why Needed         | Simpler Alternative Rejected Because |
| -------------------------- | ------------------ | ------------------------------------ |
| New webapp (webapp-host)   | Dedicated Host UX; separate URL without impacting other apps | Hosting in existing apps would blur boundaries and require complex routing |

---

## Phase 1: Agent Context Update

Run the agent context update command to refresh the coding agent context file:

```bash
/speckit-agent-context-update
```

Or manually (if installed):

```bash
# From repo root
python3 -m specify extensions run speckit.agent-context.update
```

This updates the `<!-- SPECKIT START -->` / `<!-- SPECKIT END -->` section in `CLAUDE.md`.

---

## Extension Hooks (Post-Plan)

**Optional post-plan hooks** registered in `.specify/extensions.yml`:

| Hook | Command | Description |
|------|---------|-------------|
| git.commit (optional) | `/speckit-git-commit` | Auto-commit after planning |
| agent-context.update (optional) | `/speckit-agent-context-update` | Refresh agent context |

Both hooks are optional. Execute before proceeding to `/speckit-tasks` if desired.
