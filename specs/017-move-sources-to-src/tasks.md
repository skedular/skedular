# Tasks: Move Domain Sources Into src Directory

**Input**: Design documents from `/specs/017-move-sources-to-src/`
**Prerequisites**: plan.md ✓, spec.md ✓, research.md ✓, data-model.md ✓, quickstart.md ✓

**Organization**: Tasks grouped by user story for independent implementation and verification.
Tests are **not included** — this is a structural refactor with no new logic (constitution exception approved).

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no blocking dependencies)
- **[Story]**: User story: US1–US6 from spec.md

---

## Phase 1: Setup

**Purpose**: Confirm the workspace is ready before the atomic move.

- [X] T001 Verify branch `017-move-sources-to-src` is active and `git status` shows a clean working tree

---

## Phase 2: Foundational — Atomic git mv Commit

**Purpose**: Move all 14 domain directories and `Skedular.slnx` into `src/` in one atomic commit.
This is a blocking prerequisite for every user story — no path fix tasks can begin until this commit lands.

**⚠️ CRITICAL**: Do not change any file content in this commit. `git mv` only.

- [X] T002 Execute Phase A atomic move: `mkdir -p src`, run `git mv <domain> src/<domain>` for all 14 domains (`all-in-one`, `booking`, `core`, `customer`, `gateway`, `location`, `marketplace`, `msteams`, `organization`, `shared`, `slack`, `system`, `team`, `web`), run `git mv Skedular.slnx src/Skedular.slnx`, then commit with message `chore: move domain sources into src/`

**Checkpoint**: `ls src/` lists 15 entries (14 domains + `Skedular.slnx`). No domain directory exists at repo root. `git status` is clean.

---

## Phase 3: User Story 1 — Domain Directories Relocated Under src/ (Priority: P1) 🎯 MVP

**Goal**: Verify the repository root is clean and `src/` holds all fourteen domain directories.

**Independent Test**: `ls ./` at the repo root shows none of the 14 domain names. `ls ./src` shows all 14.

- [X] T003 [US1] Verify directory layout: confirm `src/` contains all 14 domain directories and `Skedular.slnx`; confirm none of `all-in-one booking core customer gateway location marketplace msteams organization shared slack system team web` appear at the repo root

**Checkpoint**: US1 acceptance scenarios 1–4 all pass (layout verified, root clean, unchanged dirs like `docs/` and `specs/` still at root, internal file tree intact).

---

## Phase 4: User Story 2 — Solution File and Project References Updated (Priority: P1)

**Goal**: Confirm `dotnet build src/Skedular.slnx` resolves all projects without errors.
No `.csproj` `<ProjectReference>` edits needed (relative-path self-consistency — research D1, D2).

**Independent Test**: `dotnet build src/Skedular.slnx` from repo root completes with zero errors and zero unresolved reference warnings.

- [X] T004 [US2] Run `dotnet restore src/Skedular.slnx` and `dotnet build src/Skedular.slnx --no-restore` from the repository root; confirm zero build errors and zero unresolved project reference warnings

**Checkpoint**: SC-001 satisfied. Build is green.

---

## Phase 5: User Story 3 — Docker Compose Files and Dockerfiles Resolve Correctly (Priority: P1)

**Goal**: Update all 41 domain Dockerfiles and `services.Dockerfile` so every `WORKDIR` and container-absolute path reflects the new `src/` nesting. Compose validation passes.

**Independent Test**: `docker compose -f docker-compose.yml config` and `docker compose -f docker-compose-min.yml config` complete with zero path errors.

