# Tasks: Merge CI/CD Pipelines

**Input**: Design documents from `/specs/024-merge-ci-cd-pipelines/`
**Prerequisites**: [plan.md](plan.md), [spec.md](spec.md), [research.md](research.md), [data-model.md](data-model.md), [contracts/consolidated-pipeline-contract.md](contracts/consolidated-pipeline-contract.md), [quickstart.md](quickstart.md)

**Tests**: No separate test project is requested. Validation tasks use workflow/action linting and the quickstart path-change matrix.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish the workflow inventory and consolidated workflow shell.

- [X] T001 Create current workflow inventory and retirement mapping in specs/024-merge-ci-cd-pipelines/workflow-inventory.md
- [X] T002 Create initial consolidated workflow scaffold in .github/workflows/skedular-cicd-pipeline.yml
- [X] T003 [P] Add CI/CD parity validation checklist placeholders in specs/024-merge-ci-cd-pipelines/quickstart.md
- [X] T004 [P] Verify existing composite action inputs are documented in specs/024-merge-ci-cd-pipelines/workflow-inventory.md

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core consolidated workflow behavior required before any user story can be implemented.

**CRITICAL**: No user story work can begin until this phase is complete.

- [X] T005 Configure workflow triggers, permissions, concurrency, and base environment in .github/workflows/skedular-cicd-pipeline.yml
- [X] T006 Implement detect job checkout, base SHA resolution, changed-file collection, and first-push handling in .github/workflows/skedular-cicd-pipeline.yml
- [X] T007 Implement detect job trigger group outputs for global, backend, web, docs, infrastructure, and concrete deployable targets in .github/workflows/skedular-cicd-pipeline.yml
- [X] T008 Implement docs-only override and manual full-run behavior in .github/workflows/skedular-cicd-pipeline.yml
- [X] T009 Implement dependency fan-out rules for pipeline, shared backend, API definitions, web workspace, and shared infrastructure changes in .github/workflows/skedular-cicd-pipeline.yml
- [X] T010 Add global lint validation equivalent to current lint workflow in .github/workflows/skedular-cicd-pipeline.yml
- [X] T011 Add required umbrella result job skeleton with selected-job result aggregation in .github/workflows/skedular-cicd-pipeline.yml

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel.

---

## Phase 3: User Story 1 - Validate Relevant Changes Through One Required Pipeline (Priority: P1) MVP

**Goal**: One required pipeline validates only the CI segments relevant to changed folders while preserving existing validation coverage.

**Independent Test**: Open representative pull requests for one product/domain, a shared dependency, and docs-only changes; confirm one required pipeline result is reported and only expected CI segments run.

### Implementation for User Story 1

- [X] T012 [P] [US1] Add all-in-one Docker CI jobs for allapis, allprocessors, alljobs, and allinfra in .github/workflows/skedular-cicd-pipeline.yml
- [X] T013 [P] [US1] Add web app Docker CI jobs for webapp, webapp-help, webapp-teams, webapp-teams-help, webapp-spaces, and webapp-spaces-help in .github/workflows/skedular-cicd-pipeline.yml
- [X] T014 [P] [US1] Add docs event catalog Docker CI job in .github/workflows/skedular-cicd-pipeline.yml
- [X] T015 [US1] Add Terraform validation CI jobs for shared, shared Azure Entra, and domain shared infrastructure workspaces in .github/workflows/skedular-cicd-pipeline.yml
- [X] T016 [US1] Wire CI job `if:` conditions to detect outputs and fan-out rules in .github/workflows/skedular-cicd-pipeline.yml
- [X] T017 [US1] Ensure selected CI job failures fail the umbrella result and unselected skipped jobs do not fail it in .github/workflows/skedular-cicd-pipeline.yml
- [X] T018 [US1] Add representative CI path-change scenarios for docs-only, single web app, web package, backend shared, API definitions, and pipeline/action changes in specs/024-merge-ci-cd-pipelines/quickstart.md
- [X] T019 [US1] Validate the CI-only behavior against the DSST reference notes and record deviations in specs/024-merge-ci-cd-pipelines/workflow-inventory.md

**Checkpoint**: User Story 1 is independently functional: the consolidated workflow can serve as the MVP CI gate without deploying.

---

## Phase 4: User Story 2 - Preserve Existing Deployment Coverage (Priority: P2)

**Goal**: Add Skedular CD behavior after selected CI checks pass, because DSST is only a CI reference and does not provide CD behavior to copy.

**Independent Test**: Compare consolidated deployable segments against the existing Skedular workflow inventory and confirm every current staging and production deployable surface is represented with PR deploy suppression.

### Implementation for User Story 2

