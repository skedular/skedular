module "common" {
  source = "../common"

  environment = local.environment
}

module "common_resources" {
  source = "../common_resources"
  providers = {
    aws        = aws
    google     = google
    cloudflare = cloudflare
    stripe     = stripe
  }

  environment                                = local.environment
  gcp_unityhub_web_credentials_client_id     = var.gcp_unityhub_web_credentials_client_id
  gcp_unityhub_web_credentials_client_secret = var.gcp_unityhub_web_credentials_client_secret
  log_retention                              = local.log_retention
}
