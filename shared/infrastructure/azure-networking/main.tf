module "azure-networking" {
  source      = "../modules/azure-networking"
  environment = var.environment
  region      = var.region
  tags        = local.tags

}