- [X] T005 [P] [US3] Update `api-definitions/openapi/services.Dockerfile` — change the 3 `COPY ["shared/...", ...]` source paths to `COPY ["src/shared/...", ...]`; destinations stay unchanged (see data-model.md B6)
- [X] T006 [P] [US3] Update `all-in-one` Dockerfiles — change `WORKDIR "/src/all-in-one/..."` → `WORKDIR "/src/src/all-in-one/..."` in `src/all-in-one/AllApis/Dockerfile`, `AllApisJobs/Dockerfile`, `AllInfra/Dockerfile`, `AllJobs/Dockerfile`, `AllProcessors/Dockerfile`; also update `/src/gateway/` container path in `AllApis/Dockerfile` → `/src/src/gateway/`
- [X] T007 [P] [US3] Update `booking` Dockerfiles — change every `WORKDIR "/src/booking/..."` → `WORKDIR "/src/src/booking/..."` in all Dockerfiles under `src/booking/apis/`, `src/booking/jobs/`, `src/booking/processors/`
- [X] T008 [P] [US3] Update `core` and `customer` Dockerfiles — change `WORKDIR "/src/core/..."` → `WORKDIR "/src/src/core/..."` and `WORKDIR "/src/customer/..."` → `WORKDIR "/src/src/customer/..."` in all Dockerfiles under `src/core/` and `src/customer/`
- [X] T009 [P] [US3] Update `gateway` and `location` Dockerfiles — change `WORKDIR "/src/gateway/..."` → `WORKDIR "/src/src/gateway/..."` and `WORKDIR "/src/location/..."` → `WORKDIR "/src/src/location/..."` in all Dockerfiles under `src/gateway/` and `src/location/`
- [X] T010 [P] [US3] Update `marketplace`, `msteams`, and `organization` Dockerfiles — change `WORKDIR "/src/<domain>/..."` → `WORKDIR "/src/src/<domain>/..."` for each domain in all Dockerfiles under `src/marketplace/`, `src/msteams/`, `src/organization/`
- [X] T011 [P] [US3] Update `slack`, `system`, `team`, and `web` Dockerfiles — change `WORKDIR "/src/<domain>/..."` → `WORKDIR "/src/src/<domain>/..."` for each domain in all Dockerfiles under `src/slack/`, `src/system/`, `src/team/`, `src/web/`
- [X] T012 [US3] Verify Docker: run `docker compose -f docker-compose.yml config` and `docker compose -f docker-compose-min.yml config` from the repository root; confirm both complete with zero path resolution errors

**Checkpoint**: SC-002 satisfied. US3 acceptance scenarios 1–2 pass.

---

## Phase 6: User Story 4 — CI/CD Pipelines and GitHub Actions Workflows Updated (Priority: P1)

**Goal**: Update all 21 workflow files so every `paths:` filter, `workingDirectory`, `dockerFilePath`, and inline path string references `src/<domain>/` instead of `<domain>/`.

**Independent Test**: After merging, a push triggers the correct workflow with no path-not-found errors.

- [X] T013 [P] [US4] Update `.github/workflows/booking-shared.yml`, `customer-shared.yml`, `location-shared.yml` — add `src/` prefix to all `paths:` filter entries and `workingDirectory` / `working-directory` values
- [X] T014 [P] [US4] Update `.github/workflows/msteams-shared.yml`, `organization-shared.yml`, `shared.yml`, `shared-azure-entra.yml` — add `src/` prefix to all `paths:` filters and `workingDirectory` values
- [X] T015 [P] [US4] Update `.github/workflows/slack-shared.yml`, `team-shared.yml`, `web-shared.yml` — add `src/` prefix to all `paths:` filters and `workingDirectory` values
- [X] T016 [P] [US4] Update `.github/workflows/webapp.yml`, `webapp-spaces.yml`, `webapp-teams.yml` — add `src/` prefix to `paths:` filters, `workingDirectory`, and `dockerFilePath` values
- [X] T017 [P] [US4] Update `.github/workflows/webapp-help.yml`, `webapp-spaces-help.yml`, `webapp-teams-help.yml` — add `src/` prefix to any `paths:` filters and `workingDirectory` values
- [X] T018 [P] [US4] Update `.github/workflows/workarounds.yml`, `docs-event-catalog.yml`, `lint.yml` — add `src/` prefix to `paths:` filters and `dockerFilePath` values (see data-model.md B8)
- [X] T019 [US4] Update `.github/workflows/copilot-setup-steps.yml` — change `dotnet restore Skedular.slnx` → `dotnet restore src/Skedular.slnx` and `hashFiles('web/pnpm-lock.yaml')` → `hashFiles('src/web/pnpm-lock.yaml')`

