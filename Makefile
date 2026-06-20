SHELL = /bin/bash
CURRENT_DIRECTORY = $(shell pwd)

# Go variables
GOFILES = $(shell find . -type f -name '*.go' -not -path "*/mock/*.go" -not -path "*.pb.go" -not -path "*_eventgen.go" -not -path "*_gen.go")

# Docker Compose base command
DC = docker compose -p unityhubio -f docker-compose-production.yml --env-file .env

# Service groups
DEP_SERVICES     = dozzle postgres18 redis redisinsight kafka1 kowl aspire-dashboard temporal temporal-admin-tools temporal-ui
STAGING_SERVICES = staging-infra-provision staging-processors-01 staging-jobs-01 staging-apis-01
PROD_SERVICES    = prod-infra-provision prod-processors-01 prod-jobs-01 prod-apis-01

.PHONY: all
all: dep generate ## Runs dep generate

.PHONY: dep
dep: ## Install dependencies
	@cd $(CURRENT_DIRECTORY)
	@dotnet restore src/Skedular.slnx

.PHONY: generate
generate: ## Generate code
	@./api-definitions/generate.sh
	@./scripts/generate-graphql.sh
	@./src/web/apps/webapp/scripts/generate.sh
	@./src/web/apps/webapp-spaces/scripts/generate.sh
	@./src/web/apps/webapp-teams/scripts/generate.sh
	@./src/web/apps/webapp-host/scripts/generate.sh

.PHONY: lint
lint: ## run golanci-lint locally
	@terraform fmt -check -diff -recursive
	@./scripts/lint.sh

.PHONY: format
format: ## Format the source
	@terraform fmt -diff -recursive
	@./scripts/format.sh
	@goimports -w $(GOFILES)

.PHONY: images-pull
images-pull:
	$(DC) pull

.PHONY: dep-start
dep-start: ## Start dependency/infrastructure services
	$(DC) pull
	$(DC) up --build -d $(DEP_SERVICES)

.PHONY: dep-stop
dep-stop: ## Stop dependency/infrastructure services
	$(DC) down $(DEP_SERVICES)

.PHONY: dep-restart
dep-restart: ## Restart dependency/infrastructure services
	$(DC) pull
	$(DC) down $(DEP_SERVICES)
	$(DC) up --build -d $(DEP_SERVICES)

.PHONY: services-all-restart
services-all-restart:
	$(DC) pull
	$(DC) build
	$(DC) down
	$(DC) up --build -d

.PHONY: services-all-start
services-all-start:
	$(DC) pull
	$(DC) build
	$(DC) up --build -d

.PHONY: services-all-stop
services-all-stop:
	$(DC) down

.PHONY: services-all-terminate
services-all-terminate:
	$(DC) down -v

.PHONY: services-restart
services-restart:
	$(DC) pull
	$(DC) build
	$(DC) down $(STAGING_SERVICES) $(PROD_SERVICES)
	$(DC) up --build -d $(STAGING_SERVICES) $(PROD_SERVICES)

.PHONY: services-start
services-start:
	$(DC) pull
	$(DC) build
	$(DC) up --build -d $(STAGING_SERVICES) $(PROD_SERVICES)

.PHONY: services-stop
services-stop:
	$(DC) down $(STAGING_SERVICES) $(PROD_SERVICES)

.PHONY: staging-restart
staging-restart: ## Restart staging services only
	$(DC) pull
	$(DC) build
	$(DC) down $(STAGING_SERVICES)
	$(DC) up --build -d $(STAGING_SERVICES)

.PHONY: staging-start
staging-start: ## Start staging services only
	$(DC) pull
	$(DC) build
	$(DC) up --build -d $(STAGING_SERVICES)

.PHONY: staging-stop
staging-stop: ## Stop staging services only
	$(DC) down $(STAGING_SERVICES)

.PHONY: prod-restart
prod-restart: ## Restart production services only
	$(DC) pull
	$(DC) build
	$(DC) down $(PROD_SERVICES)
	$(DC) up --build -d $(PROD_SERVICES)

.PHONY: prod-start
prod-start: ## Start production services only
	$(DC) pull
	$(DC) build
	$(DC) up --build -d $(PROD_SERVICES)

.PHONY: prod-stop
prod-stop: ## Stop production services only
	$(DC) down $(PROD_SERVICES)

.PHONY: crm-restart
crm-restart:
	docker compose -p skedular_crm -f docker-compose-crm.yml --env-file .env pull
	docker compose -p skedular_crm -f docker-compose-crm.yml --env-file .env build
	docker compose -p skedular_crm -f docker-compose-crm.yml --env-file .env down
	docker compose -p skedular_crm -f docker-compose-crm.yml --env-file .env up --build -d

.PHONY: crm-start
crm-start:
	docker compose -p skedular_crm -f docker-compose-crm.yml --env-file .env pull
	docker compose -p skedular_crm -f docker-compose-crm.yml --env-file .env build
	docker compose -p skedular_crm -f docker-compose-crm.yml --env-file .env up --build -d

.PHONY: crm-stop
crm-stop:
	docker compose -p skedular_crm -f docker-compose-crm.yml --env-file .env down

.PHONY: crm-terminate
crm-terminate:
	docker compose -p skedular_crm -f docker-compose-crm.yml --env-file .env down -v

.PHONY: help
.DEFAULT_GOAL := help
help: ## Get help output
	@grep -h -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

# Variable outputting/exporting rules
var-%: ; @echo $($*)
varexport-%: ; @echo $*=$($*)
