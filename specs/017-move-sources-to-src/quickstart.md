# Quickstart: Move Domain Sources Into src Directory

**Feature**: `017-move-sources-to-src`

This guide describes the end-to-end execution sequence for a developer implementing this
feature. It is a developer-facing runbook, not a user-facing product guide.

---

## Prerequisites

- On branch `017-move-sources-to-src`
- Git working tree is clean: `git status` shows nothing uncommitted
- `dotnet` CLI available and `.NET 10 SDK` installed
- Docker available (for smoke-testing compose validation)

---

## Phase A — Atomic git mv Commit

Move all 14 domain directories and `Skedular.slnx` to `src/` in one commit. No file content
is changed in this commit.

```bash
mkdir -p src

# Move domain directories
domains=(all-in-one booking core customer gateway location marketplace msteams organization shared slack system team web)
for domain in "${domains[@]}"; do
  git mv "$domain" "src/$domain"
done

# Move the solution file
git mv Skedular.slnx src/Skedular.slnx

# Verify: expect only "renamed" status, zero modified
git status --short | wc -l

git commit -m "chore: move domain sources into src/"
```

**Checkpoint**: `ls src/` shows all 14 domains + `Skedular.slnx`. None of the domain directories
exist at the repository root.

---

## Phase B — Path Reference Fix Commits

Update all files anchored outside `src/`. Can be done in one commit or split by logical group.

### B1: Makefile

```bash
# dep target: dotnet restore
sed -i 's/dotnet restore Skedular\.slnx/dotnet restore src\/Skedular.slnx/' Makefile

# generate target: web app scripts
sed -i 's/\.\/web\/apps\//\.\/src\/web\/apps\//g' Makefile
```

### B2: scripts/generate-graphql.sh

Replace every `${BASE_DIR}/<domain>` with `${BASE_DIR}/src/<domain>` (24 occurrences).
Domains affected: `gateway`, `booking`, `core`, `customer`, `location`, `marketplace`,
`msteams`, `organization`, `slack`, `team`, `system`.

The relative `-f ../../../<domain>/...` arguments in the `nitro fusion compose` call do NOT
change — they are self-consistent relative paths from within the moved gateway directory.

### B3: scripts/update-dotnet-tools.sh

Prefix all 47 `"<domain>/..."` project path entries with `src/`.

### B4: scripts/lint.sh, scripts/format.sh

Change `Skedular.slnx` → `src/Skedular.slnx` in both files.

### B5: scripts/validate-three-products.sh, validate-workspace-layout.sh, verify-ui-package-versions.sh

Change `$root/web/apps` → `$root/src/web/apps` in all three files.

### B6: api-definitions/openapi/services.Dockerfile

Change the three `COPY ["shared/...", ...]` source paths to `COPY ["src/shared/...", ...]`.

### B7: All 41 domain Dockerfiles

For each Dockerfile at `src/<domain>/.../Dockerfile`:

- `WORKDIR "/src/<domain>/..."` → `WORKDIR "/src/src/<domain>/..."`
- Any other container-absolute path like `/src/<domain>/...` → `/src/src/<domain>/...`

### B8: .github/workflows/ (21 files)

For every workflow file, update:

1. `paths:` filter entries: `- "<domain>/**"` → `- "src/<domain>/**"`
2. `workingDirectory:` entries: `./<domain>/...` → `./src/<domain>/...`
3. `dockerFilePath:` entries: `./<domain>/...` → `./src/<domain>/...`
4. In `copilot-setup-steps.yml`: `Skedular.slnx` → `src/Skedular.slnx`, `web/pnpm-lock.yaml` → `src/web/pnpm-lock.yaml`

### B9: .vscode/tasks.json, .vscode/launch.json

Update all `${workspaceFolder}/all-in-one/...` → `${workspaceFolder}/src/all-in-one/...`
and `${workspaceFolder}/web/apps/...` → `${workspaceFolder}/src/web/apps/...`.

### B10: Prose documentation

Update `README.md`, `AGENTS.md`, `CLAUDE.md`, `.github/copilot-instructions.md`:

- All `## Project Structure` or equivalent sections showing domain directory layout
- All prose path references like `booking/`, `shared/`, `gateway/`, etc. that refer to the
  moved locations → `src/booking/`, `src/shared/`, `src/gateway/`, etc.

Commit all Phase B changes:

```bash
git add -A
git commit -m "chore: update path references for src/ domain layout"
```

---

## Verification

```bash
# 1. No domain directories at root
for d in all-in-one booking core customer gateway location marketplace msteams organization shared slack system team web; do
  [ -d "$d" ] && echo "STILL AT ROOT: $d"
done

# 2. Solution file moved
ls src/Skedular.slnx

# 3. Build from repo root
dotnet restore src/Skedular.slnx
dotnet build src/Skedular.slnx --no-restore

# 4. Compose validation
docker compose -f docker-compose.yml config
docker compose -f docker-compose-min.yml config

# 5. No stale domain paths remain in tooling files
grep -rn '"booking/\|"shared/\|"gateway/\|./booking\|./shared\|./gateway\|./web/' \
  .github/workflows/ scripts/ Makefile api-definitions/openapi/ .vscode/ 2>/dev/null \
  | grep -v "src/"
# Should return zero matches
```
