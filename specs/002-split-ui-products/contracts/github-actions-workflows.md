# Contract: GitHub Actions Workflow Pipelines

**Date**: 2026-04-18  
**Related**: [data-model.md](../data-model.md), [terraform-structure.md](terraform-structure.md)

## Overview

This contract defines the GitHub Actions workflows for continuous integration and deployment (CI/CD) of the three web applications. All workflows follow existing patterns from the current webapp.

---

## Workflow Files Structure

```text
.github/workflows/
├── lint-validate-infrastructure.yml     # Terraform linting & validation
├── build-deploy.yml                      # Application build & deployment
├── health-check.yml                      # Health project monitoring
└── README.md                             # Workflow documentation
```

---

## Workflow 1: Lint & Validate Infrastructure

**File**: `.github/workflows/lint-validate-infrastructure.yml`

**Purpose**: Validate Terraform configuration syntax and consistency

**Triggers**:

- `push` to `main` branch (or feature branches with infrastructure changes)
- `pull_request` (any branch)
- Manual trigger: `workflow_dispatch`

**Jobs**:

1. `terraform-fmt`: Check Terraform code formatting
2. `terraform-validate`: Validate all workspaces (staging, common_resources, production)
3. `tfsec`: Static security analysis of Terraform code (if configured)

**Example Configuration**:

```yaml
name: Lint & Validate Infrastructure

on:
  push:
    branches:
      - main
    paths:
      - "infrastructure/**"
      - ".github/workflows/lint-validate-infrastructure.yml"
  pull_request:
    paths:
      - "infrastructure/**"
  workflow_dispatch:

jobs:
  terraform-fmt:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: hashicorp/setup-terraform@v2
        with:
          terraform_version: 1.6.0
      - name: Terraform Format Check
        run: |
          terraform fmt -check -recursive infrastructure/

  terraform-validate:
    runs-on: ubuntu-latest
    strategy:
      matrix:
        environment: [staging, common_resources, production]
    steps:
      - uses: actions/checkout@v4
      - uses: hashicorp/setup-terraform@v2
        with:
          terraform_version: 1.6.0
      - name: Terraform Init (no backend)
        run: |
          cd infrastructure/workspaces/${{ matrix.environment }}
          terraform init -backend=false -upgrade
      - name: Terraform Validate
        run: |
          cd infrastructure/workspaces/${{ matrix.environment }}
          terraform validate
```

**Success Criteria**:

- Terraform formatting passes (no `fmt` changes needed)
- All three workspaces validate without errors
- No security issues flagged by tfsec

---

## Workflow 2: Build & Deploy Application

**File**: `.github/workflows/build-deploy.yml`

**Purpose**: Build application, run tests, and deploy to Vercel

**Triggers**:

- `push` to `main` branch
- `pull_request` (creates preview deployment)
- Manual trigger: `workflow_dispatch` with environment selection

**Jobs**:

1. `build`: Build application (npm/pnpm build)
2. `test`: Run unit and integration tests
3. `lint`: Lint JavaScript/TypeScript code
4. `deploy-staging`: Deploy to Vercel staging (auto-triggered on main)
5. `deploy-production`: Deploy to Vercel production (manual approval required)

**Example Configuration**:

```yaml
name: Build & Deploy

on:
  push:
    branches:
      - main
    paths:
      - "src/**"
      - "package.json"
      - ".github/workflows/build-deploy.yml"
  pull_request:
  workflow_dispatch:
    inputs:
      deploy_to:
        description: "Deploy to environment"
        required: true
        default: "staging"
        type: choice
        options:
          - staging
          - production

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: "18"
          cache: "npm"
      - name: Install dependencies
        run: npm ci
      - name: Build application
        run: npm run build
      - name: Upload build artifacts
        uses: actions/upload-artifact@v3
        with:
          name: build-output
          path: .next/
          retention-days: 1

  lint:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: "18"
          cache: "npm"
      - name: Install dependencies
        run: npm ci
      - name: Run linter
        run: npm run lint

  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: "18"
          cache: "npm"
      - name: Install dependencies
        run: npm ci
      - name: Run tests
        run: npm run test

  deploy-staging:
    needs: [build, lint, test]
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Deploy to Vercel Staging
        uses: vercel/action@main
        with:
          vercel-token: ${{ secrets.VERCEL_TOKEN }}
          vercel-org-id: ${{ secrets.VERCEL_ORG_ID }}
          vercel-project-id: ${{ secrets.VERCEL_PROJECT_ID_STAGING }}
          scope: ${{ secrets.VERCEL_ORG_ID }}

  deploy-production:
    needs: [build, lint, test]
    if: github.ref == 'refs/heads/main' && github.event_name == 'workflow_dispatch'
    runs-on: ubuntu-latest
    environment:
      name: production
      url: https://${{ secrets.PRODUCTION_DOMAIN }}
    steps:
      - uses: actions/checkout@v4
      - name: Deploy to Vercel Production
        uses: vercel/action@main
        with:
          vercel-token: ${{ secrets.VERCEL_TOKEN }}
          vercel-org-id: ${{ secrets.VERCEL_ORG_ID }}
          vercel-project-id: ${{ secrets.VERCEL_PROJECT_ID_PRODUCTION }}
          scope: ${{ secrets.VERCEL_ORG_ID }}
```

