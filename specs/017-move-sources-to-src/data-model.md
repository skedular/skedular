# Data Model: Move Domain Sources Into src Directory

**Feature**: `017-move-sources-to-src`
**Date**: 2026-05-28

## Overview

This feature has no persistent data model changes (no database schema, no domain entities).
The "data model" here is the **path transformation model** — the rules governing which paths
change, by how much, and which stay the same.

---

## Directory Map: Before → After

| Before (root-relative) | After (root-relative) | Action                               |
| ---------------------- | --------------------- | ------------------------------------ |
| `all-in-one/`          | `src/all-in-one/`     | Move via `git mv`                    |
| `booking/`             | `src/booking/`        | Move via `git mv`                    |
| `core/`                | `src/core/`           | Move via `git mv`                    |
| `customer/`            | `src/customer/`       | Move via `git mv`                    |
| `gateway/`             | `src/gateway/`        | Move via `git mv`                    |
| `location/`            | `src/location/`       | Move via `git mv`                    |
| `marketplace/`         | `src/marketplace/`    | Move via `git mv`                    |
| `msteams/`             | `src/msteams/`        | Move via `git mv`                    |
| `organization/`        | `src/organization/`   | Move via `git mv`                    |
| `shared/`              | `src/shared/`         | Move via `git mv`                    |
| `slack/`               | `src/slack/`          | Move via `git mv`                    |
| `system/`              | `src/system/`         | Move via `git mv`                    |
| `team/`                | `src/team/`           | Move via `git mv`                    |
| `web/`                 | `src/web/`            | Move via `git mv`                    |
| `Skedular.slnx`        | `src/Skedular.slnx`   | Move via `git mv` (no content edits) |
| `api-definitions/`     | `api-definitions/`    | Stays at root                        |
| `assets/`              | `assets/`             | Stays at root                        |
| `docs/`                | `docs/`               | Stays at root                        |
| `scripts/`             | `scripts/`            | Stays at root                        |
| `specs/`               | `specs/`              | Stays at root                        |
| `.github/`             | `.github/`            | Stays at root                        |
| `.specify/`            | `.specify/`           | Stays at root                        |
| `.vscode/`             | `.vscode/`            | Stays at root (content updated)      |
| All root loose files   | unchanged location    | Stays (content updated where needed) |

---

## Path Transformation Rules

### Rule 1: Root-External Reference

Any file that remains at the repository root (or under `scripts/`, `api-definitions/`,
`.github/`, `.vscode/`) and contains a root-relative path into a moved domain must have `src/`
prepended to the domain segment:

```text
<domain>/<rest>  →  src/<domain>/<rest>
```

Where `<domain>` ∈ `{all-in-one, booking, core, customer, gateway, location, marketplace,
msteams, organization, shared, slack, system, team, web}`.

**Applies to**: `Makefile`, workflow files, scripts, Dockerfiles, `.vscode/` configs,
prose documentation.

### Rule 2: Cross-Domain Relative Reference (files inside src/)

Any relative path between two moved directories is self-consistent — the relative distance
is unchanged. **No transformation required.**

```text
# src/all-in-one/AllInOne/AllInOne.csproj references shared:
# Before: all-in-one/AllInOne/ → ../../shared/ = shared/        ✓
# After:  src/all-in-one/AllInOne/ → ../../shared/ = src/shared/ ✓
../../shared/<project>  →  ../../shared/<project>   (NO CHANGE)
```

**Applies to**: All `.csproj` `<ProjectReference>` entries, all domain `.slnx` files.

### Rule 3: Skedular.slnx Position Change

`Skedular.slnx` moves from the repository root into `src/`. Its internal project paths use
paths relative to the `.slnx` file's own location. Since both the `.slnx` and all domains move
into `src/` together, the relative paths (e.g., `booking/apis/Booking.Api/Booking.Api.csproj`)
resolve correctly from `src/Skedular.slnx` with **no content changes**.

External callers that invoke `dotnet restore Skedular.slnx` from the repo root must change to
`dotnet restore src/Skedular.slnx`.

### Rule 4: Docker WORKDIR Container Path

