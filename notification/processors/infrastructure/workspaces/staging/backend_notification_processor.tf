terraform {
  required_version = ">= 1.0.0"

  backend "s3" {
    region         = "us-east-1"
    bucket         = "unityhub-staging-notification-processor-terraform-state"
    key            = "terraform.tfstate"
    dynamodb_table = "unityhub-staging-notification-processor-terraform-state-lock"
    profile        = ""
    encrypt        = "true"
  }
}
