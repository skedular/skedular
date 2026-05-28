# Feature Specification: Move Domain Sources Into src Directory

**Feature Branch**: `017-move-sources-to-src`
**Created**: 2026-05-28
**Status**: Draft

## Clarifications

### Session 2026-05-28

- Q: Should `Skedular.slnx` stay at the repository root with paths updated, or move into `src/` matching the DSST pattern? → A: Move into `src/Skedular.slnx` — peer-relative paths remain self-consistent with no path edits needed inside the file.

## User Scenarios & Testing _(mandatory)_

### User Story 1 — Domain Directories Relocated Under src/ (Priority: P1)

A developer cloning the repository for the first time finds a clean, predictable root layout
where all domain source trees (`all-in-one`, `booking`, `core`, `customer`, `gateway`,
`location`, `marketplace`, `msteams`, `organization`, `shared`, `slack`, `system`, `team`,
`web`) live under a single `src/` directory. Root-level entries are limited to configuration,
tooling, documentation, and build artifacts — not domain source trees. The developer can
navigate to any domain with one extra path segment (`src/<domain>/`) instead of scanning a
flat list of mixed-purpose directories.

**Why this priority**: Root cleanliness is the stated goal of this feature. All other stories
depend on the move being completed first.

**Independent Test**: Clone the repository after the changes are merged. Verify that `ls ./src`
lists the moved domains and that those directories no longer exist at the root. Verify that
`ls ./` does not contain any of the fourteen domain directories.

**Acceptance Scenarios**:

1. **Given** the repository is on the main branch after merge, **When** a developer lists the
   root directory, **Then** none of the following are present at root: `all-in-one/`, `booking/`,
   `core/`, `customer/`, `gateway/`, `location/`, `marketplace/`, `msteams/`, `organization/`,
   `shared/`, `slack/`, `system/`, `team/`, `web/`.
2. **Given** the root directory is listed, **When** the developer looks for `src/`, **Then** it
   exists and contains exactly those fourteen directories.
3. **Given** the moved layout, **When** the developer lists the directories that must remain at
   root (`docs/`, `specs/`, `api-definitions/`, `scripts/`, `assets/`), **Then** all five are
   still at the root unchanged.
4. **Given** the moved layout, **When** the developer opens any moved directory, **Then** all
   internal files and sub-directories are intact with no content changes.

---

### User Story 2 — Solution File and Project References Updated (Priority: P1)

A developer opens `Skedular.slnx` in an IDE and all projects load without path errors. Every
`<ProjectReference>` and `<Import>` inside `.csproj` / `.slnx` files that previously used paths
like `../../shared/…` correctly resolves under the new `src/<domain>/…` layout.

**Why this priority**: Without this, the entire build is broken and no development work can
proceed after the move.

**Independent Test**: Run `dotnet build src/Skedular.slnx` from the repository root after the move
and confirm the build completes with zero unresolved project reference errors.

**Acceptance Scenarios**:

1. **Given** `Skedular.slnx` has moved into `src/`, **When** `dotnet build src/Skedular.slnx` is
   executed from the repository root, **Then** all projects resolve and the build completes
   without reference errors.
2. **Given** any `.csproj` that contains `<ProjectReference>` paths crossing domain
   boundaries, **When** the reference is resolved at build time, **Then** the relative path
   correctly traverses through `src/` and finds the target project.
3. **Given** domain-specific `.slnx` files inside moved directories (e.g., within
   `src/booking/`, `src/team/`), **When** they are opened in an IDE, **Then** all project paths
   within them resolve correctly — peer-relative paths (e.g., `../shared/`) remain valid because
   both sides moved into `src/` together and require no further edits.

---

### User Story 3 — Docker Compose Files and Dockerfiles Resolve Correctly (Priority: P1)

An operator running `docker compose up` from the repository root after the move gets the same
containers as before. All Dockerfiles inside moved directories that referenced build-context
paths into other domains now reference the new `src/<domain>/` paths correctly.

**Why this priority**: CI/CD and local development depend on Docker being functional. Broken
compose files or Dockerfiles block integration testing and deployment.

**Independent Test**: Run `docker compose -f docker-compose.yml config` and `docker compose -f
docker-compose-min.yml config` from the repository root and confirm both complete with zero path
resolution errors.

**Acceptance Scenarios**:

1. **Given** `docker-compose.yml`, `docker-compose-min.yml`, `docker-compose-crm.yml`, and
   `docker-compose-production.yml` at the root, **When** `docker compose config` is run for
   each, **Then** all complete with zero errors for entries that do not reference moved
   directories.
2. **Given** any Dockerfile inside a moved domain that uses `dotnet restore` or `dotnet publish`
   with repository-root-relative paths, **When** the Docker build runs, **Then** all path
   arguments resolve correctly under `src/<domain>/`.
