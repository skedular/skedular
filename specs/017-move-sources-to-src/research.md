# Research: Move Domain Sources Into src Directory

**Feature**: `017-move-sources-to-src`
**Date**: 2026-05-28
**Phase**: 0 — Pre-Design Research

---

## Key Insight: Relative-Path Self-Consistency

The most important finding of this research is that **the majority of internal cross-references
do not need to be updated**. Because all fourteen domains move together into `src/`, the relative
distance between any two of those directories is unchanged. Existing `<ProjectReference>` entries
and domain-level `.slnx` files that navigate peer directories via relative paths (e.g.,
`../shared/`, `../../shared/`) resolve correctly after the move without any edits.

Only files **anchored outside `src/`** — files that remain at the repository root, in
`api-definitions/`, `scripts/`, or `.github/` — need their paths updated, because they reference
the moved directories from an external vantage point.

The DSST repository (`042-move-sources-to-src`) serves as the authoritative implementation
reference. Its patterns are adapted here for the fourteen-domain Skedular layout.

---

## Decision Log

### D1: Cross-Domain .csproj ProjectReferences

**Decision**: No updates required for any `<ProjectReference>` in any moved `.csproj`.

**Rationale**: Before the move, e.g., `all-in-one/AllInOne/AllInOne.csproj` references
`../../booking/...`. After the move to `src/all-in-one/AllInOne/`, the path `../../booking/`
resolves to `src/booking/` (two levels up from `src/all-in-one/AllInOne/` = `src/`, then
`booking/`). The same logic applies to all three-level traversals. Relative depth is preserved
because all domains move together.

**Alternatives considered**: Updating all cross-domain project references — rejected as
unnecessary given the self-consistency property.

### D2: Domain-Level .slnx Files

**Decision**: Domain-level `.slnx` files inside moved domains (e.g., within `src/booking/`,
`src/team/`) do NOT need path updates.

**Rationale**: These files reference peer directories via relative paths. After the move to
`src/<domain>/`, `../shared/` still resolves to `src/shared/` — self-consistency preserved.

### D3: Skedular.slnx

**Decision**: `Skedular.slnx` MUST be moved to `src/Skedular.slnx` via `git mv`. No content
edits required.

**Rationale** (from spec clarification session): Because all fourteen domains move into `src/`
together, the existing relative project paths (e.g., `booking/apis/Booking.Api/...`) resolve
correctly from within `src/`. Moving the solution file into `src/` makes `src/` a
self-contained build root and matches the DSST pattern.

**Downstream consumers that reference `Skedular.slnx` from the repo root require updates**:

- `Makefile` (dep target): `dotnet restore Skedular.slnx` → `dotnet restore src/Skedular.slnx`
- `scripts/lint.sh`: `dotnet jb inspectcode Skedular.slnx` → `dotnet jb inspectcode src/Skedular.slnx`
- `scripts/format.sh`: `dotnet jb cleanupcode Skedular.slnx` → `dotnet jb cleanupcode src/Skedular.slnx`
- `.github/workflows/copilot-setup-steps.yml`: `dotnet restore Skedular.slnx` → `dotnet restore src/Skedular.slnx`

### D4: Docker Compose Files

**Decision**: `docker-compose.yml`, `docker-compose-min.yml`, `docker-compose-crm.yml`, and
`docker-compose-production.yml` require NO updates.

**Rationale**: All four files contain only infrastructure service definitions (PostgreSQL, Redis,
Kafka, Temporal, Zipkin, etc.). None contain `build:` sections with `context:` or `dockerfile:`
entries pointing into moved domain directories.

### D5: Domain Dockerfiles (41 files)

**Decision**: All 41 Dockerfiles inside moved directories need `WORKDIR` path updates.
In addition, any container-absolute path string referencing a moved domain also needs updating.

**Rationale**: Docker builds run with build context `.` (repo root) via the
`build-test-push/action.yml` composite action. The Skedular Dockerfile pattern is:

```dockerfile
WORKDIR "/src"       # container absolute path — the container's /src directory
COPY . .             # copies repo root into container /src
WORKDIR "/src/<domain>/path/to/project"  # navigate to project in container
```

After the move, `<domain>/` lives at `src/<domain>/` in the repo root, so `COPY . .`
places it at container `/src/src/<domain>/`. The WORKDIR must be updated:

```text
WORKDIR "/src/<domain>/..."  →  WORKDIR "/src/src/<domain>/..."
```

Any other absolute container path string like `/src/gateway/apis/Gateway/gateway.far` (found in
`all-in-one/AllApis/Dockerfile`) must also gain the `src/` segment.

**Alternatives considered**: Changing `COPY . .` to `COPY src/ .` — rejected because
`.config/dotnet-tools.json`, `.git`, and other root files outside `src/` are needed by the
build (e.g., `dotnet tool restore` reads `.config/dotnet-tools.json` at the project level, and
the `verify-gateway-far.sh` script needs `.github/`).

### D6: api-definitions/openapi Dockerfiles

**Decision**: `clients.Dockerfile` — no changes. `services.Dockerfile` — COPY source paths
need updating.

**Rationale**: These are built with build context `../../` (repo root).

