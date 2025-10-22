module "azure-redis" {
  count = local.is_staging ? 0 : 1

  source         = "../../modules/azure-redis"
  resource_group = data.azurerm_resource_group.existing_rg[0].name
  environment    = var.environment
  region         = var.azure_region
}
