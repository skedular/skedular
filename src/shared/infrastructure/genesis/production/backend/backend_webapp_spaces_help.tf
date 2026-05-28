terraform {
  required_version = ">= 1.0.0"

  backend "s3" {
    region         = "us-east-1"
    bucket         = "unityhub-production-webappspaces-help-terraform-state"
    key            = "terraform.tfstate"
    dynamodb_table = "unityhub-production-webappspaces-help-terraform-state-lock"
    profile        = ""
    encrypt        = "true"
  }
}
