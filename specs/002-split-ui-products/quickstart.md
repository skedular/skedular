# Quick Start: Scaffolding a New Web Application

**Date**: 2026-04-18  
**Related**: [plan.md](plan.md), [data-model.md](data-model.md), [contracts/](contracts/)

## Overview

This guide provides step-by-step instructions for scaffolding a new web application project (`webapp-teams` or `webapp-spaces`) from the current webapp template.

## Implementation Notes

- Execute Phase 1 and Phase 2 tasks from `tasks.md` before story-specific scaffolding.
- Use `scripts/verify-ui-package-versions.sh webapp` to validate shared UI baseline before cloning.
- Use `scripts/validate-three-products.sh webapp` and `scripts/validate-workspace-layout.sh webapp` to capture baseline parity.
- Record baseline results in `specs/002-split-ui-products/docs/foundation-baseline.md` before starting US1/US2.

---

## Prerequisites

Before starting, ensure you have:

- [ ] Repository cloned locally
- [ ] Node.js 18+ installed
- [ ] Terraform 1.6+ installed
- [ ] AWS CLI v2 installed (with credentials configured)
- [ ] Vercel CLI installed (optional, for local testing)
- [ ] Git branch created for the new project (e.g., `002-split-ui-products`)

**Verify Tools**:

```bash
node --version          # Should be >= 18.0.0
terraform --version    # Should be >= 1.6.0
aws --version          # Should be >= 2.0.0
```

---

## Step 1: Clone the Current Webapp Template

Start by copying the current webapp structure to create the new project.

### 1.1 Create New Project Directories

```bash
# From repository root
mkdir -p web/apps/webapp-teams
mkdir -p web/apps/webapp-teams-help

# OR for marketplace
mkdir -p web/apps/webapp-spaces
mkdir -p web/apps/webapp-spaces-help
```

### 1.2 Copy Project Structure

```bash
# Copy webapp structure to new project
cp -r web/apps/webapp/* web/apps/webapp-teams/

# Copy webapphelp structure to health project
cp -r web/apps/webapphelp/* web/apps/webapp-teams-help/

# For marketplace, repeat with webapp-spaces directory
cp -r web/apps/webapp/* web/apps/webapp-spaces/
cp -r web/apps/webapphelp/* web/apps/webapp-spaces-help/
```

### 1.3 Remove Git History (Optional)

If you want to start fresh without copying git history:

```bash
# Remove .git directory from new projects
rm -rf web/apps/webapp-teams/.git
rm -rf web/apps/webapp-teams-help/.git

# Remove any build artifacts
rm -rf web/apps/webapp-teams/.next
rm -rf web/apps/webapp-teams/node_modules
rm -rf web/apps/webapp-teams-help/node_modules
```

---

## Step 2: Update Project Configuration Files

Update configuration files to reflect the new project identity.

### 2.1 Update package.json

For `web/apps/webapp-teams/package.json`:

**Changes**:

- `name`: "@skedular/webapp-teams"
- `description`: "Skedular Teams Web Application"
- Ensure design system version matches other products

```bash
# Edit web/apps/webapp-teams/package.json
# Update the "name" and "description" fields
```

**Example diff**:

```json
{
  "name": "@skedular/webapp-teams",  # WAS: @skedular/webapp
  "description": "Skedular Teams Web Application",  # WAS: Skedular Web Application
  "version": "1.0.0",
  "private": true,
  ...
}
```

### 2.2 Update Environment Files

For `web/apps/webapp-teams/.env.example`:

**Changes**:

- `NEXT_PUBLIC_APP_NAME`: "webapp-teams"
- `NEXT_PUBLIC_API_URL`: Update if environment-specific

```bash
# Edit web/apps/webapp-teams/.env.example
# Update APP_NAME to "webapp-teams"
# Verify NEXT_PUBLIC_APP_URL is correct
```

### 2.3 Update .github/workflows

**Path**: `web/apps/webapp-teams/.github/workflows/`

**Changes**:

- Update job names to reflect project identity
- Update Vercel project IDs to point to new Vercel projects
- Keep the same trigger conditions and step logic

```bash
# Edit web/apps/webapp-teams/.github/workflows/build-deploy.yml
# Update VERCEL_PROJECT_ID_STAGING and VERCEL_PROJECT_ID_PRODUCTION
# (These will be set up in the "Configure Vercel" step)
```

---

## Step 3: Verify Directory Structure

Confirm the new project has the correct structure matching the contract.

### 3.1 Verify Directories Exist

```bash
# For webapp-teams
cd web/apps/webapp-teams
find . -type d -maxdepth 2 | sort

# Should show:
# .
# ./infrastructure
# ./infrastructure/modules
# ./infrastructure/workspaces
# ./infrastructure/workspaces/staging
# ./infrastructure/workspaces/common_resources
# ./infrastructure/workspaces/production
# ./src
# ./src/pages
# ./src/components
# ./public
# ./docs
# ./.github
# ./.github/workflows
```

