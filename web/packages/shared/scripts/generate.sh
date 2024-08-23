#!/usr/bin/env sh

set -e
set -x

cd "$(dirname "${0}")/.."

npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/customer_v1.yaml --output ./src/clients/openapi/unityhub/customer/v1/fetch --name UnityHubCustomerClient --client fetch --exportSchemas true
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/organization_v1.yaml --output ./src/clients/openapi/unityhub/organization/v1/fetch --name UnityHubOrganizationClient --client fetch --exportSchemas true
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/booking_v1.yaml --output ./src/clients/openapi/unityhub/booking/v1/fetch --name UnityHubBookingClient --client fetch --exportSchemas true
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/notification_v1.yaml --output ./src/clients/openapi/unityhub/notification/v1/fetch --name UnityHubNotificationClient --client fetch --exportSchemas true
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/team_v1.yaml --output ./src/clients/openapi/unityhub/team/v1/fetch --name UnityHubTeamClient --client fetch --exportSchemas true
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/location_v1.yaml --output ./src/clients/openapi/unityhub/location/v1/fetch --name UnityHubLocationClient --client fetch --exportSchemas true
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/slack_v1.yaml --output ./src/clients/openapi/unityhub/slack/v1/fetch --name UnityHubSlackClient --client fetch --exportSchemas true
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/payment_v1.yaml --output ./src/clients/openapi/unityhub/payment/v1/fetch --name UnityHubBillingClient --client fetch --exportSchemas true
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/unityhub/billing_v1.yaml --output ./src/clients/openapi/unityhub/billing/v1/fetch --name UnityHubPaymentClient --client fetch --exportSchemas true
