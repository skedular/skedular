output "test_aks_id" {
  value = module.azure-aks.aks_cluster_aks_id
}

output "test_cluster_portal_fqdn" {
  value = module.azure-aks.aks_cluster_cluster_portal_fqdn
}
