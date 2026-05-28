resource "google_project" "default" {
  org_id     = var.organization_id
  project_id = var.id
  name       = var.name
}

resource "aws_ssm_parameter" "default" {
  name  = var.parameter_store_name_project_id
  type  = "String"
  value = google_project.default.id
  tags  = local.tags
}