3. **Given** the `.dockerignore` at the repository root, **When** it contains path exclusions
   referencing moved domain directories, **Then** those entries are updated to `src/<domain>/`
   so image builds exclude the correct paths.
4. **Given** the full compose stack, **When** `docker compose up --build` runs, **Then** all
   images build and start without build-time path errors.

---

### User Story 4 — CI/CD Pipelines and GitHub Actions Workflows Updated (Priority: P1)

An automated pipeline triggered by a push to main runs all jobs successfully after the move.
Every GitHub Actions workflow step that references a directory or file within a moved domain
(e.g., working directories, path filters, artifact paths, build contexts) now uses the
`src/<domain>/…` equivalent path.

**Why this priority**: A broken pipeline means no automated validation, no releases, and blocked
collaboration.

**Independent Test**: After merging, push a commit and verify all GitHub Actions workflow runs
complete green without path-not-found errors in any step.

**Acceptance Scenarios**:

1. **Given** the workflows under `.github/workflows/` that previously referenced root-level
   domain directories, **When** any step that uses such a path runs, **Then** it uses the
   updated `src/<domain>/` path and succeeds.
2. **Given** `on.push.paths` or `on.pull_request.paths` filters in workflow YAML files that
   listed paths like `booking/**` or `shared/**`, **When** a change is made to a file in
   `src/booking/`, **Then** the correct workflow triggers.
3. **Given** artifact upload/download steps that reference domain build output paths, **When**
   pipelines run, **Then** artifacts are located and uploaded without errors.
4. **Given** the `copilot-instructions.md` and `AGENTS.md` referenced from workflows or
   automation tooling, **When** those files contain path references to moved domains, **Then**
   those references are updated.

---

### User Story 5 — Generation Scripts Work with New Paths (Priority: P2)

A developer running `api-definitions/generate.sh` or `scripts/generate-graphql.sh` after the
move gets the same generated output as before. Scripts that previously used root-relative paths
to domain source files now resolve those files correctly under `src/`.

**Why this priority**: Code generation is a critical workflow. Broken generators force developers
to hand-edit generated files, which project policy explicitly forbids.

**Independent Test**: Run `api-definitions/generate.sh` from the repository root on the updated
branch and verify generated files are written to their expected output locations with no
path-not-found errors.

**Acceptance Scenarios**:

1. **Given** `api-definitions/generate.sh` is run from the repository root, **When** it
   resolves input contracts or output target paths that reference moved directories, **Then** it
   finds and writes files correctly under `src/`.
2. **Given** `scripts/generate-graphql.sh` is run from the repository root, **When** it
   resolves schema export targets and per-domain project paths, **Then** it locates projects
   under `src/` and writes composed schema files to their expected locations.
3. **Given** the web app generation script `web/apps/webapp/scripts/generate.sh`, **When** it
   is run, **Then** it completes without path errors (note: `web/` is now under `src/web/`).
4. **Given** any other script under `scripts/` that references a moved domain path, **When** it
   is run, **Then** it completes without path errors.
5. **Given** the `Makefile` at the repository root that calls generation and build scripts,
   **When** any make target is invoked, **Then** it resolves all paths correctly under `src/`.

---

### User Story 6 — Documentation and Prose References Updated (Priority: P2)

A developer reading `README.md`, `AGENTS.md`, `CLAUDE.md`, `.github/copilot-instructions.md`,
or any other root-level documentation file finds that all directory examples, path listings, and
project structure diagrams reflect the new `src/<domain>/` layout. No stale references to
root-level domain paths remain.

**Why this priority**: Stale documentation misleads contributors and undermines the value of the
structural change.

**Independent Test**: Search the repository for any occurrence of a root-level domain directory
name (e.g., `booking/`, `shared/`) outside of `src/` and `specs/` — none should appear as
directory path references in documentation or configuration prose after the move.

**Acceptance Scenarios**:

1. **Given** `README.md`, `AGENTS.md`, `CLAUDE.md`, and `.github/copilot-instructions.md`,
   **When** a developer reads the project structure sections, **Then** all domain directory
   examples read `src/<domain>/` rather than `<domain>/`.
2. **Given** any root-level prose file, **When** it contains a path reference to a moved domain,
   **Then** that reference uses the `src/<domain>/` prefix.
3. **Given** the `## Project Structure` code block in `copilot-instructions.md`, **When** a
   developer reads it, **Then** it accurately reflects the post-move layout with `src/` as the
   parent of all domain directories.

---

### Edge Cases

- What if a script computes a path relative to `$SCRIPT_ROOT` or `__DIR__` and already resolves
  correctly after the move? Those paths need no change.
