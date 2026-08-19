module "common" {
  source = "../../workspaces/common"

  environment = var.environment
}

resource "google_iam_workload_identity_pool" "github_action_pool" {
  project                   = module.common.gcp_project_id
  workload_identity_pool_id = "github-action-pool"
  display_name              = "GitHub Action Pool"
  description               = "Identity pool for GitHub action deployments"
}

resource "google_iam_workload_identity_pool_provider" "github" {
  project                            = module.common.gcp_project_id
  workload_identity_pool_id          = google_iam_workload_identity_pool.github_action_pool.workload_identity_pool_id
  workload_identity_pool_provider_id = "github-provider"
  attribute_mapping = {
    "google.subject"       = "assertion.sub"
    "attribute.actor"      = "assertion.actor"
    "attribute.aud"        = "assertion.aud"
    "attribute.repository" = "assertion.repository"
  }
  oidc {
    issuer_uri = "https://token.actions.githubusercontent.com"
  }
}

resource "google_service_account" "github_actions" {
  project      = module.common.gcp_project_id
  account_id   = "github-actions"
  display_name = "Service Account used for GitHub Actions"
}

resource "google_service_account_iam_member" "workload_identity_user" {
  service_account_id = google_service_account.github_actions.name
  role               = "roles/iam.workloadIdentityUser"
  member             = "principalSet://iam.googleapis.com/${google_iam_workload_identity_pool.github_action_pool.name}/attribute.repository/${module.common.github_repository_unityhubio}"
}

resource "aws_ssm_parameter" "workload_identity_provider" {
  name  = module.common.parameter_store_name_gcp_github_actions_workload_identity_provider
  type  = "String"
  value = "${google_iam_workload_identity_pool.github_action_pool.name}/providers/${google_iam_workload_identity_pool_provider.github.workload_identity_pool_provider_id}"
  tags  = local.tags
}

resource "aws_ssm_parameter" "service_account" {
  name  = module.common.parameter_store_name_gcp_github_actions_service_account
  type  = "String"
  value = google_service_account.github_actions.email
  tags  = local.tags
}
