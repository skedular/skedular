SHELL = /bin/bash
CURRENT_DIRECTORY = $(shell pwd)

# Go variables
GOFILES = $(shell find . -type f -name '*.go' -not -path "*/mock/*.go" -not -path "*.pb.go" -not -path "*_eventgen.go" -not -path "*_gen.go")

.PHONY: all
all: dep generate ## Runs dep generate

.PHONY: dep
dep: ## Install dependencies
	@cd $(CURRENT_DIRECTORY)
	@dotnet restore Skedular.slnx

.PHONY: generate
generate: ## Generate code
	@./api-definitions/generate.sh
	@./scripts/generate-graphql.sh
	@./web/apps/webapp/scripts/generate.sh
	@./web/apps/webapp-spaces/scripts/generate.sh
	@./web/apps/webapp-teams/scripts/generate.sh

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
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env pull

.PHONY: dep-restart
dep-restart:
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env pull
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env down dozzle postgres18 redis redisinsight kafka1 kowl zipkin jaeger
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env up --build -d dozzle postgres18 redis redisinsight kafka1 kowl zipkin jaeger

.PHONY: services-all-restart
services-all-restart:
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env pull
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env build
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env down
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env up --build -d

.PHONY: services-all-start
services-all-start:
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env pull
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env build
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env up --build -d

.PHONY: services-all-stop
services-all-stop:
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env down

.PHONY: services-all-terminate
services-all-terminate:
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env down -v

.PHONY: services-restart
services-restart:
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env pull
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env build
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env down staging-infra-provision staging-processors-01 staging-jobs-01 staging-apis-01 prod-infra-provision prod-processors-01 prod-apis-01 prod-jobs-01 prod-apis-01
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env up --build -d staging-infra-provision staging-processors-01 staging-jobs-01 staging-apis-01 prod-infra-provision prod-processors-01 prod-apis-01 prod-jobs-01 prod-apis-01

.PHONY: services-start
services-start:
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env pull
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env build
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env up --build -d staging-infra-provision staging-processors-01 staging-jobs-01 staging-apis-01 prod-infra-provision prod-processors-01 prod-apis-01 prod-jobs-01 prod-apis-01

.PHONY: services-stop
services-stop:
	docker compose -p unityhubio -f docker-compose-production.yml --env-file .env down staging-infra-provision staging-processors-01 staging-jobs-01 staging-apis-01 prod-infra-provision prod-processors-01 prod-apis-01 prod-jobs-01 prod-apis-01

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