- `clients.Dockerfile` only copies `api-definitions/openapi` which stays at root. ✓
- `services.Dockerfile` has explicit `COPY ["shared/...", ...]` entries. After the move,
  `shared/` is at `src/shared/`, so each COPY source needs the `src/` prefix:
  ```
  COPY ["shared/Api.Shared", "shared/Api.Shared"]          →  COPY ["src/shared/Api.Shared", "shared/Api.Shared"]
  COPY ["shared/Enterprise.Shared", "shared/Enterprise.Shared"]  →  COPY ["src/shared/Enterprise.Shared", "shared/Enterprise.Shared"]
  COPY ["shared/Skedularctl", "shared/Skedularctl"]          →  COPY ["src/shared/Skedularctl", "shared/Skedularctl"]
  ```

### D7: GitHub Actions Workflows

**Decision**: All 21 workflow files under `.github/workflows/` need path updates. The
`copilot-setup-steps.yml` is the only file that requires an update outside the domain path
filters (it uses `hashFiles('web/pnpm-lock.yaml')`).

**Rationale**: Workflows are anchored at the repo root. Every `paths:` filter, `workingDirectory`,
`dockerFilePath`, and inline path string that references a moved domain needs the `src/` prefix.

**Summary of workflow change categories**:

1. `on.push.paths` / `on.pull_request.paths` filters: `<domain>/**` → `src/<domain>/**`
2. `working-directory:` / `workingDirectory:` steps: `./<domain>/...` → `./src/<domain>/...`
3. `dockerFilePath:` entries: `./<domain>/...` → `./src/<domain>/...`
4. `hashFiles()` expressions: `web/pnpm-lock.yaml` → `src/web/pnpm-lock.yaml`
5. Comment prose: update for accuracy (non-blocking but clean)

### D8: scripts/generate-graphql.sh

**Decision**: All `${BASE_DIR}/<domain>/...` absolute path constructions must be updated.
Relative paths used from within a moved directory do NOT need updating.

**Rationale**: `BASE_DIR` is computed as the repository root (script's parent's parent). Every
`export_schema "${BASE_DIR}/booking/..."` call uses an absolute path anchored at `BASE_DIR`
and therefore needs `src/` inserted.

The nitro fusion compose command uses relative paths from within `src/gateway/apis/Gateway`:

```bash
cd "${BASE_DIR}/src/gateway/apis/Gateway"
-f ../../../booking/apis/Booking.Api/schema.graphqls   # still valid after move
```

Going three levels up from `src/gateway/apis/Gateway` lands at `src/`, then `booking/...` is
`src/booking/...`. This remains self-consistent — **no change needed** for the `-f` relative
args.

The `git checkout -- ./.graphqlrc.json` commands use relative paths from within the moved
directory — **no change needed**.

### D9: scripts/update-dotnet-tools.sh

**Decision**: All `"<domain>/..."` path entries in the script's project list must be updated
with the `src/` prefix.

**Rationale**: The script iterates a hardcoded list of project subdirectory paths relative to
the repo root. All 47 entries reference moved domains.

### D10: Web-Referencing Scripts (validate-\*.sh, verify-ui-package-versions.sh)

**Decision**: `scripts/validate-three-products.sh`, `scripts/validate-workspace-layout.sh`,
and `scripts/verify-ui-package-versions.sh` need `web/` → `src/web/` path updates.

**Rationale**: These scripts compute `root` as the repo root using `$(dirname)/..`. They then
construct paths like `$root/web/apps/$app`. Since `web/` moves to `src/web/`, the path must
become `$root/src/web/apps/$app`.

### D11: .vscode Configuration Files

**Decision**: `.vscode/tasks.json` and `.vscode/launch.json` need updates for all `all-in-one/`
and `web/` path references (per FR-018).

**Rationale**: These files contain `${workspaceFolder}/all-in-one/AllInOne/...` and
`${workspaceFolder}/web/apps/webapp/...` entries. Both `all-in-one/` and `web/` move into
`src/`, so paths must become `${workspaceFolder}/src/all-in-one/...` and
`${workspaceFolder}/src/web/...`.

### D12: Makefile

**Decision**: Three change categories in `Makefile`:

1. `dotnet restore Skedular.slnx` → `dotnet restore src/Skedular.slnx`
2. `./web/apps/webapp/scripts/generate.sh` → `./src/web/apps/webapp/scripts/generate.sh`
3. Same pattern for `webapp-spaces` and `webapp-teams` generate.sh calls.

**Rationale**: All three paths are anchored from the repo root in the Makefile's `dep` and
`generate` targets.

---

## Files That Do NOT Need Updates

| File / Category                                    | Reason                                        |
| -------------------------------------------------- | --------------------------------------------- |
| All `.csproj` `<ProjectReference>` entries         | Relative-path self-consistency (D1)           |
| Domain-level `.slnx` files inside moved dirs       | Relative-path self-consistency (D2)           |
| `docker-compose*.yml` (4 files)                    | No domain build contexts (D4)                 |
| `api-definitions/openapi/clients.Dockerfile`       | Only references `api-definitions/` (D6)       |
| `api-definitions/openapi/generate.sh`              | Only calls `./openapi/generate.sh` (relative) |
| `.pre-commit-config.yaml`                          | No domain path references                     |
| `.terraformignore`                                 | Only Terraform-specific patterns              |
| `scripts/delete-all-workflow-runs.sh`              | No domain path references                     |
| `scripts/start-dependencies*.sh`                   | No domain path references                     |
| `scripts/update-web-npm-packages.sh`               | No domain path references                     |
| `api-definitions/events/`, `api-definitions/grpc/` | Stay at root, no domain paths                 |

---

## Unresolved Items

None. All questions from the spec clarification session are resolved.
