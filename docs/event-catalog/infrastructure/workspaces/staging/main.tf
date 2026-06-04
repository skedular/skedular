module "shared_common" {
  source = "../../../../../src/shared/infrastructure/workspaces/common"

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

  environment = local.environment
}
