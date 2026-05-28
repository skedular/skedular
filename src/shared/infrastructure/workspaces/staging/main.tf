module "common" {
  source = "../common"

  environment = local.environment
}

module "common_resources" {
  source = "../common_resources"
  providers = {
    aws        = aws
    google     = google
    random     = random
    cloudflare = cloudflare
    stripe     = stripe
  }

  environment                       = local.environment
  gcp_web_credentials_client_id     = var.gcp_web_credentials_client_id
  gcp_web_credentials_client_secret = var.gcp_web_credentials_client_secret
  log_retention                     = local.log_retention
  azure_region                      = var.azure_region
}
