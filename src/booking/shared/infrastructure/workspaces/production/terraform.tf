terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 6.0"
    }

    stripe = {
      source  = "lukasaron/stripe"
      version = "~> 3.0"
    }
  }
}
