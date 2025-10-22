locals {
  node_pools = {
    system = {
      name                  = "workers"
      vm_size               = "Standard_D2s_v4"
      node_count            = 2
      vnet_subnet           = { id = data.azurerm_subnet.aks-public-subnet.id }
      create_before_destroy = true
      upgrade_settings = {
        drain_timeout_in_minutes      = 0
        node_soak_duration_in_minutes = 0
        max_surge                     = "10%"
      }
      zones           = ["1", "2"]
      os_disk_size_gb = 60
    }
  }
}