# Backend Key Strategy

Terraform state keys must be isolated per product and workspace.

## Pattern

`<product>/<workspace>/terraform.tfstate`

## Products

- webapp
- webapp-teams
- webapp-spaces
