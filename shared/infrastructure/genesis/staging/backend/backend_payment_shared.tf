terraform {
  required_version = ">= 1.0.0"

  backend "s3" {
    region         = "us-east-1"
    bucket         = "unityhub-staging-payment-shared-terraform-state"
    key            = "terraform.tfstate"
    dynamodb_table = "unityhub-staging-payment-shared-terraform-state-lock"
    profile        = ""
    role_arn       = ""
    encrypt        = "true"
  }
}
