data "azurerm_subnet" "aks-system-subnet" {
  name                 = "aks-system-subnet"
  virtual_network_name = module.naming.virtual_network.name
  resource_group_name  = var.resource_group
}

data "azurerm_subnet" "aks-public-subnet" {
  name                 = "aks-public-subnet"
  virtual_network_name = module.naming.virtual_network.name
  resource_group_name  = var.resource_group
}

data "azurerm_client_config" "current" {}

data "azuread_user" "kirill" {
  user_principal_name = "kirill@getskedular.com"
}

data "azuread_user" "morteza" {
  user_principal_name = "morteza@getskedular.com"
}

data "azurerm_key_vault" "vault" {
  name                = "skedular-${module.naming.key_vault.name}"
  resource_group_name = var.resource_group
}