### 3.2 Verify Key Files Exist

```bash
# For webapp-teams
ls -la web/apps/webapp-teams/package.json
ls -la web/apps/webapp-teams/tsconfig.json
ls -la web/apps/webapp-teams/next.config.js
ls -la web/apps/webapp-teams/.env.example
ls -la web/apps/webapp-teams/.github/workflows/lint-validate-infrastructure.yml
ls -la web/apps/webapp-teams/.github/workflows/build-deploy.yml
```

---

## Step 4: Set Up Local Development Environment

Prepare the local development environment for the new project.

### 4.1 Install Dependencies

```bash
cd web/apps/webapp-teams
npm ci  # or pnpm install
```

**Expected output**: `up to date` after successful install, design system package installed.

### 4.2 Verify Design System Integration

```bash
# Check package.json for design system dependency
grep -A 2 "@skedular/design-system" package.json

# Should output:
# "@skedular/design-system": "1.0.0",
```

### 4.3 Create Local Environment File

```bash
cp .env.example .env.local

# Edit .env.local and set actual values (replace placeholders)
# Example:
# NEXT_PUBLIC_API_URL=https://api-staging.skedular.io
# BACKEND_API_TOKEN=your-actual-token-here
```

---

## Step 5: Verify Local Development

Test that the application builds and runs locally.

### 5.1 Run Build

```bash
cd web/apps/webapp-teams
npm run build
```

**Expected output**:

```
  ▲ Next.js 14.0.0
  Creating an optimized production build ...
  ✓ Compiled successfully
  ✓ Linting and type checking ...
  ✓ Collecting page data ...
  ✓ Generating static pages (2/2)
  ✓ Collecting build traces ...
  Route (pages) Size
  ○ /_app                   0 B
  ○ / (ISR)                 5 kB
```

If build fails:

- Check Node.js version: `node --version`
- Clear cache: `rm -rf .next && npm ci`
- Check for TypeScript errors: `npm run lint`

### 5.2 Run Linting

```bash
npm run lint
```

**Expected output**: No errors, 0 warnings.

If linting fails:

- Check ESLint configuration: `cat eslint.config.js`
- Auto-fix issues: `npx eslint src --fix`

### 5.3 Run Tests (if configured)

```bash
npm run test
```

**Expected output**: All tests pass (or "No tests found" if none configured).

### 5.4 Run Development Server

```bash
npm run dev
```

**Expected output**:

```
  ▲ Next.js 14.0.0
  ▶ Local:        http://localhost:3000
  ▶ Environments: .env.local
```

**Verify application**:

- Open http://localhost:3000 in browser
- Page should load without errors
- Design system components should render
- Console should show no critical errors (check browser dev tools)

Press `CTRL+C` to stop the dev server.

---

## Step 6: Verify Terraform Infrastructure

Test the Terraform configuration for each workspace.

### 6.1 Initialize Terraform (No Backend)

```bash
cd infrastructure/workspaces/staging
terraform init -backend=false -upgrade
```

**Expected output**:

```
Terraform has been successfully configured!
You may now begin working with Terraform.
```

### 6.2 Validate Terraform Configuration

```bash
cd infrastructure/workspaces/staging
terraform validate
```

**Expected output**:

```
Success! The configuration is valid.
```

### 6.3 Repeat for Other Workspaces

```bash
# Test common_resources workspace
cd ../common_resources
terraform init -backend=false -upgrade
terraform validate

# Test production workspace
cd ../production
terraform init -backend=false -upgrade
terraform validate
```

**All three workspaces should validate successfully.**

---

## Step 7: Update GitHub Actions (Optional for Local Testing)

Configure GitHub Actions workflows to work with the new project.

### 7.1 Update Workflow Secrets

In GitHub repository settings, add/update secrets for the new project:

```
VERCEL_TOKEN                          # Shared, already exists
VERCEL_ORG_ID                         # Shared, already exists
VERCEL_PROJECT_ID_STAGING             # NEW: webapp-teams staging project ID
VERCEL_PROJECT_ID_PRODUCTION          # NEW: webapp-teams production project ID
PRODUCTION_DOMAIN                     # NEW: webapp-teams production domain
STAGING_DOMAIN                        # NEW: webapp-teams staging domain
```

### 7.2 Test Workflow Locally (Optional)

If using `act` tool to test GitHub Actions locally:

```bash
cd web/apps/webapp-teams
act -j terraform-validate
```

---

## Step 8: Verify Design System Integration

Ensure the shared design system is correctly integrated.

### 8.1 Check Design System Import

