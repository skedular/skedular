provider "aws" {
  region = module.common.aws_region
}

provider "google" {
  region  = module.common.gcp_region
  project = module.common.gcp_project_id
}