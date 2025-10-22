# ---------------------------------------------------------------
# Terraform AWS Wrapper
#
# Required arguments:
#   ENV   = environment name  (e.g. dev, prod)
#   STACK = stack directory   (e.g. azure-networking)
#
# Examples:
#   make new        ENV=prod STACK=azure-networking
#   make plan       ENV=prod STACK=azure-networking
#   make apply      ENV=prod STACK=azure-networking
#   make plan-destroy ENV=prod STACK=azure-networking
#   make destroy    ENV=prod STACK=azure-networking
#   make clean      STACK=azure-networking
#
# Vars file location:
#   <STACK>/variables/<ENV>.tfvars
# ---------------------------------------------------------------

TF      ?= terraform
ENV     ?= # must be provided
STACK   ?=          # must be provided
VARS    := variables/$(ENV).tfvars
STACK_DIR := $(STACK)

ifndef STACK
$(error >>> STACK is required. Usage: make <target> ENV=dev STACK=my-stack)
endif

.PHONY: new plan apply plan-destroy destroy clean init workspace-check

# Create/select workspace
new:
	@echo ">>> Creating/selecting workspace '$(ENV)' in stack $(STACK)"
	@cd $(STACK_DIR) && \
		($(TF) workspace new $(ENV) 2>/dev/null || $(TF) workspace select $(ENV))

# Ensure workspace exists & selected
list:
	@cd $(STACK_DIR) && \
		if ! $(TF) workspace list | grep -q '$(ENV)'; then \
			echo "Workspace $(ENV) does not exist. Run 'make new ENV=$(ENV) STACK=$(STACK)' first."; \
			exit 1; \
		fi && \
		$(TF) workspace list && \
		$(TF) workspace select $(ENV) >/dev/null

# Terraform init
init:
	@echo ">>> Initializing terraform for stack $(STACK)"
	@cd $(STACK_DIR) && $(TF) init -upgrade

plan: init
	@echo ">>> Planning for stack $(STACK) env $(ENV)"
	@cd $(STACK_DIR) && \
		$(TF) workspace select $(ENV) 2>/dev/null && \
		$(TF) plan -input=false -out=tfplan -var-file=$(VARS)

apply: init
	@echo ">>> Applying for stack $(STACK) env $(ENV)"
	@cd $(STACK_DIR) && \
		$(TF) workspace select $(ENV) 2>/dev/null && \
		$(TF) apply -var-file=$(VARS)

plan-destroy: init
	@echo ">>> Planning destroy for stack $(STACK) env $(ENV)"
	@cd $(STACK_DIR) && \
		$(TF) workspace select $(ENV) 2>/dev/null && \
		$(TF) plan -destroy -out=tfplan-destroy -var-file=$(VARS)

destroy: init
	@echo ">>> Destroying stack $(STACK) env $(ENV)"
	@cd $(STACK_DIR) && \
		$(TF) workspace select $(ENV) 2>/dev/null && \
		$(TF) destroy -var-file=$(VARS)

clean:
	@echo ">>> Cleaning local terraform directories for stack $(STACK)"
	@cd $(STACK_DIR) && rm -rf .terraform .terraform.lock.hcl tfplan tfplan-destroy
