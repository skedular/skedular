SHELL = /bin/bash
CURRENT_DIRECTORY = $(shell pwd)

# Go variables
GOFILES = $(shell find . -type f -name '*.go' -not -path "*/mock/*.go" -not -path "*.pb.go" -not -path "*_eventgen.go" -not -path "*_gen.go")

.PHONY: all
all: dep generate ## Runs dep generate

.PHONY: dep
dep: ## Install dependencies
	@cd $(CURRENT_DIRECTORY)
	@dotnet restore UnityHub.sln

.PHONY: generate
generate: ## Generate code
	@./api-definitions/generate.sh
	@./web/packages/shared/scripts/generate.sh

.PHONY: sync-web-schema
sync-web-schema: ## Sync GraphQL schema to web applications
	@./web/scripts/download-federated-schema.sh

.PHONY: lint
lint: ## run golanci-lint locally
	@terraform fmt -check -diff -recursive
	@./scripts/lint.sh

.PHONY: format
format: ## Format the source
	@terraform fmt -diff -recursive
	@./scripts/format.sh
	@goimports -w $(GOFILES)

.PHONY: services-restart
services-all-restart:
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env pull
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env build
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env down
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env up --build -d

.PHONY: services-start
services-all-start:
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env pull
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env build
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env up --build -d

.PHONY: services-stop
services-all-stop:
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env down

.PHONY: services-terminate
services-all-terminate:
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env down -v

services-restart:
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env pull
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env build
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env down staging-infra-provision staging-processors-01 staging-apis-01 prod-infra-provision prod-processors-01 prod-apis-01
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env up --build -d staging-infra-provision staging-processors-01 staging-apis-01 prod-infra-provision prod-processors-01 prod-apis-01

.PHONY: services-start
services-start:
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env pull
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env build
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env up --build -d staging-infra-provision staging-processors-01 staging-apis-01 prod-infra-provision prod-processors-01 prod-apis-01

.PHONY: services-stop
services-stop:
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env down staging-infra-provision staging-processors-01 staging-apis-01 prod-infra-provision prod-processors-01 prod-apis-01

.PHONY: publicwebsite-restart
publicwebsite-restart:
	docker compose -p unityhubio_publicwebsite -f docker-compose-publicwebsite.yml --env-file .env pull
	docker compose -p unityhubio_publicwebsite -f docker-compose-publicwebsite.yml --env-file .env build
	docker compose -p unityhubio_publicwebsite -f docker-compose-publicwebsite.yml --env-file .env down
	docker compose -p unityhubio_publicwebsite -f docker-compose-publicwebsite.yml --env-file .env up --build -d

.PHONY: publicwebsite-start
publicwebsite-start:
	docker compose -p unityhubio_publicwebsite -f docker-compose-publicwebsite.yml --env-file .env pull
	docker compose -p unityhubio_publicwebsite -f docker-compose-publicwebsite.yml --env-file .env build
	docker compose -p unityhubio_publicwebsite -f docker-compose-publicwebsite.yml --env-file .env up --build -d

.PHONY: publicwebsite-stop
publicwebsite-stop:
	docker compose -p unityhubio_publicwebsite -f docker-compose-publicwebsite.yml --env-file .env down

.PHONY: publicwebsite-terminate
publicwebsite-terminate:
	docker compose -p unityhubio_publicwebsite -f docker-compose-publicwebsite.yml --env-file .env down -v

.PHONY: publicwebsite-dev-restart
publicwebsite-dev-restart:
	docker compose -p unityhubio_publicwebsite_dev -f docker-compose-publicwebsite-dev.yml --env-file .env pull
	docker compose -p unityhubio_publicwebsite_dev -f docker-compose-publicwebsite-dev.yml --env-file .env build
	docker compose -p unityhubio_publicwebsite_dev -f docker-compose-publicwebsite-dev.yml --env-file .env down
	docker compose -p unityhubio_publicwebsite_dev -f docker-compose-publicwebsite-dev.yml --env-file .env up --build -d

.PHONY: publicwebsite-dev-start
publicwebsite-dev-start:
	docker compose -p unityhubio_publicwebsite_dev -f docker-compose-publicwebsite-dev.yml --env-file .env pull
	docker compose -p unityhubio_publicwebsite_dev -f docker-compose-publicwebsite-dev.yml --env-file .env build
	docker compose -p unityhubio_publicwebsite_dev -f docker-compose-publicwebsite-dev.yml --env-file .env up --build -d

.PHONY: publicwebsite-dev-stop
publicwebsite-dev-stop:
	docker compose -p unityhubio_publicwebsite_dev -f docker-compose-publicwebsite-dev.yml --env-file .env down

.PHONY: publicwebsite-dev-terminate
publicwebsite-dev-terminate:
	docker compose -p unityhubio_publicwebsite_dev -f docker-compose-publicwebsite-dev.yml --env-file .env down -v

.PHONY: help
.DEFAULT_GOAL := help
help: ## Get help output
	@grep -h -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

# Variable outputting/exporting rules
var-%: ; @echo $($*)
varexport-%: ; @echo $*=$($*)
