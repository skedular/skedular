module "common" {
  source = "../common"
}

module "common_resources_staging" {
  source = "../common_resources"

  providers = {
    aws = aws
  }

  environment = local.environment
}