**Checkpoint**: All 21 workflow files updated. US4 acceptance scenarios 1–2 pass.

---

## Phase 7: User Story 5 — Generation Scripts Work with New Paths (Priority: P2)

**Goal**: Update `Makefile` and all scripts under `scripts/` that reference moved domain paths.

**Independent Test**: `bash api-definitions/generate.sh` and `bash scripts/generate-graphql.sh` from repo root complete without path errors.

- [X] T020 [P] [US5] Update `Makefile` — change `dotnet restore Skedular.slnx` → `dotnet restore src/Skedular.slnx` and `./web/apps/webapp/scripts/generate.sh`, `./web/apps/webapp-spaces/scripts/generate.sh`, `./web/apps/webapp-teams/scripts/generate.sh` → `./src/web/apps/...` equivalents
- [X] T021 [P] [US5] Update `scripts/generate-graphql.sh` — replace all `${BASE_DIR}/<domain>` references with `${BASE_DIR}/src/<domain>` (24 changes across `gateway`, `booking`, `core`, `customer`, `location`, `marketplace`, `msteams`, `organization`, `slack`, `team`, `system`); do NOT change the nitro fusion `-f ../../../<domain>/...` relative args. Note: `api-definitions/generate.sh` requires no changes — it uses relative-only calls (research D6).
- [X] T022 [P] [US5] Update `scripts/update-dotnet-tools.sh` — prefix all 47 `"<domain>/..."` project-list entries with `src/` (e.g., `"booking/apis/Booking.Api"` → `"src/booking/apis/Booking.Api"`)
- [X] T023 [P] [US5] Update `scripts/lint.sh` and `scripts/format.sh` — change `Skedular.slnx` → `src/Skedular.slnx` in each file (1 change per file)
- [X] T024 [P] [US5] Update `scripts/validate-three-products.sh`, `scripts/validate-workspace-layout.sh`, `scripts/verify-ui-package-versions.sh` — change `$root/web/apps` → `$root/src/web/apps` in each file (1 change per file)

**Checkpoint**: Makefile targets, generate scripts, lint/format scripts all resolve domain paths correctly under `src/`.

---

## Phase 8: User Story 6 — Documentation and Prose References Updated (Priority: P2)

**Goal**: Update `.vscode/` configs and all root-level prose documents so every domain path example reflects `src/<domain>/`.

**Independent Test**: `grep -r` for bare domain path references in documentation and tooling configs returns zero matches.

- [X] T025 [P] [US6] Update `.vscode/tasks.json` — change `${workspaceFolder}/all-in-one/` → `${workspaceFolder}/src/all-in-one/`
- [X] T026 [P] [US6] Update `.vscode/launch.json` — change all 8 occurrences of `${workspaceFolder}/all-in-one/` and `${workspaceFolder}/web/apps/webapp/` to `${workspaceFolder}/src/all-in-one/` and `${workspaceFolder}/src/web/apps/webapp/`
- [X] T027 [P] [US6] Update `README.md` — change all domain directory path examples and project structure listings to use `src/<domain>/`
- [X] T028 [P] [US6] Update `AGENTS.md` — change all domain directory path references and examples in rules and notes to use `src/<domain>/`
- [X] T029 [P] [US6] Update `CLAUDE.md` and `.github/copilot-instructions.md` — change all `## Project Structure` diagrams, Active Technologies path references, and any `## Recent Changes` path examples to use `src/<domain>/`

**Checkpoint**: US6 acceptance scenarios 1–3 pass. No stale root-level domain paths in documentation.

