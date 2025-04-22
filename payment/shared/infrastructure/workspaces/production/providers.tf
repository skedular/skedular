provider "aws" {
  region = module.shared_common.aws_region
}

provider "stripe" {
  api_key = var.stripe_api_key
}