- [X] T020 [P] [US2] Add staging CD jobs for webapp, webapp-help, webapp-teams, webapp-teams-help, webapp-spaces, and webapp-spaces-help in .github/workflows/skedular-cicd-pipeline.yml
- [X] T021 [P] [US2] Add production CD jobs for webapp, webapp-help, webapp-teams, webapp-teams-help, webapp-spaces, and webapp-spaces-help in .github/workflows/skedular-cicd-pipeline.yml
- [X] T022 [P] [US2] Add staging and production CD jobs for docs event catalog in .github/workflows/skedular-cicd-pipeline.yml
- [X] T023 [US2] Add staging CD jobs for shared, shared Azure Entra, booking, customer, location, msteams, organization, slack, and team infrastructure workspaces in .github/workflows/skedular-cicd-pipeline.yml
- [X] T024 [US2] Add production CD jobs for shared, shared Azure Entra, booking, customer, location, msteams, organization, slack, and team infrastructure workspaces in .github/workflows/skedular-cicd-pipeline.yml
- [X] T025 [US2] Wire all CD jobs to depend on their matching selected CI validation jobs in .github/workflows/skedular-cicd-pipeline.yml
- [X] T026 [US2] Enforce pull request deploy suppression and `main`-only staging eligibility in .github/workflows/skedular-cicd-pipeline.yml
- [X] T027 [US2] Preserve production environment gates, approvals, and existing production conditions in .github/workflows/skedular-cicd-pipeline.yml
- [X] T028 [US2] Record CD parity for every staging and production target in specs/024-merge-ci-cd-pipelines/workflow-inventory.md
- [X] T029 [US2] Add quickstart validation cases for PR no-deploy, `main` staging deploy eligibility, and production gated deploy behavior in specs/024-merge-ci-cd-pipelines/quickstart.md

**Checkpoint**: User Stories 1 and 2 work together: selected CI runs first, then selected CD becomes eligible only under the documented event and environment policy.

---

## Phase 5: User Story 3 - Make Change Detection Auditable (Priority: P3)

**Goal**: Make every run/skip, fan-out, docs-only, manual-run, and CD policy decision visible in workflow output.

**Independent Test**: Run representative changes and verify the GitHub Actions summary explains changed files, trigger groups, selected/skipped CI jobs, selected/skipped CD jobs, and reasons within two minutes.

### Implementation for User Story 3

- [X] T030 [US3] Add detect job summary output for event name, ref, base SHA, head SHA, changed file count, and changed file list in .github/workflows/skedular-cicd-pipeline.yml
- [X] T031 [US3] Add trigger group summary output with active groups, inactive groups, and fan-out reasons in .github/workflows/skedular-cicd-pipeline.yml
- [X] T032 [US3] Add selected and skipped CI segment summary output in .github/workflows/skedular-cicd-pipeline.yml
- [X] T033 [US3] Add selected, skipped, blocked-by-policy, and waiting-for-approval CD segment summary output in .github/workflows/skedular-cicd-pipeline.yml
- [X] T034 [US3] Add warning and failure diagnostics for failed detection, missing changed files, failed selected jobs, and skipped required dependencies in .github/workflows/skedular-cicd-pipeline.yml
- [X] T035 [US3] Verify summary output avoids secrets, tokens, Terraform state values, and sensitive payloads in .github/workflows/skedular-cicd-pipeline.yml
- [X] T036 [US3] Add auditability validation steps to specs/024-merge-ci-cd-pipelines/quickstart.md

**Checkpoint**: All user stories are independently functional and the consolidated pipeline is explainable from the workflow summary.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Retire duplicate workflows, validate the full path matrix, and clean up documentation.

- [X] T037 Remove or disable old web app workflow files in .github/workflows/webapp.yml, .github/workflows/webapp-help.yml, .github/workflows/webapp-teams.yml, .github/workflows/webapp-teams-help.yml, .github/workflows/webapp-spaces.yml, and .github/workflows/webapp-spaces-help.yml
- [X] T038 Remove or disable old shared/domain infrastructure workflow files in .github/workflows/shared.yml, .github/workflows/shared-azure-entra.yml, .github/workflows/booking-shared.yml, .github/workflows/customer-shared.yml, .github/workflows/location-shared.yml, .github/workflows/msteams-shared.yml, .github/workflows/organization-shared.yml, .github/workflows/slack-shared.yml, and .github/workflows/team-shared.yml
- [X] T039 Remove or disable old app/support workflow files now represented by the consolidated pipeline in .github/workflows/workarounds.yml, .github/workflows/docs-event-catalog.yml, and .github/workflows/lint.yml
- [X] T040 [P] Document any intentionally retained non-CI/CD maintenance workflows in specs/024-merge-ci-cd-pipelines/workflow-inventory.md
- [X] T041 Run workflow linting against .github/workflows/skedular-cicd-pipeline.yml and record results in specs/024-merge-ci-cd-pipelines/quickstart.md
- [X] T042 Run `make lint` validation from Makefile and record results in specs/024-merge-ci-cd-pipelines/quickstart.md
- [X] T043 Execute or simulate the quickstart validation matrix and mark outcomes in specs/024-merge-ci-cd-pipelines/quickstart.md
- [X] T044 Update .github/copilot-instructions.md if implementation changes active technologies, retained workflows, or validation commands beyond the current plan

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately.
- **Foundational (Phase 2)**: Depends on Setup completion - blocks all user stories.
- **User Story 1 (Phase 3)**: Depends on Foundational completion and is the MVP.
- **User Story 2 (Phase 4)**: Depends on Foundational completion and can be implemented after or alongside US1, but final CD eligibility depends on matching CI jobs from US1.
- **User Story 3 (Phase 5)**: Depends on Foundational completion and can be developed alongside US1/US2 once job names and trigger outputs are stable.
- **Polish (Phase 6)**: Depends on desired user stories being complete, especially before old workflow retirement.

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Phase 2; no dependency on US2 or US3.
- **User Story 2 (P2)**: Starts after Phase 2; CD jobs must depend on matching CI jobs produced by US1 before release.
- **User Story 3 (P3)**: Starts after Phase 2; summary details depend on the final trigger and job names from US1/US2.

