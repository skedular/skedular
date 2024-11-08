module "shared_common" {
  source = "../../../infrastructure/workspaces/common"

  environment = local.environment
}

module "common_resources" {
  source = "../common_resources"
  providers = {
    aws = aws
  }

  environment = local.environment
}
