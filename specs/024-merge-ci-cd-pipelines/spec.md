# Feature Specification: Merge CI/CD Pipelines

**Feature Branch**: `024-merge-ci-cd-pipelines`  
**Created**: 2026-06-04  
**Status**: Draft  
**Input**: User description: "I need you to look into the only ci/cd pipeline in added workspace dsst, use that as reference and merge all Skedular pipelines into a single ci/cd pipeline. it should be folder aware and not build everything if not required, read dsst pipeline first, you do not need to bring the versioning from that to here, no versioning here is required for now. only merge all ci cd pipelines into one single one which is aware of folder changes"

## Clarifications

### Session 2026-06-04

- Q: Which deployment trigger behavior should the consolidated pipeline use? → A: PRs validate only; pushes to `main` may deploy staging; production uses existing production gates.
- Q: Does the DSST reference include CD behavior to copy? → A: No. DSST is currently a CI reference only; Skedular must add CD jobs after successful CI checks.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Validate Relevant Changes Through One Required Pipeline (Priority: P1)

As a contributor, I want one CI/CD pipeline to evaluate my change and run only the validation work relevant to the folders I changed, so pull requests get a clear required status without wasting time on unrelated products or domains.

**Why this priority**: This is the core value of the feature: replacing many independent pipelines with a single reliable gate that is aware of repository structure.

**Independent Test**: Can be tested by opening pull requests that change one representative folder at a time and confirming the single pipeline reports one required result while running only the expected validation segments.

**Acceptance Scenarios**:

1. **Given** a pull request changes only one product or domain folder, **When** the CI/CD pipeline runs, **Then** only that product or domain's validation segment and any required shared dependency segments run.
2. **Given** a pull request changes a shared dependency folder used by multiple products or domains, **When** the CI/CD pipeline runs, **Then** all affected downstream validation segments run.
3. **Given** a pull request changes only documentation or specification files, **When** the CI/CD pipeline runs, **Then** no build or deployment validation segments run and the single required pipeline result still completes successfully.

---

### User Story 2 - Preserve Existing Deployment Coverage (Priority: P2)

As a release owner, I want the consolidated pipeline to preserve the staging and production deployment coverage currently spread across the existing Skedular workflows, so consolidation does not remove any deployable surface.

**Why this priority**: The feature must be a merge of current CI/CD behavior, not a reduction in release capability.

**Independent Test**: Can be tested by comparing the consolidated pipeline's deployable segments against the existing workflow inventory and confirming every current deployable product, shared infrastructure area, and environment remains represented.

**Acceptance Scenarios**:

1. **Given** a pull request affects a deployable app or infrastructure area, **When** the pipeline runs, **Then** validation runs without deploying to staging or production.
2. **Given** a push to `main` affects a deployable app or infrastructure area, **When** validation succeeds, **Then** the matching staging deployment segment is eligible to run.
3. **Given** a change affects a deployable app or infrastructure area and existing production release conditions are met, **When** the pipeline completes prerequisite validation, **Then** the matching production deployment segment is eligible to run through the existing production gates.
4. **Given** a change does not affect a deployable area, **When** the pipeline runs, **Then** unrelated deployment segments do not run.

---

### User Story 3 - Make Change Detection Auditable (Priority: P3)

As a maintainer, I want the pipeline to clearly show why each segment ran or skipped, so reviewers and operators can diagnose pipeline behavior without reading every workflow rule.

**Why this priority**: Folder-aware pipelines are only trustworthy when their decisions are visible and easy to review.

**Independent Test**: Can be tested by running representative changes and reviewing the pipeline summary for changed files, trigger groups, final run/skip decisions, and reason messages.

**Acceptance Scenarios**:

1. **Given** a pipeline run with changed files, **When** change detection completes, **Then** the run summary lists the detected trigger groups and the final decision for each pipeline segment.
2. **Given** a pipeline segment is skipped, **When** a maintainer reviews the run, **Then** the summary identifies that it was skipped because no relevant files or dependencies changed.
3. **Given** a manual run is started, **When** change detection executes, **Then** the pipeline clearly states that the run is manual and all segments are included.

### Edge Cases