### Within Each User Story

- For US1, add CI segment families before wiring umbrella failure behavior.
- For US2, add CD segment families before enforcing final event and environment gates.
- For US3, add detect summary first, then CI/CD segment summaries, then failure diagnostics.
- Retire old workflows only after consolidated CI/CD parity is documented.

## Parallel Opportunities

- T003 and T004 can run in parallel after T001/T002 are started because they edit separate documentation/workflow surfaces.
- T012, T013, and T014 can run in parallel because they add different CI job families in the consolidated workflow.
- T020, T021, and T022 can run in parallel because they add separate CD job families.
- T040 can run in parallel with workflow linting once retained workflow decisions are known.
- US1 and US3 can overlap after foundational trigger output names are stable.

## Parallel Example: User Story 1

```text
Task: "T012 [P] [US1] Add all-in-one Docker CI jobs for allapis, allprocessors, alljobs, and allinfra in .github/workflows/skedular-cicd-pipeline.yml"
Task: "T013 [P] [US1] Add web app Docker CI jobs for webapp, webapp-help, webapp-teams, webapp-teams-help, webapp-spaces, and webapp-spaces-help in .github/workflows/skedular-cicd-pipeline.yml"
Task: "T014 [P] [US1] Add docs event catalog Docker CI job in .github/workflows/skedular-cicd-pipeline.yml"
```

## Parallel Example: User Story 2

```text
Task: "T020 [P] [US2] Add staging CD jobs for webapp, webapp-help, webapp-teams, webapp-teams-help, webapp-spaces, and webapp-spaces-help in .github/workflows/skedular-cicd-pipeline.yml"
Task: "T021 [P] [US2] Add production CD jobs for webapp, webapp-help, webapp-teams, webapp-teams-help, webapp-spaces, and webapp-spaces-help in .github/workflows/skedular-cicd-pipeline.yml"
Task: "T022 [P] [US2] Add staging and production CD jobs for docs event catalog in .github/workflows/skedular-cicd-pipeline.yml"
```

## Parallel Example: User Story 3

```text
Task: "T030 [US3] Add detect job summary output for event name, ref, base SHA, head SHA, changed file count, and changed file list in .github/workflows/skedular-cicd-pipeline.yml"
Task: "T036 [US3] Add auditability validation steps to specs/024-merge-ci-cd-pipelines/quickstart.md"
```

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup.
2. Complete Phase 2: Foundational detection, trigger outputs, fan-out, lint, and umbrella skeleton.
3. Complete Phase 3: User Story 1 CI selection and validation coverage.
4. Stop and validate docs-only, single-folder, shared dependency, and pipeline/action CI behavior.
5. Keep old deployment workflows active until US2 CD parity is complete.

### Incremental Delivery

1. Deliver US1 as the single required folder-aware CI gate.
2. Add US2 to preserve Skedular CD after selected CI succeeds.
3. Add US3 to make every run/skip/CD policy decision auditable.
4. Retire old workflows only after quickstart parity validation passes.

### Parallel Team Strategy

1. One developer owns detect and fan-out foundations.
2. One developer owns CI segment families for US1.
3. One developer owns CD segment families for US2.
4. One developer owns summary/auditability for US3 once output and job names stabilize.

## Notes

- [P] tasks indicate different files or separable workflow sections with no dependency on incomplete tasks.
- [US1], [US2], and [US3] labels map directly to spec user stories.
- Every task includes an exact repository path.
- Do not copy DSST versioning or package publishing behavior into Skedular.
- DSST is the CI reference only; Skedular CD must be sourced from existing Skedular workflows and run after matching CI checks.
