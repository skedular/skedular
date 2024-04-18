#!/usr/bin/env sh

set -e
set -x

cd "$(dirname "${0}")/.."

mkdir -p src/openapi/clients
rm -rf src/openapi/clients/unityhub


npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/customer_v1.yaml --output ./src/openapi/clients/unityhub/customer/v1/fetch --name UnityHubCustomerClient --client fetch --exportSchemas true
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/organization_v1.yaml --output ./src/openapi/clients/unityhub/organization/v1/fetch --name UnityHubOrganizationClient --client fetch --exportSchemas true
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/booking_v1.yaml --output ./src/openapi/clients/unityhub/booking/v1/fetch --name UnityHubBookingClient --client fetch --exportSchemas true
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/notification_v1.yaml --output ./src/openapi/clients/unityhub/notification/v1/fetch --name UnityHubNotificationClient --client fetch --exportSchemas true
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/team_v1.yaml --output ./src/openapi/clients/unityhub/team/v1/fetch --name UnityHubTeamClient --client fetch --exportSchemas true
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/location_v1.yaml --output ./src/openapi/clients/unityhub/location/v1/fetch --name UnityHubLocationClient --client fetch --exportSchemas true
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/slack_v1.yaml --output ./src/openapi/clients/unityhub/slack/v1/fetch --name UnityHubSlackClient --client fetch --exportSchemas true
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/payment_v1.yaml --output ./src/openapi/clients/unityhub/payment/v1/fetch --name UnityHubBillingClient --client fetch --exportSchemas true
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/billing_v1.yaml --output ./src/openapi/clients/unityhub/billing/v1/fetch --name UnityHubPaymentClient --client fetch --exportSchemas true
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/msteams_v1.yaml --output ./src/openapi/clients/unityhub/msteams/v1/fetch --name UnityHubMSTeamsClient --client fetch --exportSchemas true