- First push or missing comparison base still produces deterministic change detection instead of failing before validation can begin.
- Manual pipeline runs include all validation and deployment-readiness segments unless environment approval rules prevent deployment.
- Pipeline-definition changes trigger all relevant validation segments so changes to CI/CD behavior are self-validating.
- Shared contract, shared library, or shared web package changes fan out to every dependent product or domain that could be affected.
- Documentation-only, specification-only, and agent-instruction-only changes do not waste build or deployment time but still produce the single required status.
- Multiple unrelated folder changes in one pull request combine their trigger groups rather than selecting only one path.
- Failed change detection fails the single required pipeline result and does not silently skip validation.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The system MUST replace the current multiple Skedular CI/CD workflow set with one consolidated CI/CD pipeline definition for the repository.
- **FR-002**: The consolidated pipeline MUST classify changed files into named trigger groups that represent the current Skedular product, domain, shared, documentation, contract, infrastructure, and pipeline-definition areas.
- **FR-003**: The consolidated pipeline MUST run a change detection stage for every pull request, main-branch push, and manual run so that one umbrella pipeline result is always produced.
- **FR-004**: The consolidated pipeline MUST run build and validation segments only when their trigger group or a required dependency trigger group is active.
- **FR-005**: The consolidated pipeline MUST preserve the validation coverage of the existing Skedular workflows, including app builds, shared package validation, domain/shared infrastructure validation, and documentation or catalog build validation where currently present.
- **FR-006**: The consolidated pipeline MUST preserve deployment coverage for the existing staging and production deployable surfaces while gating those deployment segments behind the same relevant trigger groups and successful prerequisite validation.
- **FR-006a**: The consolidated pipeline MUST validate pull requests without deploying to staging or production.
- **FR-006b**: The consolidated pipeline MUST allow pushes to `main` to deploy affected staging surfaces after successful validation.
- **FR-006c**: The consolidated pipeline MUST allow production deployment only through the existing production release gates, environment protections, and approval conditions.
- **FR-007**: The consolidated pipeline MUST treat shared dependencies as transitive triggers, so changes to shared backend, shared web, shared infrastructure, contracts, generation inputs, or pipeline definitions run all dependent validation segments that may be affected.
- **FR-008**: The consolidated pipeline MUST treat documentation-only and specification-only changes as valid pipeline runs that skip unrelated builds and deployments while still reporting the required umbrella result.
- **FR-009**: The consolidated pipeline MUST support manual runs that intentionally execute the full consolidated validation path.
- **FR-010**: The consolidated pipeline MUST NOT introduce package versioning, tag-based version resolution, package publishing semantics, or release version calculation as part of this feature.
- **FR-011**: The consolidated pipeline MUST make each segment's run or skip decision visible in the run output or summary, including the changed-path groups that caused the decision.
- **FR-012**: The consolidated pipeline MUST fail the overall required result when any selected validation or deployment segment fails.
- **FR-013**: The consolidated pipeline MUST avoid requiring maintainers to update multiple workflow files when adding or changing a product/domain trigger group after this consolidation.
- **FR-014**: The consolidated pipeline MUST use the existing DSST single-pipeline pattern as the CI behavioral reference for change detection, conditional fan-out, docs-only skipping, and umbrella status reporting, excluding DSST versioning behavior.
- **FR-015**: The consolidated pipeline MUST add Skedular CD behavior after successful selected CI checks, because the DSST reference currently covers CI only and does not provide CD behavior to copy.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Feature MUST emit structured logs for start/completion of core workflows.
- **LOG-002**: Feature MUST emit structured logs for meaningful state transitions and branch decisions.
- **LOG-003**: Feature MUST emit actionable warning/error logs for failure and recovery paths.
- **LOG-004**: Feature logs MUST include correlation context (for example request/workflow identifiers) and MUST avoid sensitive data leakage.
- **LOG-005**: Pipeline run output MUST include a change detection summary with changed file count, active trigger groups, skipped trigger groups, docs-only decisions, manual-run decisions, and dependency fan-out decisions.

### Key Entities

- **Change Set**: The files changed in a pull request, push, or manual run context; used to decide which pipeline segments are relevant.
- **Trigger Group**: A named repository area or dependency category, such as a product app, domain shared area, shared dependency area, documentation area, contract area, or pipeline-definition area.
- **Pipeline Segment**: A build, validation, infrastructure, or deployment unit that can run or skip based on trigger group decisions.
- **Dependency Fan-Out Rule**: A rule that maps a shared or cross-cutting trigger group to all affected downstream pipeline segments.
- **Umbrella Pipeline Result**: The single required CI/CD status reported for the repository change, regardless of how many individual segments run or skip.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 100% of current Skedular CI/CD workflow responsibilities are represented in the consolidated pipeline inventory before the old workflow set is retired.
- **SC-002**: Pull requests that change only one isolated product or domain run no unrelated product/domain build segments in at least 90% of representative test cases.
- **SC-003**: Documentation-only or specification-only pull requests complete the required pipeline result without running build or deployment validation segments in 100% of representative test cases.
- **SC-004**: A shared dependency change triggers every documented dependent validation segment in 100% of representative test cases.
- **SC-005**: Manual pipeline runs execute the full consolidated validation path in 100% of manual-run test cases.
- **SC-006**: Maintainers can identify why each segment ran or skipped from the pipeline output within 2 minutes for representative pipeline runs.
- **SC-007**: The number of active Skedular CI/CD workflow definitions is reduced to one consolidated pipeline, excluding non-CI/CD setup or maintenance workflows if they are documented as out of scope.

## Assumptions

- The target repository for this feature is `unityhubio`; `dsst` is a reference workspace only.
- The DSST pipeline is used as a behavioral model for single-file CI orchestration, change detection, conditional jobs, docs-only skipping, manual full runs, and umbrella status checks.
- Skedular CD behavior is sourced from the existing Skedular workflows and must run after the relevant CI checks succeed.
- DSST package versioning and tag-driven publish behavior are intentionally out of scope for this feature.
- Existing Skedular build, validation, image, infrastructure, and deployment actions remain valid and should be reused where they already provide the required behavior.
- Existing staging and production environment protections, secrets, and approvals remain authoritative after consolidation.
- Non-CI/CD workflows such as development environment setup or package cleanup may remain separate when they are not part of the application CI/CD pipeline, provided this is documented during implementation.
