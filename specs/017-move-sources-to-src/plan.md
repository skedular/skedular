# Implementation Plan: Move Domain Sources Into src Directory

**Branch**: `017-move-sources-to-src` | **Date**: 2026-05-28 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `specs/017-move-sources-to-src/spec.md`

## Summary

Move fourteen domain source directories (`all-in-one`, `booking`, `core`, `customer`, `gateway`,
`location`, `marketplace`, `msteams`, `organization`, `shared`, `slack`, `system`, `team`,
`web`) plus `Skedular.slnx` from the repository root into a new `src/` container using `git mv`
per-file for history preservation. Follow with a reference-fix commit updating the ~100
path references in 31 files (Makefile, 21 workflow files, 7 scripts, 41 Dockerfiles,
2 Dockerfiles in `api-definitions/openapi/`, 2 `.vscode/` configs, and 4 prose documents).
Cross-domain `.csproj` `<ProjectReference>` entries and domain-level `.slnx` files require no
edits due to relative-path self-consistency. The DSST `042-move-sources-to-src` feature serves
as the authoritative implementation reference. See [research.md](research.md) for full decision
log and [data-model.md](data-model.md) for the complete file change inventory.

## Technical Context

**Language/Version**: Bash/sh, YAML (GitHub Actions), Dockerfile syntax, C# 14 / .NET 10
**Primary Dependencies**: git CLI (for `git mv`), dotnet CLI (build verification), Docker CLI (compose validation)
**Storage**: N/A — filesystem rename operation only
**Testing**: `dotnet build src/Skedular.slnx`, `docker compose config`, existing CI test suites
**Target Platform**: Repository structure (git + filesystem); Ubuntu CI runners
**Project Type**: Repository structural refactoring
**Performance Goals**: N/A
**Constraints**: `git mv` per-file required; all 14 domains + `Skedular.slnx` moved in one atomic commit; all path fix commits separate
**Scale/Scope**: 14 directories + 1 solution file moved; ~31 external files requiring ~100+ path reference updates

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — Does this feature touch `api-definitions/` or any generated surface?
      **No new contracts introduced.** Generation scripts (`api-definitions/generate.sh`,
      `scripts/generate-graphql.sh`) are updated in this feature so they continue to produce
      identical outputs from the repository root. Review gate satisfied.

- [x] **II. Domain Boundaries** — Does this feature cross domain ownership lines?
      **No cross-domain ownership changes.** All domain source trees move together. No service,
      event, or database access pattern is altered. Physical directory reorganization only.

- [x] **III. Testing** — What test tier is required?
      **Structural refactoring — no new test logic.** All existing unit, integration, and system
      tests must pass unchanged after the move. Exception approved: no new unit/integration tests
      are written for this feature (it contains no new logic). Build verification (`dotnet build`)
      and compose validation (`docker compose config`) serve as the acceptance tests.

- [x] **IV. Frontend** — Does this feature include web changes?
      **`web/` moves to `src/web/`.** No Relay fragment, generated artefact, typography wrapper,
      or copy changes. Workflow `paths:` filters and scripts that reference `web/` are updated.
      No `@skedular/ui` / `@skedular/shared` boundary changes.

- [x] **V. Pattern Consistency** — Does this feature introduce a new pattern?
      **Yes — establishes the `src/` top-level source container convention for the Skedular
      repository.** This is an intentional structural alignment with the DSST repository pattern.
      No parallel abstractions or framework deviations introduced. Justification: root cleanliness,
      predictable layout for new contributors, alignment with established sibling repo convention.

- [x] **VI. Logging** — Does this feature add or change behaviour?
      **No new application code paths, no new `ILogger` events required.** This is a structural
      refactor. CI/CD pipeline logs will reflect updated paths post-merge; no instrumentation work
      needed. Constitution logging exception approved for structural-only features.

## Project Structure

### Documentation (this feature)

