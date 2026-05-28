data "azurerm_resource_group" "existing_rg" {
  name = module.naming.resource_group.name
}

data "azurerm_subnet" "aks_subnet" {
  name                 = "aks-system-subnet"
  virtual_network_name = module.naming.virtual_network.name
  resource_group_name  = module.naming.resource_group.name
}