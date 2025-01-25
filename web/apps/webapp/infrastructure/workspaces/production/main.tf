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

  environment                       = local.environment
  gcp_web_credentials_client_id     = var.gcp_web_credentials_client_id
  gcp_web_credentials_client_secret = var.gcp_web_credentials_client_secret
  google_analytics_measurement_id   = "G-3TYYHWY70E"
  google_tag_manager_container_id   = "GTM-TVB7D4HJ"
  workos_api_key                    = var.workos_api_key
}
