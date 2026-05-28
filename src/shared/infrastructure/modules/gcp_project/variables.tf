variable "tags" {
  type        = map(string)
  description = "tags"
}

variable "organization_id" {
  type        = string
  description = "organization id"
}

variable "id" {
  type        = string
  description = "project id"
}

variable "name" {
  type        = string
  description = "project name"
}

variable "parameter_store_name_project_id" {
  type        = string
  description = "project Id parameter store name"
}
