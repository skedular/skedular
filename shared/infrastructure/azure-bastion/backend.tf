terraform {
  backend "s3" {
    region       = "us-east-1"
    bucket       = "unityhub-terraform-state"
    key          = "azure/infrastructure/azure-bastion-state-file.tfstate"
    profile      = "unityhub-operations"
    encrypt      = true
    use_lockfile = true
  }
}
