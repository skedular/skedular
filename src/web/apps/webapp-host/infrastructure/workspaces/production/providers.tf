provider "aws" {
  region = module.shared_common.aws_region
}

provider "vercel" {
  api_token = var.vercel_api_token
}

provider "cloudflare" {
  api_token = var.cloudflare_api_key
}
