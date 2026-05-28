module "naming" {
  source  = "Azure/naming/azurerm"
  version = "0.4.2"
  prefix  = [var.environment]
}

resource "azurerm_user_assigned_identity" "aks" {
  name                = "${module.naming.kubernetes_cluster.name}-identity"
  resource_group_name = var.resource_group
  location            = var.region
}

resource "azuread_group" "aks_admins" {
  display_name     = "${module.naming.kubernetes_cluster.name}-admins"
  security_enabled = true
  members = [
    data.azuread_user.kirill.object_id,
    data.azuread_user.morteza.object_id
  ]
}

module "aks_cluster" {
  source                    = "Azure/aks/azurerm"
  version                   = "11.0.0"
  location                  = var.region
  prefix                    = module.naming.kubernetes_cluster.name
  resource_group_name       = var.resource_group
  kubernetes_version        = var.kubernetes_version
  automatic_channel_upgrade = "patch"
  agents_availability_zones = ["1", "2"]
  agents_count              = null
  agents_max_count          = 2
  agents_max_pods           = 100
  agents_min_count          = 1
  agents_pool_name          = "aksnodepool"
  agents_pool_linux_os_configs = [
    {
      transparent_huge_page_enabled = "always"
      sysctl_configs = [
        {
          fs_aio_max_nr               = 65536
          fs_file_max                 = 100000
          fs_inotify_max_user_watches = 1000000
        }
      ]
    }
  ]
  agents_type                     = "VirtualMachineScaleSets"
  azure_policy_enabled            = true
  auto_scaling_enabled            = true
  local_account_disabled          = false
  log_analytics_workspace_enabled = true
  maintenance_window = {
    allowed = [
      {
        day   = "Sunday",
        hours = [22, 23]
      },
    ]
    not_allowed = []
  }
  net_profile_dns_service_ip        = "10.0.0.10"
  net_profile_service_cidr          = "10.0.0.0/16"
  network_plugin                    = "azure"
  network_policy                    = "azure"
  node_os_channel_upgrade           = "NodeImage"
  os_disk_size_gb                   = 60
  private_cluster_enabled           = false
  rbac_aad_azure_rbac_enabled       = true
  role_based_access_control_enabled = true
  rbac_aad_admin_group_object_ids   = [azuread_group.aks_admins.object_id]
  sku_tier                          = "Free"
  vnet_subnet                       = { id = data.azurerm_subnet.aks-system-subnet.id }

  agents_labels = {
    "node1" : "label1"
  }
  agents_tags = {
    "Agent" : "agentTag"
  }
  node_pools = local.node_pools
}

