data "azurerm_resource_group" "rg" {
  name = var.resource_group
}

data "azurerm_subnet" "allowed" {
  for_each             = toset(var.allowed_subnet_names)
  name                 = each.key
  virtual_network_name = module.naming.virtual_network.name
  resource_group_name  = var.resource_group
}