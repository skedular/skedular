# Research: Merge CI/CD Pipelines

## Decision: Use DSST as the CI orchestration reference, then add Skedular CD after CI succeeds

**Rationale**: The DSST reference pipeline currently demonstrates the CI side: one workflow file, one first `detect` job, conditional validation jobs, docs-only skipping, manual full runs, and one stable required status. Skedular must use that CI pattern as the base and then add the CD side that DSST does not yet implement: staging deployment after selected CI jobs pass on `main`, and production deployment only through existing production gates.

**Alternatives considered**:

- Keep multiple workflow files and refine their path filters: rejected because it preserves duplicated trigger logic and does not produce one umbrella CI/CD result.
- Chain workflows through `workflow_run`: rejected because it adds latency and makes dependency ordering less explicit.
- Use only workflow-level `paths`: rejected because skipped workflows may not produce the desired required umbrella status.

## Decision: Model folder awareness through named trigger groups

**Rationale**: Existing Skedular workflows already imply trigger groups through their path filters: shared backend, domain shared folders, all-in-one/backend images, web apps, web packages, web infrastructure, shared infrastructure, docs event catalog, and pipeline files. Explicit trigger groups make those responsibilities auditable and reusable inside one pipeline.

**Alternatives considered**:

- Inline path checks in every job: rejected because it repeats logic and makes future product/domain additions error-prone.
- One broad monorepo trigger for every source change: rejected because the feature explicitly requires not building everything when not required.
- Only app-level trigger groups: rejected because shared packages, contracts, infrastructure, and pipeline changes need transitive fan-out.

## Decision: Preserve existing reusable action contracts

**Rationale**: Skedular already has reusable composite actions for Docker build/test/push and Terraform validate/apply. The consolidation should change orchestration, not rebuild the CI/CD mechanics. Reusing `.github/actions/build-test-push`, `.github/actions/lint-validate-infrastructure`, and `.github/actions/deploy-infrastructure` preserves existing behavior and minimizes blast radius.

**Alternatives considered**:

- Replace composite actions with inline workflow steps: rejected because it would increase the size of the consolidation and duplicate already-working behavior.
- Introduce a new third-party monorepo pipeline action: rejected because it adds a new dependency when DSST shows the existing shell-based detection pattern is sufficient.

## Decision: Exclude versioning and package release semantics

**Rationale**: The user explicitly requested that DSST versioning not be brought into Skedular for this feature. The consolidated Skedular pipeline may continue to use existing image tag behavior inside the build action, but it must not add DSST-style package version resolution, tag-driven package publishing, or release version calculation.

**Alternatives considered**:

- Port DSST's tag/version handling wholesale: rejected by user requirement.
- Add a generic versioning placeholder for future work: rejected because it would blur scope and affect tasks for this feature.

## Decision: Deployment behavior follows clarified Option A and is implemented as post-CI CD jobs

**Rationale**: The clarified behavior is: pull requests validate only; pushes to `main` may deploy affected staging surfaces after validation; production deploys only through existing production release gates, environment protections, and approval conditions. Because DSST only covers CI today, the Skedular consolidated workflow must explicitly add CD jobs that depend on the matching CI jobs and reuse existing deployment actions.

**Alternatives considered**:

- Deploy staging and production on every `main` push with only environment approvals: rejected because it can surprise release owners.
- Make all deployments manual-only: rejected because it removes existing deployment automation.
- Preserve every workflow's trigger semantics exactly without normalization: rejected because the single pipeline needs a clear, documented rule set.

## Decision: Treat documentation/spec-only changes as successful no-build runs

**Rationale**: The DSST reference includes a docs-only override that disables build trigger flags while still allowing the umbrella pipeline to report success. Skedular should keep that behavior for documentation, Spec Kit artifacts, and agent instruction changes.

**Alternatives considered**:

- Skip the entire workflow for documentation-only changes: rejected because no umbrella required status would be produced.
- Run lint/build for documentation-only changes: rejected because the feature goal is to avoid unnecessary work.

## Decision: Pipeline-definition and action changes trigger broad validation

**Rationale**: Changes to `.github/workflows/`, `.github/actions/`, and relevant prompt/agent CI support files can affect any downstream job. Broad fan-out self-validates pipeline edits and mirrors the DSST global trigger idea for pipeline files.

**Alternatives considered**:

- Validate only the changed workflow segment: rejected because shared CI/CD changes can break unrelated jobs.
- Treat pipeline edits as docs-only: rejected because they directly affect build/deploy behavior.

## Decision: Use workflow-run summaries as the primary audit surface

**Rationale**: GitHub Actions summaries and logs can show changed files, trigger groups, fan-out decisions, selected jobs, skipped jobs, and manual/docs-only decisions without introducing new storage or services. This satisfies observability requirements for CI/CD logic.

**Alternatives considered**:

- Store pipeline decision data in repository artifacts only: rejected because artifacts are less visible during review.
- Open PR comments for every decision: rejected because existing Terraform validation already comments and additional comments may create noise.
