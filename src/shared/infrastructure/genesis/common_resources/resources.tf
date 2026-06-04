module "common" {
  source = "../common"
}

module "gcp_oneoff" {
  source = "../modules/gcp-oneoff"

  providers = {
    aws = aws
  }

  organization_name = module.common.organization_name
  environment       = var.environment
}

module "aws_oneoff" {
  source = "../modules/aws-oneoff"

  providers = {
    aws = aws
  }

  organization_name = module.common.organization_name
  environment       = var.environment
}

module "azure_oneoff" {
  source = "../modules/azure-oneoff"

  providers = {
    aws = aws
  }

  organization_name = module.common.organization_name
  environment       = var.environment
}

module "shared" {
  source = "../modules/shared"

  providers = {
    aws = aws
  }

  organization_name = module.common.organization_name
  environment       = var.environment
}

module "publicweb" {
  source = "../modules/public-web"

  providers = {
    aws = aws
  }

  organization_name = module.common.organization_name
  environment       = var.environment
}

module "webapp" {
  source = "../modules/webapp"

  providers = {
    aws = aws
  }

  organization_name = module.common.organization_name
  environment       = var.environment
}

module "webapp_spaces" {
  source = "../modules/webapp-spaces"

  providers = {
    aws = aws
  }

  organization_name = module.common.organization_name
  environment       = var.environment
}

module "webapp_teams" {
  source = "../modules/webapp-teams"

  providers = {
    aws = aws
  }

  organization_name = module.common.organization_name
  environment       = var.environment
}

module "customer" {
  source = "../modules/customer"

  providers = {
    aws = aws
  }

  organization_name = module.common.organization_name
  environment       = var.environment
}

module "organization" {
  source = "../modules/organization"

  providers = {
    aws = aws
  }

  organization_name = module.common.organization_name
  environment       = var.environment
}

module "team" {
  source = "../modules/team"

  providers = {
    aws = aws
  }

  organization_name = module.common.organization_name
  environment       = var.environment
}

module "location" {
  source = "../modules/location"

  providers = {
    aws = aws
  }

  organization_name = module.common.organization_name
  environment       = var.environment
}

module "slack" {
  source = "../modules/slack"

  providers = {
    aws = aws
  }

  organization_name = module.common.organization_name
  environment       = var.environment
}

module "msteams" {
  source = "../modules/msteams"

  providers = {
    aws = aws
  }

  organization_name = module.common.organization_name
  environment       = var.environment
}

module "marketplace" {
  source = "../modules/marketplace"

  providers = {
    aws = aws
  }

  organization_name = module.common.organization_name
  environment       = var.environment
}

module "booking" {
  source = "../modules/booking"

  providers = {
    aws = aws
  }

  organization_name = module.common.organization_name
  environment       = var.environment
}

module "core" {
  source = "../modules/core"

  providers = {
    aws = aws
  }

  organization_name = module.common.organization_name
  environment       = var.environment
}
