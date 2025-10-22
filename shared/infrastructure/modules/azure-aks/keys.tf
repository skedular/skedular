resource "azurerm_key_vault_secret" "aks-admin-client-certificate" {
  name         = "${module.naming.key_vault_key.name}-aks-admin-client-certificate"
  key_vault_id = data.azurerm_key_vault.vault.id
  value        = module.aks_cluster.admin_client_certificate
}

resource "azurerm_key_vault_secret" "aks-admin-client-key" {
  name         = "${module.naming.key_vault_key.name}-aks-admin-client-key"
  key_vault_id = data.azurerm_key_vault.vault.id
  value        = module.aks_cluster.admin_client_key
}

resource "azurerm_key_vault_secret" "aks-admin-ca-certificate" {
  name         = "${module.naming.key_vault_key.name}-aks-admin-ca-certificate"
  key_vault_id = data.azurerm_key_vault.vault.id
  value        = module.aks_cluster.admin_client_certificate
}

resource "azurerm_key_vault_secret" "aks-cluster-admin-host" {
  name         = "${module.naming.key_vault_key.name}-aks-cluster-admin-hos"
  key_vault_id = data.azurerm_key_vault.vault.id
  value        = module.aks_cluster.admin_host
}

resource "azurerm_key_vault_secret" "aks-cluster-admin-password" {
  name         = "${module.naming.key_vault_key.name}-aks-cluster-admin-password"
  key_vault_id = data.azurerm_key_vault.vault.id
  value        = module.aks_cluster.admin_password
}

resource "azurerm_key_vault_secret" "aks-cluster-admin-username" {
  name         = "${module.naming.key_vault_key.name}-aks-cluster-admin-username"
  key_vault_id = data.azurerm_key_vault.vault.id
  value        = module.aks_cluster.admin_username
}

resource "azurerm_key_vault_secret" "aks-client-certificate" {
  name         = "${module.naming.key_vault_key.name}-aks-client-certificate"
  key_vault_id = data.azurerm_key_vault.vault.id
  value        = module.aks_cluster.client_certificate
}

resource "azurerm_key_vault_secret" "aks-client-key" {
  name         = "${module.naming.key_vault_key.name}-aks-client-key"
  key_vault_id = data.azurerm_key_vault.vault.id
  value        = module.aks_cluster.client_key
}

resource "azurerm_key_vault_secret" "aks-cluster-client-key" {
  name         = "${module.naming.key_vault_key.name}-aks-cluster-client-key"
  key_vault_id = data.azurerm_key_vault.vault.id
  value        = module.aks_cluster.client_certificate
}

resource "azurerm_key_vault_secret" "aks-cluster-host" {
  name         = "${module.naming.key_vault_key.name}-aks-cluster-host"
  key_vault_id = data.azurerm_key_vault.vault.id
  value        = module.aks_cluster.host
}

resource "azurerm_key_vault_secret" "aks-cluster-kube-raw" {
  name         = "${module.naming.key_vault_key.name}-aks-cluster-kube-raw"
  key_vault_id = data.azurerm_key_vault.vault.id
  value        = module.aks_cluster.kube_config_raw
}

resource "azurerm_key_vault_secret" "aks-cluster-password" {
  name         = "${module.naming.key_vault_key.name}-aks-cluster-password"
  key_vault_id = data.azurerm_key_vault.vault.id
  value        = module.aks_cluster.password
}

resource "azurerm_key_vault_secret" "aks-cluster-username" {
  name         = "${module.naming.key_vault_key.name}-aks-cluster-username"
  key_vault_id = data.azurerm_key_vault.vault.id
  value        = module.aks_cluster.username
}