```bash
cd web/apps/webapp-teams

# Check that _app.tsx imports design system
grep -n "@skedular/design-system" src/pages/_app.tsx

# Should show:
# src/pages/_app.tsx:import { ThemeProvider } from '@skedular/design-system';
# src/pages/_app.tsx:import '@skedular/design-system/styles/globals.css';
```

### 8.2 Verify Design System Version Match

```bash
# Check version in this project
grep "@skedular/design-system" package.json

# Check version in webapp
grep "@skedular/design-system" ../webapp/package.json

# Versions should match exactly (not range like ~1.0.0)
```

### 8.3 Test Component Rendering

```bash
# Check that a design system component is used
grep -r "Typography\|Button\|Card" src/components/

# Should show usage of design system components
```

---

## Step 9: Create Initial Commit

Commit the scaffolded project to git.

```bash
# From repository root
git add web/apps/webapp-teams/
git add web/apps/webapp-teams-help/

# Review changes
git status

# Commit
git commit -m "scaffold: add webapp-teams and webapp-teams-help projects

- Replicate webapp structure for private product variant
- Set up Terraform workspaces (staging, common_resources, production)
- Configure GitHub Actions workflows
- Integrate shared design system
- Both projects ready for feature extraction (Phase 2)"
```

---

## Verification Checklist

Before considering scaffolding complete, verify:

- [ ] New project directories created with correct names
- [ ] Directory structure matches contract (infrastructure/, src/, etc.)
- [ ] package.json updated with new project name and shared design system version
- [ ] npm install completes successfully
- [ ] `npm run build` succeeds
- [ ] `npm run lint` passes with 0 errors
- [ ] `npm run test` passes (or no tests found)
- [ ] `npm run dev` starts dev server
- [ ] Application loads at http://localhost:3000
- [ ] Design system components render without errors
- [ ] All three Terraform workspaces validate successfully
- [ ] GitHub Actions workflows are configured (secrets set)
- [ ] Changes committed to git with meaningful commit message

---

## Troubleshooting

### Build Fails with TypeScript Errors

```bash
# Clear cache and reinstall
rm -rf node_modules package-lock.json
npm ci
npm run build
```

### Design System Not Found

```bash
# Verify design system is installed
npm list @skedular/design-system

# If not installed:
npm install @skedular/design-system@1.0.0
```

### Terraform Init Fails

```bash
# Make sure you're in the correct workspace directory
cd infrastructure/workspaces/staging

# Try initializing without backend first (for validation)
terraform init -backend=false

# If backend config is wrong, check terraform.tf backend block matches pattern
cat terraform.tf | grep -A 5 "backend"
```

### Development Server Won't Start

```bash
# Check if port 3000 is in use
lsof -i :3000

# If in use, kill the process or use different port
npm run dev -- -p 3001

# Or clear Next.js cache
rm -rf .next
npm run dev
```

---

## Next Steps

After successful scaffolding:

1. **Phase 2 Tasks**: Generate task breakdown using `/speckit.tasks`
2. **Feature Extraction**: Plan extraction of private/marketplace functionality from main webapp
3. **Testing**: Set up health checks and monitoring for new products
4. **Documentation**: Update team documentation with new project information
5. **Deployment**: Deploy new projects to staging and production environments

---

## Reference

- [Terraform Structure Contract](contracts/terraform-structure.md)
- [GitHub Actions Workflows Contract](contracts/github-actions-workflows.md)
- [Web Application Structure Contract](contracts/webapp-structure.md)
- [Data Model](data-model.md)
- [Implementation Plan](plan.md)

---

## Validation Record (2026-04-18)

Executed against the current scaffold state to satisfy implementation validation:

### Tool Versions

- `node --version` -> `v22.19.0`
- `terraform --version` -> `Terraform v1.14.8`
- `aws --version` -> `aws-cli/2.30.5 ...`

### Build Validation

- `pnpm --filter webapp-teams build` -> pass (`real 3.21s`)
- `pnpm --filter webapp-spaces build` -> pass (`real 2.74s`)

### Terraform Validation

All commands run with `terraform init -backend=false -input=false` then `terraform validate`.

- `webapp-teams/staging` -> pass (`real 2.35s`)
- `webapp-teams/common_resources` -> pass (`real 2.19s`)
- `webapp-teams/production` -> pass (`real 2.19s`)
- `webapp-spaces/staging` -> pass (`real 2.14s`)
- `webapp-spaces/common_resources` -> pass (`real 2.14s`)
- `webapp-spaces/production` -> pass (`real 2.13s`)

### Structure Checks

Verified both `webapp-teams` and `webapp-spaces` include:

- `infrastructure/workspaces/staging`
- `infrastructure/workspaces/common_resources`
- `infrastructure/workspaces/production`
- `package.json`
- `tsconfig.json`
- `next.config.ts`
