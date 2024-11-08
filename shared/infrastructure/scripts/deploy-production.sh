#!/bin/bash

set -e
set -x

APP_REGISTRATION_NAME="UnityHub"

APP_ID=$(az ad app list --display-name $APP_REGISTRATION_NAME --query "[0].appId" -o tsv)

if [ -z "$APP_ID" ]; then
  echo "App Registration '$APP_REGISTRATION_NAME' not found."
  exit 1
fi

APPLICATION_ID_URI="api://msteams.unityhub.io/$APP_ID"
az ad app update --id $APP_ID --identifier-uris $APPLICATION_ID_URI
