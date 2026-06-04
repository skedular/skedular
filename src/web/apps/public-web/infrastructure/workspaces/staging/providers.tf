provider "aws" {
  region = module.shared_common.aws_region
}

provider "cloudflare" {
  api_token = var.cloudflare_api_key
}
