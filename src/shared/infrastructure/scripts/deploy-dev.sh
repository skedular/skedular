#!/bin/bash

set -e
set -x

APP_REGISTRATION_NAME="Skedular-dev"

APP_ID=$(az ad app list --display-name $APP_REGISTRATION_NAME --query "[0].appId" -o tsv)

if [ -z "$APP_ID" ]; then
  echo "App Registration '$APP_REGISTRATION_NAME' not found."
  exit 1
fi

APPLICATION_ID_URI="api://localhost:15002/$APP_ID"
az ad app update --id $APP_ID --identifier-uris $APPLICATION_ID_URI

az ad app permission add --id $APP_ID --api 00000003-0000-0000-c000-000000000000 --api-permissions $APP_ID=Scope
az ad app update --id $APP_ID --set oauth2Permissions=@- <<EOF
[
  {
    "id": "$(uuidgen)",
    "adminConsentDisplayName": "Skedular application can access app's web APIs",
    "adminConsentDescription": "Allows Skedular application to call the app's web APIs as the current user.",
    "userConsentDisplayName": "Skedular application can access app's web APIs and make requests on your behalf",
    "userConsentDescription": "Enable Skedular application to call this app's web APIs with the same rights that you have",
    "isEnabled": true,
    "type": "User",
    "value": "access_as_user"
  }
]