- What if a `.csproj` uses a `<PackagePath>`, `<ContentRoot>`, or other metadata entry that is
  root-relative? Those entries must be reviewed and updated.
- What if Aspire `AppHost` projects in `all-in-one/` reference sibling domain projects by
  relative path? Those references cross domain boundaries and must be recalculated for the new
  `src/` depth.
- What if `NuGet.config` has `fallbackPackageFolders` or source paths that are root-relative?
  Those entries must be updated.
- What if `.gitignore` or `.dockerignore` contains patterns matching moved domain paths? Those
  patterns must be updated to `src/<domain>/` equivalents.
- What if Terraform or infrastructure config files under `.config/` reference moved domain paths?
  Those references must be updated.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The directories `all-in-one`, `booking`, `core`, `customer`, `gateway`, `location`,
  `marketplace`, `msteams`, `organization`, `shared`, `slack`, `system`, `team`, and `web` MUST
  be relocated from the repository root to `src/<directory-name>`.
- **FR-002**: The directories `api-definitions`, `assets`, `docs`, `scripts`, and `specs` MUST
  remain at the repository root unchanged.
- **FR-003**: All root-level configuration and documentation files (`AGENTS.md`, `CLAUDE.md`,
  `README.md`, `QWEN.md`, `Makefile`, `docker-compose*.yml`, dotfiles, and hidden directories
  `.github`, `.specify`, `.vscode`, `.config`, `.agents`) MUST remain at the root. Their
  prose and JSON content that references moved domain paths MUST be updated to reflect the new
  `src/<domain>/` paths.
- **FR-004**: `Skedular.slnx` MUST be moved from the repository root into `src/Skedular.slnx`.
  Because all domain source trees move into `src/` together, its existing project paths (e.g.,
  `booking/apis/Booking.Api/Booking.Api.csproj`) remain correct relative to `src/` with no
  content edits required.
- **FR-005**: Domain-specific `.slnx` files inside moved domains use peer-relative paths (e.g.,
  `../shared/`) that remain correct after the move because all referenced peers also move into
  `src/` together. These files require no path updates.
- **FR-006**: Every `.csproj` file that contains `<ProjectReference>` paths crossing domain
  boundaries uses relative paths that remain correct after the move due to relative-path
  self-consistency — all referenced domains move into `src/` together. These files require no
  path updates.
- **FR-007**: `docker-compose.yml`, `docker-compose-min.yml`, `docker-compose-crm.yml`, and
  `docker-compose-production.yml` that contain only infrastructure service definitions with no
  `build:` sections pointing into moved directories require no updates. Any that do reference
  moved directories MUST be updated.
- **FR-008**: Every Dockerfile inside a moved directory that uses `dotnet restore`, `dotnet
publish`, or `COPY` with paths relative to the build context root MUST have those path
  arguments updated to include the `src/` prefix. Additionally, any Dockerfile outside a moved
  directory (e.g., `api-definitions/openapi/services.Dockerfile`) that uses root-relative `COPY`
  source paths into a moved domain MUST also be updated.
- **FR-009**: Every GitHub Actions workflow file under `.github/workflows/` that references a
  moved directory by path MUST be updated to the `src/<domain>/` equivalent. This includes
  `working-directory` fields, `paths` filters, artifact paths, and any inline path strings.
- **FR-010**: `api-definitions/generate.sh` and `scripts/generate-graphql.sh` MUST be updated
  wherever they reference paths into moved directories so they produce identical outputs from
  the repository root.
- **FR-011**: The `Makefile` at the repository root MUST be updated wherever it references paths
  into moved directories.
- **FR-012**: Any other script under `scripts/` that references a moved domain path MUST be
  updated.
- **FR-013**: `Aspire` `AppHost` project files in `all-in-one/` that reference sibling projects
  by relative path require NO updates. Because all domains move into `src/` together, the
  existing relative paths (e.g., `../../booking/`) remain self-consistent — see research.md D1.
- **FR-014**: All existing automated tests (unit, integration, system) MUST pass without
  modification to test logic — only path references in project and configuration files change.
- **FR-015**: Prose path references inside `README.md`, `AGENTS.md`, `CLAUDE.md`,
  `.github/copilot-instructions.md`, and any other root-level documentation that mention moved
  domain directories MUST be updated to use `src/<domain>/` paths.
- **FR-016**: `.gitignore` and `.dockerignore` entries that reference moved domain directory
  patterns MUST be updated to their `src/<domain>/` equivalents.
- **FR-017**: Every file move MUST be performed via `git mv` (not OS move followed by
  `git add -A`) to preserve rename history in `git log --follow`.
