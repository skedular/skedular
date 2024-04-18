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

  environment                     = local.environment
  google_analytics_measurement_id = "G-F9FYTVMKRC"
  google_tag_manager_container_id = "GTM-5H8MKJPK"
}
