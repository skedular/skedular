module "shared_common" {
  source = "../../../../../../shared/infrastructure/workspaces/common"

  environment = local.environment
}

module "common_resources" {
  source = "../common_resources"

  providers = {
    cloudflare = cloudflare
  }

  environment                     = local.environment
  google_analytics_measurement_id = local.google_analytics_measurement_id
  logrocket_app_id                = local.logrocket_app_id
}
