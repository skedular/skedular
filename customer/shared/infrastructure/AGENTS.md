# Customer Infrastructure Workspace Config Notes

This file applies to `customer/shared/infrastructure`.

## Purpose

- This directory contains environment-specific workspace configuration files for the customer domain.
- Subdirectories correspond to deployment environments (`common`, `staging`, `production`, etc.).
- These files are used by infrastructure tooling and Aspire deployments to configure the customer domain host.

## Agent Rule

- Do not put application code here; this is infrastructure configuration only.
- Coordinate with the deployment/infrastructure team before changing production or staging configs.
