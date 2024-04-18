module "shared_common" {
  source = "../../../../shared/infrastructure/workspaces/common"

  environment = local.environment
}

module "common_resources" {
  source = "../common_resources"

  providers = {
    aws    = aws
    random = random
  }

  environment = local.environment
}
