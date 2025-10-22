data "azurerm_subnet" "database-subnet" {
  name                 = "database-subnet"
  virtual_network_name = module.naming.virtual_network.name
  resource_group_name  = var.resource_group
}

data "azurerm_subnet" "aks-private-subnet" {
  name                 = "aks-private-subnet"
  virtual_network_name = module.naming.virtual_network.name
  resource_group_name  = var.resource_group
}

data "azurerm_subnet" "bastion-subnet" {
  name                 = "AzureBastionSubnet"
  virtual_network_name = module.naming.virtual_network.name
  resource_group_name  = var.resource_group
}

data "azurerm_resource_group" "rg" {
  name = var.resource_group
}

data "azurerm_key_vault" "vault" {
  name                = module.naming.key_vault.name
  resource_group_name = var.resource_group
}