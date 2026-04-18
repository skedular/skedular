# Contract: Terraform Infrastructure Structure

**Date**: 2026-04-18  
**Related**: [data-model.md](../data-model.md), [plan.md](../plan.md)

## Overview

This contract defines the Terraform module organization, workspace structure, backend configuration, and state isolation strategy for all three web applications (`webapp`, `webapp-teams`, `webapp-spaces`).

---

## Module Organization

### Standard Module Structure

```text
infrastructure/
├── modules/
│   ├── app/
│   │   ├── main.tf          # Main application infrastructure
│   │   ├── variables.tf     # Input variables
│   │   ├── outputs.tf       # Output values
│   │   └── locals.tf        # Local values
│   ├── database/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   └── outputs.tf
│   ├── networking/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   └── outputs.tf
│   ├── monitoring/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   └── outputs.tf
│   └── [other domain-specific modules]
├── workspaces/
│   ├── staging/
│   │   ├── terraform.tf     # Backend and provider config
│   │   ├── main.tf          # Workspace-specific resource definitions
│   │   ├── variables.tf     # Workspace-scoped variables
│   │   ├── terraform.tfvars # Environment-specific values
│   │   └── outputs.tf       # Workspace outputs
│   ├── common_resources/
│   │   ├── terraform.tf
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   ├── terraform.tfvars
│   │   └── outputs.tf
│   └── production/
│       ├── terraform.tf
│       ├── main.tf
│       ├── variables.tf
│       ├── terraform.tfvars
│       └── outputs.tf
├── versions.tf              # Terraform version and provider requirements
└── README.md                # Infrastructure documentation
```

---

## Provider Configuration

### Required Providers (All Three Webapps)

```hcl
terraform {
  required_version = ">= 1.6.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 6.0"
    }
    vercel = {
      source  = "vercel/vercel"
      version = "~> 4.0"
    }
    google = {
      source  = "hashicorp/google"
      version = "~> 5.0"
    }
  }
}

provider "aws" {
  region = var.aws_region

  default_tags {
    tags = {
      Environment = var.environment
      Project     = var.project_id
      ManagedBy   = "Terraform"
    }
  }
}

provider "vercel" {
  api_token = var.vercel_api_token
}
```

---

## Backend Configuration

### S3 Backend Structure

**S3 Bucket**: `skedular-terraform-state`

**Key Prefix Pattern**: `{project_id}/{environment}/`

**Examples**:

- `webapp/staging/terraform.tfstate`
- `webapp/common_resources/terraform.tfstate`
- `webapp/production/terraform.tfstate`
- `webapp-teams/staging/terraform.tfstate`
- `webapp-teams/common_resources/terraform.tfstate`
- `webapp-teams/production/terraform.tfstate`
- `webapp-spaces/staging/terraform.tfstate`
- `webapp-spaces/common_resources/terraform.tfstate`
- `webapp-spaces/production/terraform.tfstate`

### Backend Configuration Block (in each workspace/terraform.tf)

```hcl
terraform {
  backend "s3" {
    bucket         = "skedular-terraform-state"
    key            = "{project_id}/{environment}/terraform.tfstate"
    region         = "us-east-1"
    encrypt        = true
    dynamodb_table = "terraform-locks"
  }
}
```

**Substitutions**:

- `{project_id}`: One of: `webapp`, `webapp-teams`, `webapp-spaces`
- `{environment}`: One of: `staging`, `common_resources`, `production`

### State Locking

- **DynamoDB Table**: `terraform-locks`
- **Lock Timeout**: 30 minutes (default)
- **Lock Table Schema**:
  ```hcl
  resource "aws_dynamodb_table" "terraform_locks" {
    name           = "terraform-locks"
    billing_mode   = "PAY_PER_REQUEST"
    hash_key       = "LockID"

    attribute {
      name = "LockID"
      type = "S"
    }
  }
  ```

---

## Workspace Variables

### Standard Variables (All Workspaces)

```hcl
variable "environment" {
  description = "Environment name (staging, common_resources, production)"
  type        = string
  validation {
    condition     = contains(["staging", "common_resources", "production"], var.environment)
    error_message = "Environment must be one of: staging, common_resources, production"
  }
}

variable "project_id" {
  description = "Project identifier (webapp, webapp-teams, webapp-spaces)"
  type        = string
  validation {
    condition     = contains(["webapp", "webapp-teams", "webapp-spaces"], var.project_id)
    error_message = "Project ID must be one of: webapp, webapp-teams, webapp-spaces"
  }
}

variable "aws_region" {
  description = "AWS region for resources"
  type        = string
  default     = "us-east-1"
}

variable "vercel_api_token" {
  description = "Vercel API token for deployments"
  type        = string
  sensitive   = true
}

variable "design_system_version" {
  description = "Shared design system version (must be same across all three apps)"
  type        = string
}

variable "app_domain" {
  description = "Domain for the web application"
  type        = string
}

variable "health_check_interval" {
  description = "Health check frequency in seconds"
  type        = number
  default     = 300
}
```

