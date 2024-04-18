# Manual infrastructure setup

## AWS

- Create account in AWS for each environment
- Run aws-oneoff for each environment using session token

## GCP

- Create GCP projects for all environments
- Enable IAM API in each one of them
- Run gcp-oneoff for each project
- Create OAuth Conset screen in each project
- Create OAuth client ID credential for each one of them

gcloud auth application-default login
gcloud config set project unityhub-staging
gcloud config set project unityhub-production