- **FR-018**: `.vscode/tasks.json` and `.vscode/launch.json` MUST be updated wherever they
  contain `${workspaceFolder}/all-in-one/…` or `${workspaceFolder}/web/…` path references,
  replacing them with `${workspaceFolder}/src/all-in-one/…` and `${workspaceFolder}/src/web/…`
  respectively so that VS Code launch and task configurations continue to work after the move.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: This is a structural refactor with no runtime behavior changes. No new
  application log events are required.
- **LOG-002**: CI/CD pipeline step logs MUST reflect updated paths after the change — no
  step output should reference stale root-level domain paths in warnings or errors.
- **LOG-003**: Build output from `dotnet build` MUST complete cleanly with no unresolved
  reference warnings that would indicate a missed path update.
- **LOG-004**: Generation script output MUST indicate successful completion without path errors
  to confirm all source-path references in generators are updated.

### Key Entities

- **Root layout**: The set of entries visible at repository root — defines discoverability and
  contributor first impression.
- **src/ directory**: New top-level container holding all domain source trees. Acts as the single
  entry point for all compilable code.
- **Domain directory**: One of the fourteen source trees (`all-in-one`, `booking`, `core`,
  `customer`, `gateway`, `location`, `marketplace`, `msteams`, `organization`, `shared`, `slack`,
  `system`, `team`, `web`) being relocated.
- **Cross-domain project reference**: A `<ProjectReference>` in a `.csproj` that points from one
  domain directory into another. These retain self-consistent relative paths because all domains
  move together into `src/`.
- **Generation script**: Shell scripts (`api-definitions/generate.sh`,
  `scripts/generate-graphql.sh`, `web/apps/webapp/scripts/generate.sh`) that traverse the source
  tree and emit generated files. Path correctness is critical; these stay at their current root
  locations but their internal path strings must be updated.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: `dotnet build src/Skedular.slnx` from the repository root completes with zero
  errors and zero unresolved project reference warnings after the move.
- **SC-002**: `docker compose config` for each of the four compose files completes with zero
  path resolution errors.
- **SC-003**: All GitHub Actions workflow runs triggered on the feature branch complete green
  with no path-not-found failures in any step.
- **SC-004**: `api-definitions/generate.sh` and `scripts/generate-graphql.sh` both complete
  without errors and produce output identical to a baseline run on main before the move.
- **SC-005**: The repository root contains none of the fourteen domain source directories and a
  new `src/` directory exists containing all fourteen.
- **SC-006**: All existing automated test suites (unit, integration, system) pass without any
  modification to test code or test configuration.
- **SC-007**: `README.md`, `AGENTS.md`, `CLAUDE.md`, and `.github/copilot-instructions.md`
  contain no stale references to root-level domain paths after the merge — all such references
  read `src/<domain>/`.
- **SC-008**: `make generate` (the umbrella generation command) completes without errors and
  produces identical output to the pre-move baseline.

## Assumptions

- All fourteen domain directories are moved in a single atomic `git mv` commit. Path reference
  fixes (solution file, workflows, scripts, Dockerfiles, prose) follow in one or more separate
  commits.
- `api-definitions`, `assets`, `docs`, `scripts`, and `specs` are explicitly excluded from the
  move as they are cross-cutting concerns and tooling anchors, not domain source trees.
- Root-level dotfiles and hidden directories (`.github`, `.vscode`, `.specify`, `.config`,
  `.agents`, `.git`) are not moved.
- Root-level loose files (`AGENTS.md`, `CLAUDE.md`, `README.md`, `QWEN.md`, `Makefile`,
  `docker-compose*.yml`, `.editorconfig`, `.gitignore`, `.gitattributes`, `.dockerignore`,
  `.env`, `.env.template`, `keyckloak.json`, `qodana.yaml`) stay at the root. Their content
  referencing domain paths is updated in the reference-fix commit(s).
- `Skedular.slnx` moves from the repository root into `src/Skedular.slnx`. Its internal
  project paths require no edits because all domains move into `src/` together, keeping
  peer-relative paths self-consistent.
- Cross-domain `.csproj` `<ProjectReference>` paths retain self-consistent relative paths
  because all referenced domains move together — no `.csproj` edits are required.
- The DSST repository's completed `042-move-sources-to-src` feature serves as the authoritative
  implementation reference for this feature.
- No external tooling outside this repository holds hardcoded paths into the moved directories
  that would require out-of-band updates.
- All path updates are mechanical (find-and-replace of path prefixes) rather than logic changes,
  so no functional behavior changes.
- `system/` is a source domain containing system/integration tests and moves to `src/system/`
  alongside the other domains.
- `web/` contains the frontend applications and web tests and moves to `src/web/` alongside the
  other domains.