### Environment-Specific Values (terraform.tfvars)

**Example for webapp-teams/staging/terraform.tfvars**:

```hcl
environment              = "staging"
project_id              = "webapp-teams"
aws_region              = "us-east-1"
design_system_version   = "1.0.0"
app_domain              = "private-staging.skedular.io"
health_check_interval   = 300
```

---

## Output Values

### Standard Outputs (All Workspaces)

```hcl
output "app_url" {
  description = "URL of deployed application"
  value       = var.app_domain
}

output "terraform_state_bucket" {
  description = "S3 bucket containing Terraform state"
  value       = "skedular-terraform-state"
}

output "terraform_state_key" {
  description = "S3 key for this workspace's state file"
  value       = "${var.project_id}/${var.environment}/terraform.tfstate"
}

output "deployment_id" {
  description = "Unique deployment identifier"
  value       = "${var.project_id}-${var.environment}-${formatdate("YYYYMMDD-hhmm", timestamp())}"
}

output "health_check_url" {
  description = "URL of health check endpoint"
  value       = "https://${var.app_domain}/health"
}
```

---

## Validation Checklist

For each workspace, verify before `terraform init`:

- [ ] `terraform.tf` backend key follows pattern: `{project_id}/{environment}/terraform.tfstate`
- [ ] `{project_id}` is one of: `webapp`, `webapp-teams`, `webapp-spaces`
- [ ] `{environment}` is one of: `staging`, `common_resources`, `production`
- [ ] All required variables are defined in `variables.tf`
- [ ] Environment-specific values are set in `terraform.tfvars`
- [ ] Sensitive variables (API tokens, secrets) are NOT committed to git (use `.gitignore`)
- [ ] S3 bucket name matches the shared genesis bucket: `skedular-terraform-state`
- [ ] DynamoDB table name matches: `terraform-locks`
- [ ] All three workspaces exist (staging, common_resources, production)

---

## Commands

### Workspace Initialization

```bash
cd infrastructure/workspaces/staging
terraform init
terraform plan -out=tfplan
terraform apply tfplan
```

### Cross-Workspace Consistency Check

```bash
# Verify all three workspaces validate successfully
for env in staging common_resources production; do
  echo "Validating $env..."
  cd infrastructure/workspaces/$env
  terraform init -backend=false
  terraform validate || exit 1
  cd ../../..
done
echo "All workspaces validated successfully"
```

### State Inspection

```bash
# Show state for a workspace
terraform -chdir=infrastructure/workspaces/staging state list

# Refresh state
terraform -chdir=infrastructure/workspaces/staging refresh
```

---

## State Isolation Guarantees

1. **Separate State Files**: Each workspace has its own state file in S3 (separate key paths)
2. **State Locking**: DynamoDB ensures only one operation modifies state at a time
3. **Encryption**: S3 objects are encrypted at rest
4. **Versioning**: S3 versioning enabled to allow state rollback if needed
5. **Access Control**: IAM policies restrict state access to authorized roles only

---

## Disaster Recovery

### Backup Strategy

- S3 state bucket has versioning enabled
- Daily automated backups to separate S3 bucket
- 30-day retention policy

### Recovery Procedure

If state is corrupted:

1. Restore from backup: `aws s3 cp s3://terraform-backups/{project_id}/{environment}/* s3://skedular-terraform-state/{project_id}/{environment}/`
2. Refresh Terraform: `terraform refresh`
3. Validate: `terraform plan` should show minimal changes

---

## Contract Compliance

All three web applications (`webapp`, `webapp-teams`, `webapp-spaces`) MUST:

- [ ] Use the same module structure (modules/ + workspaces/)
- [ ] Have exactly three workspaces: staging, common_resources, production
- [ ] Use the shared S3 backend with workspace-scoped key paths
- [ ] Define all standard variables and outputs
- [ ] Pass `terraform validate` on all workspaces
- [ ] Use the same provider versions (AWS ~> 6.0, Vercel ~> 4.0, etc.)
- [ ] Store sensitive values in GitHub Actions secrets, NOT in git
- [ ] Document environment-specific configurations in terraform.tfvars comments
