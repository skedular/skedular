module "shared_common" {
  source = "../../../../../../shared/infrastructure/workspaces/common"

  environment = local.environment
}

module "common_resources" {
  source = "../common_resources"

  providers = {
    aws        = aws
    random     = random
    vercel     = vercel
    cloudflare = cloudflare
  }

  environment                                = local.environment
  gcp_unityhub_web_credentials_client_id     = var.gcp_unityhub_web_credentials_client_id
  gcp_unityhub_web_credentials_client_secret = var.gcp_unityhub_web_credentials_client_secret
  slack_client_secret                        = var.slack_client_secret
  google_analytics_measurement_id            = "G-F9FYTVMKRC"
  google_tag_manager_container_id            = "GTM-5H8MKJPK"
}