**Environment Variables Required**:

- `VERCEL_TOKEN`: Vercel API token
- `VERCEL_ORG_ID`: Vercel organization ID
- `VERCEL_PROJECT_ID_STAGING`: Project ID for staging environment
- `VERCEL_PROJECT_ID_PRODUCTION`: Project ID for production environment
- `PRODUCTION_DOMAIN`: Domain for production environment

**Success Criteria**:

- Build completes without errors
- All tests pass
- Linting passes
- Deployment to Vercel succeeds
- Application responds to health check endpoint

---

## Workflow 3: Health Project Monitoring

**File**: `.github/workflows/health-check.yml`

**Purpose**: Monitor health and uptime of deployed applications

**Triggers**:

- Scheduled: Every 5 minutes (or configurable interval)
- Manual trigger: `workflow_dispatch`

**Jobs**:

1. `health-check`: Ping health endpoint of main app
2. `design-system-check`: Verify design system components render
3. `alert-on-failure`: Send alert if health check fails

**Example Configuration**:

```yaml
name: Health Check

on:
  schedule:
    - cron: "*/5 * * * *" # Every 5 minutes
  workflow_dispatch:

jobs:
  health-check:
    runs-on: ubuntu-latest
    strategy:
      matrix:
        environment: [staging, production]
    steps:
      - name: Check application health
        run: |
          RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" https://${{ env[format('{0}_DOMAIN', matrix.environment)] }}/health)
          if [ "$RESPONSE" != "200" ]; then
            echo "Health check failed: HTTP $RESPONSE"
            exit 1
          fi
          echo "Health check passed"
        env:
          staging_DOMAIN: ${{ secrets.STAGING_DOMAIN }}
          production_DOMAIN: ${{ secrets.PRODUCTION_DOMAIN }}

  alert-on-failure:
    if: failure()
    runs-on: ubuntu-latest
    steps:
      - name: Send Slack notification
        uses: slackapi/slack-github-action@v1.24.0
        with:
          payload: |
            {
              "text": "Health check failed for ${{ github.repository }}"
            }
        env:
          SLACK_WEBHOOK_URL: ${{ secrets.SLACK_WEBHOOK_URL }}
```

**Success Criteria**:

- Health endpoint returns HTTP 200
- Response time < 2 seconds
- All health checks pass

---

## Workflow Configuration for Each Product

Each product (`webapp`, `webapp-teams`, `webapp-spaces`) has its own workflow configuration with environment-specific secrets.

### Secrets Template (GitHub Repository Settings)

For each product, configure secrets:

```
VERCEL_TOKEN                  # Shared across all products
VERCEL_ORG_ID                 # Shared across all products
VERCEL_PROJECT_ID_STAGING     # Product-specific
VERCEL_PROJECT_ID_PRODUCTION  # Product-specific
PRODUCTION_DOMAIN             # Product-specific (e.g., private.skedular.io, marketplace.skedular.io)
STAGING_DOMAIN                # Product-specific
SLACK_WEBHOOK_URL             # Shared or product-specific
```

---

## Workflow Execution & Parallelization

**Concurrent Workflow Runs**:

- All three products can run workflows in parallel (no cross-product dependencies)
- Within a workflow, jobs can run in parallel (e.g., lint, test, build simultaneously)
- Deploy jobs run sequentially (staging first, production requires approval)

**Execution Order** (within a single product workflow):

1. **Parallel**: lint, test, build, terraform-validate
2. **After 1 succeeds**: deploy-staging
3. **Manual trigger required**: deploy-production

---

## Logging & Observability

All workflows MUST log:

- Workflow trigger and initiator
- Build/deployment stages (START, IN_PROGRESS, SUCCESS/FAILURE)
- Deployment targets (staging URL, production URL)
- Any errors or warnings
- Deployment duration (total time)

**Log Format**:

```
[TIMESTAMP] [WORKFLOW] [STAGE] [STATUS] [MESSAGE]
Example:
2026-04-18T10:30:45Z [build-deploy] [build] [IN_PROGRESS] Building application...
2026-04-18T10:32:10Z [build-deploy] [build] [SUCCESS] Build completed in 85 seconds
2026-04-18T10:32:15Z [build-deploy] [deploy-staging] [IN_PROGRESS] Deploying to Vercel staging...
2026-04-18T10:33:20Z [build-deploy] [deploy-staging] [SUCCESS] Deployment succeeded
```

---

## Contract Compliance

All three web applications MUST:

- [ ] Have lint-validate-infrastructure.yml workflow
- [ ] Have build-deploy.yml workflow
- [ ] Have health-check.yml workflow (or centralized version)
- [ ] All workflows trigger on push to main branch
- [ ] All workflows have manual trigger capability (workflow_dispatch)
- [ ] All workflows validate all three Terraform workspaces
- [ ] All workflows pass linting, testing, and validation before deployment
- [ ] Staging deployments are automatic on main push
- [ ] Production deployments require manual approval
- [ ] Health checks run on 5-minute schedule (or equivalent)
- [ ] Failures generate alerts (Slack or equivalent)