```text
specs/017-move-sources-to-src/
├── plan.md              # This file
├── research.md          # Phase 0 — path audit and self-consistency analysis
├── data-model.md        # Phase 1 — path transformation model and file change inventory
├── quickstart.md        # Phase 1 — developer execution runbook
└── tasks.md             # Phase 2 output (/speckit.tasks — NOT created by /speckit.plan)
```

### Repository Layout After Move

```text
skedular/                            ← repo root (unchanged)
├── .agents/
├── .config/
├── .github/
│   ├── copilot-instructions.md        ← ## Project Structure section updated
│   └── workflows/                     ← all 21 workflow files: paths/workingDirectory/dockerFilePath updated
├── .specify/
├── .vscode/
│   ├── tasks.json                     ← all-in-one/ and web/ paths updated
│   └── launch.json                    ← all-in-one/ and web/ paths updated
├── api-definitions/
│   └── openapi/
│       └── services.Dockerfile        ← shared/ COPY sources → src/shared/
├── assets/                            ← unchanged
├── docs/                              ← unchanged
├── scripts/
│   ├── generate-graphql.sh            ← ${BASE_DIR}/<domain>/ → ${BASE_DIR}/src/<domain>/
│   ├── update-dotnet-tools.sh         ← all 47 domain path entries prefixed with src/
│   ├── lint.sh                        ← Skedular.slnx → src/Skedular.slnx
│   ├── format.sh                      ← Skedular.slnx → src/Skedular.slnx
│   ├── validate-three-products.sh     ← $root/web/ → $root/src/web/
│   ├── validate-workspace-layout.sh   ← $root/web/ → $root/src/web/
│   └── verify-ui-package-versions.sh  ← $root/web/ → $root/src/web/
├── specs/                             ← unchanged
├── src/                               ← NEW container directory
│   ├── Skedular.slnx                  ← moved from root (no content edits)
│   ├── all-in-one/                    ← moved from root
│   ├── booking/                       ← moved from root
│   ├── core/                          ← moved from root
│   ├── customer/                      ← moved from root
│   ├── gateway/                       ← moved from root
│   ├── location/                      ← moved from root
│   ├── marketplace/                   ← moved from root
│   ├── msteams/                       ← moved from root
│   ├── organization/                  ← moved from root
│   ├── shared/                        ← moved from root
│   ├── slack/                         ← moved from root
│   ├── system/                        ← moved from root
│   ├── team/                          ← moved from root
│   └── web/                           ← moved from root
├── AGENTS.md                          ← prose domain path references updated
├── CLAUDE.md                          ← prose domain path references updated
├── Makefile                           ← dep + generate targets updated
├── README.md                          ← prose domain path references updated
├── docker-compose*.yml                ← NO CHANGES (no domain build contexts)
└── [other root files]                 ← NO CHANGES
```

## Complexity Tracking

No constitution violations. No complexity exceptions required.

---

## Implementation Phases

### Phase A — Atomic git mv Commit

Create `src/` and move all 14 domain directories plus `Skedular.slnx` using `git mv` per-file
in a single commit. No file contents are changed.

```bash
mkdir -p src
domains=(all-in-one booking core customer gateway location marketplace msteams organization shared slack system team web)
for domain in "${domains[@]}"; do git mv "$domain" "src/$domain"; done
git mv Skedular.slnx src/Skedular.slnx
git status --short | wc -l   # expect only renamed entries
git commit -m "chore: move domain sources into src/"
```

**Verification**: `ls src/` lists 15 entries (14 domains + `Skedular.slnx`).
None of the domain names exist at root. `git status` clean.

---

### Phase B — Path Reference Fix Commits

Update all files anchored outside `src/` whose paths are now stale. Recommended as one commit
or grouped by category. See [data-model.md](data-model.md) for complete change inventory and
[quickstart.md](quickstart.md) for the developer runbook.

#### B1 — Makefile

- `dotnet restore Skedular.slnx` → `dotnet restore src/Skedular.slnx`
- `./web/apps/webapp*/scripts/generate.sh` → `./src/web/apps/webapp*/scripts/generate.sh`

#### B2 — scripts/generate-graphql.sh