Domain Dockerfiles use the container path `/src` as the build WORKDIR (coincidentally named).
`COPY . .` copies the repo root into container `/src`. After the host move, `<domain>/` is at
`src/<domain>/` in the repo root, landing at `/src/src/<domain>/` in the container:

```text
WORKDIR "/src/<domain>/path/to/project"
→
WORKDIR "/src/src/<domain>/path/to/project"
```

Any other container-absolute path string referencing a moved domain (e.g., in shell commands
inside the Dockerfile) follows the same rule.

### Rule 5: api-definitions Dockerfile COPY Sources

`api-definitions/openapi/services.Dockerfile` is built with repo root as build context (`../../`
from `api-definitions/openapi/`). It uses explicit COPY sources for `shared/` subdirectories:

```text
COPY ["shared/<rest>", "shared/<rest>"]
→
COPY ["src/shared/<rest>", "shared/<rest>"]
```

The destination path inside the container is unchanged.

---

## Complete File Change Inventory

### Category A: Move Only (no content edits)

| File                | Change                                    |
| ------------------- | ----------------------------------------- |
| `Skedular.slnx`     | `git mv Skedular.slnx src/Skedular.slnx`  |
| All 14 domain trees | `git mv <domain> src/<domain>` (per-file) |

### Category B: Path Reference Updates Required

#### B1 — Makefile (3 changes)

| Before                                         | After                                              |
| ---------------------------------------------- | -------------------------------------------------- |
| `dotnet restore Skedular.slnx`                 | `dotnet restore src/Skedular.slnx`                 |
| `./web/apps/webapp/scripts/generate.sh`        | `./src/web/apps/webapp/scripts/generate.sh`        |
| `./web/apps/webapp-spaces/scripts/generate.sh` | `./src/web/apps/webapp-spaces/scripts/generate.sh` |
| `./web/apps/webapp-teams/scripts/generate.sh`  | `./src/web/apps/webapp-teams/scripts/generate.sh`  |

#### B2 — scripts/generate-graphql.sh (24 changes)

Replace every `${BASE_DIR}/<domain>` with `${BASE_DIR}/src/<domain>` in:

