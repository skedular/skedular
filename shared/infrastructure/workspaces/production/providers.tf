provider "aws" {
  region = module.common.aws_region
}

provider "google" {
  region  = module.common.gcp_region
  project = module.common.gcp_project_id
}

provider "cloudflare" {
  api_token = var.cloudflare_api_key
}

provider "stripe" {
  api_key = var.stripe_api_key
}

provider "azurerm" {
  features {}
  subscription_id = var.azure_subscription_id
}