All `${BASE_DIR}/<domain>/...` occurrences → `${BASE_DIR}/src/<domain>/...` (24 changes).
The nitro compose `-f ../../../<domain>/...` relative args do NOT change.

#### B3 — scripts/update-dotnet-tools.sh

All 47 `"<domain>/..."` project list entries → `"src/<domain>/..."`.

#### B4 — scripts/lint.sh, scripts/format.sh

`Skedular.slnx` → `src/Skedular.slnx` (1 change each).

#### B5 — scripts/validate-three-products.sh, validate-workspace-layout.sh, verify-ui-package-versions.sh

`$root/web/apps/$app` → `$root/src/web/apps/$app` (1 change each).

#### B6 — api-definitions/openapi/services.Dockerfile

Three `COPY ["shared/...", ...]` source paths → `COPY ["src/shared/...", ...]`.

#### B7 — All 41 domain Dockerfiles

`WORKDIR "/src/<domain>/..."` → `WORKDIR "/src/src/<domain>/..."` in every Dockerfile.
Any other container-absolute path like `/src/<domain>/...` also gains the `src/` segment
(e.g., `all-in-one/AllApis/Dockerfile` references `/src/gateway/apis/Gateway/gateway.far`).

#### B8 — .github/workflows/ (21 files)

For each workflow:

1. `paths:` filters: `- "<domain>/**"` → `- "src/<domain>/**"`
2. `workingDirectory:` / `working-directory:`: `./<domain>/...` → `./src/<domain>/...`
3. `dockerFilePath:`: `./<domain>/...` → `./src/<domain>/...`
4. `copilot-setup-steps.yml` only: `Skedular.slnx` → `src/Skedular.slnx`; `web/pnpm-lock.yaml` → `src/web/pnpm-lock.yaml`

#### B9 — .vscode/tasks.json, .vscode/launch.json

`${workspaceFolder}/all-in-one/...` → `${workspaceFolder}/src/all-in-one/...`
`${workspaceFolder}/web/apps/webapp/...` → `${workspaceFolder}/src/web/apps/webapp/...`

#### B10 — Prose documentation

Update domain path references in `README.md`, `AGENTS.md`, `CLAUDE.md`,
`.github/copilot-instructions.md` (Project Structure section and Active Technologies).

---

## Verification Checklist

After all Phase B commits, run from repo root:

```bash
# 1. No domain directories at root
for d in all-in-one booking core customer gateway location marketplace msteams organization shared slack system team web; do
  [ -d "$d" ] && echo "STILL AT ROOT: $d"
done

# 2. Build
dotnet restore src/Skedular.slnx && dotnet build src/Skedular.slnx --no-restore

# 3. Compose validation
docker compose -f docker-compose.yml config
docker compose -f docker-compose-min.yml config

# 4. Stale path grep (expect zero matches)
grep -rn '"booking/\|"shared/\|"gateway/\|"all-in-one/\|"web/' \
  .github/workflows/ scripts/ Makefile api-definitions/openapi/ .vscode/ 2>/dev/null \
  | grep -v "src/"
```

---

## Files That Do NOT Need Updates

| File / Category                               | Reason                             |
| --------------------------------------------- | ---------------------------------- |
| All `.csproj` `<ProjectReference>` entries    | Relative-path self-consistency     |
| Domain-level `.slnx` inside `src/`            | Relative-path self-consistency     |
| `docker-compose*.yml` (4 files)               | No domain build contexts           |
| `api-definitions/openapi/clients.Dockerfile`  | Only references `api-definitions/` |
| `api-definitions/openapi/generate.sh`         | Relative-only calls                |
| `.pre-commit-config.yaml`, `.terraformignore` | No domain paths                    |
| `.gitignore`, `.dockerignore`                 | No domain-path patterns (verified) |
| `scripts/delete-all-workflow-runs.sh`         | No domain paths                    |
| `scripts/start-dependencies*.sh`              | No domain paths                    |
| `scripts/update-web-npm-packages.sh`          | No domain paths                    |