- `rm -f "${BASE_DIR}/gateway/..."` (1)
- `export_schema "${BASE_DIR}/<domain>/..."` calls (9)
- `cd "${BASE_DIR}/gateway/..."` (1)
- `GATEWAY_FAR="${BASE_DIR}/gateway/..."` (1)
- `cd "${BASE_DIR}/<domain>/domain/..."` integration test dirs (10)
- `cd "${BASE_DIR}/system/..."` (1)
- gateway compose schema entry grep (no path change needed — it's a zip entry name)

Note: The `nitro fusion compose -f ../../../<domain>/...` relative arguments do NOT change —
they are relative from within the moved gateway directory and remain self-consistent.

#### B3 — scripts/update-dotnet-tools.sh (47 changes)

All 47 `"<domain>/..."` entries in the project list → `"src/<domain>/..."`.

#### B4 — scripts/lint.sh, scripts/format.sh (1 change each)

| Before          | After               |
| --------------- | ------------------- |
| `Skedular.slnx` | `src/Skedular.slnx` |

#### B5 — scripts/validate-three-products.sh, scripts/validate-workspace-layout.sh, scripts/verify-ui-package-versions.sh (1 change each)

| Before                | After                     |
| --------------------- | ------------------------- |
| `$root/web/apps/$app` | `$root/src/web/apps/$app` |

#### B6 — api-definitions/openapi/services.Dockerfile (3 changes)

| Before                                   | After                                        |
| ---------------------------------------- | -------------------------------------------- |
| `COPY ["shared/Api.Shared", ...]`        | `COPY ["src/shared/Api.Shared", ...]`        |
| `COPY ["shared/Enterprise.Shared", ...]` | `COPY ["src/shared/Enterprise.Shared", ...]` |
| `COPY ["shared/Skedularctl", ...]`       | `COPY ["src/shared/Skedularctl", ...]`       |

#### B7 — Domain Dockerfiles (41 files, 1–3 changes each)

All `WORKDIR "/src/<domain>/..."` container paths → `WORKDIR "/src/src/<domain>/..."`.
Any additional container path strings referencing moved domains also updated.
Example from `all-in-one/AllApis/Dockerfile`:

- `WORKDIR "/src/all-in-one/AllApis"` → `WORKDIR "/src/src/all-in-one/AllApis"`
- `/src/gateway/apis/Gateway/gateway.far` → `/src/src/gateway/apis/Gateway/gateway.far`

#### B8 — .github/workflows/ (21 files)

Per-workflow breakdown:

| Workflow                  | Changes                                                           |
| ------------------------- | ----------------------------------------------------------------- |
| `booking-shared.yml`      | `paths` filters, `workingDirectory`                               |
| `customer-shared.yml`     | `paths` filters, `workingDirectory`                               |
| `docs-event-catalog.yml`  | `paths` filters                                                   |
| `location-shared.yml`     | `paths` filters, `workingDirectory`                               |
| `msteams-shared.yml`      | `paths` filters, `workingDirectory`                               |
| `organization-shared.yml` | `paths` filters, `workingDirectory`                               |
| `shared-azure-entra.yml`  | `paths` filters, `workingDirectory`                               |
| `shared.yml`              | `paths` filters, `workingDirectory`                               |
| `slack-shared.yml`        | `paths` filters, `workingDirectory`                               |
| `team-shared.yml`         | `paths` filters, `workingDirectory`                               |
| `web-shared.yml`          | `paths` filters, `workingDirectory`                               |
| `webapp.yml`              | `paths` filters, `workingDirectory`, `dockerFilePath`             |
| `webapp-help.yml`         | `paths` filters (if any), `workingDirectory`                      |
| `webapp-spaces.yml`       | `paths` filters, `workingDirectory`, `dockerFilePath`             |
| `webapp-spaces-help.yml`  | `paths` filters (if any), `workingDirectory`                      |
| `webapp-teams.yml`        | `paths` filters, `workingDirectory`, `dockerFilePath`             |
| `webapp-teams-help.yml`   | `paths` filters (if any), `workingDirectory`                      |
| `workarounds.yml`         | `paths` filters, `dockerFilePath`                                 |
| `lint.yml`                | `paths` filters (if any)                                          |
| `copilot-setup-steps.yml` | `dotnet restore Skedular.slnx`, `hashFiles('web/pnpm-lock.yaml')` |
| `workarounds.yml`         | `paths` filters, `dockerFilePath`                                 |

#### B9 — .vscode/tasks.json (1 change), .vscode/launch.json (8 changes)

| Before                                   | After                                        |
| ---------------------------------------- | -------------------------------------------- |
| `${workspaceFolder}/all-in-one/...`      | `${workspaceFolder}/src/all-in-one/...`      |
| `${workspaceFolder}/web/apps/webapp/...` | `${workspaceFolder}/src/web/apps/webapp/...` |

#### B10 — Prose Documentation (4+ files)

| File                              | Changes                                              |
| --------------------------------- | ---------------------------------------------------- |
| `README.md`                       | All domain directory path examples                   |
| `AGENTS.md`                       | Domain directory references in rules and examples    |
| `CLAUDE.md`                       | Domain directory references                          |
| `.github/copilot-instructions.md` | `## Project Structure` section + Active Technologies |

---

## Verification Checklist

After all commits:

```bash
# 1. No root-level domain directories remain
for d in all-in-one booking core customer gateway location marketplace msteams organization shared slack system team web; do
  [ -d "$d" ] && echo "STILL AT ROOT: $d" || echo "OK: $d"
done

# 2. src/ contains all fourteen
ls src/

# 3. Build
dotnet restore src/Skedular.slnx
dotnet build src/Skedular.slnx --no-restore

# 4. Generate scripts (smoke test)
bash api-definitions/generate.sh
bash scripts/generate-graphql.sh

# 5. Compose validation
docker compose -f docker-compose.yml config
docker compose -f docker-compose-min.yml config

# 6. Stale path grep (should return zero matches)
grep -r "\"booking/\|\"shared/\|\"gateway/\|\"all-in-one/\|\"web/" \
  .github/workflows/ scripts/ Makefile api-definitions/openapi/ .vscode/ \
  --include="*.yml" --include="*.yaml" --include="*.sh" --include="*.json" \
  --include="Makefile" --include="Dockerfile"
```