---

## Phase 9: Polish & Verification

**Purpose**: Commit all Phase B changes and run the full verification checklist.

- [X] T030 Commit all Phase B changes: `git add -A && git commit -m "chore: update path references for src/ domain layout"`
- [X] T031 Run stale path grep check — `grep -rn "booking/\|shared/\|gateway/\|all-in-one/\|web/" .github/workflows/ scripts/ Makefile api-definitions/openapi/ .vscode/ README.md AGENTS.md CLAUDE.md .github/copilot-instructions.md 2>/dev/null | grep -v "src/"` — expect zero matches (covers tooling files per SC-003/SC-004 and prose files per SC-007)
- [X] T032 Run full verification checklist: `dotnet restore src/Skedular.slnx`, `dotnet build src/Skedular.slnx --no-restore`, `docker compose -f docker-compose.yml config`, `docker compose -f docker-compose-min.yml config` — all must succeed with zero errors
- [X] T033 [US5] Run generation script verification — `bash scripts/generate-graphql.sh` and `make generate` from the repository root; confirm both complete without path-not-found errors and produce output identical to the pre-move baseline (satisfies SC-004 and SC-008)

---

## Dependencies

```text
T001 → T002 → T003          (setup → move → verify layout)
T002 → T004                 (move → verify build)
T002 → T005–T011            (move → docker fixes, all parallel)
T005–T011 → T012            (all dockerfiles done → compose verify)
T002 → T013–T018            (move → workflow fixes, all parallel)
T018, T013–T017 → T019      (other workflows first, copilot-setup-steps last due to special fields)
T002 → T020–T024            (move → script fixes, all parallel)
T002 → T025–T029            (move → doc fixes, all parallel)
T005–T029 → T030            (all path fixes done → commit)
T030 → T031 → T032 → T033   (commit → stale grep → build/compose → generate verify)
```

## Parallel Execution

After **T002** commits the atomic move, all four story groups can run simultaneously:

| Agent A (US3 — Docker)            | Agent B (US4 — Workflows)      | Agent C (US5 — Scripts)  | Agent D (US6 — Docs)     |
| --------------------------------- | ------------------------------ | ------------------------ | ------------------------ |
| T005 services.Dockerfile          | T013 booking/customer/location | T020 Makefile            | T025 .vscode/tasks.json  |
| T006 all-in-one Dockerfiles       | T014 msteams/org/shared        | T021 generate-graphql.sh | T026 .vscode/launch.json |
| T007 booking Dockerfiles          | T015 slack/team/web            | T022 update-dotnet-tools | T027 README.md           |
| T008 core/customer Dockerfiles    | T016 webapp\*.yml              | T023 lint.sh/format.sh   | T028 AGENTS.md           |
| T009 gateway/location Dockerfiles | T017 webapp-\*-help.yml        | T024 validate scripts    | T029 CLAUDE.md/copilot   |
| T010 marketplace/msteams/org      | T018 workarounds/docs/lint     |                          |                          |
| T011 slack/system/team/web        | T019 copilot-setup-steps.yml   |                          |                          |
| T012 docker compose verify        |                                |                          |                          |

## Implementation Strategy

**MVP scope**: T001–T004 complete the atomic move and verify the build is functional.
All four P2-independent story groups (US3–US6) can then proceed in parallel without blocking each other.

1. **Phase A first** (T001–T002): The atomic `git mv` must land before ANY other work.
2. **Verify immediately** (T003–T004): Confirm the layout and build pass before starting the 28 path-fix tasks.
3. **Parallel path fixes** (T005–T029): All four story groups are fully independent. Assign each to a separate agent or batch them in order US3 → US4 → US5 → US6 if sequential.
4. **Single Phase B commit** (T030): Commit everything together to keep git history clean.
5. **Final verification** (T031–T033): Stale path grep + build + compose + generation scripts confirm completeness.
