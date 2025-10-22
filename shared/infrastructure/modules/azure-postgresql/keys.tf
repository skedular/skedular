resource "azurerm_key_vault_secret" "aks-admin-client-certificate" {
  name         = "${module.naming.key_vault_key.name}-pg-admin-password"
  key_vault_id = data.azurerm_key_vault.vault.id
  value        = random_password.adminpassword.result
}