module "azure-networking" {
  count = local.is_staging ? 0 : 1

  source      = "../../modules/azure-networking"
  environment = var.environment
  region      = var.azure_region
  tags        = local.tags
}
