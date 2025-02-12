#!/usr/bin/env sh

set -e
set -x

cd "$(dirname "${0}")/.."

npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/customer_v1.yaml --output ./src/clients/openapi/skedular/customer/v1/fetch --name SkedularCustomerClient --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/organization_v1.yaml --output ./src/clients/openapi/skedular/organization/v1/fetch --name SkedularOrganizationClient --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/booking_v1.yaml --output ./src/clients/openapi/skedular/booking/v1/fetch --name SkedularBookingClient --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/notification_v1.yaml --output ./src/clients/openapi/skedular/notification/v1/fetch --name SkedularNotificationClient --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/team_v1.yaml --output ./src/clients/openapi/skedular/team/v1/fetch --name SkedularTeamClient --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/location_v1.yaml --output ./src/clients/openapi/skedular/location/v1/fetch --name SkedularLocationClient --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/slack_v1.yaml --output ./src/clients/openapi/skedular/slack/v1/fetch --name SkedularSlackClient --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/payment_v1.yaml --output ./src/clients/openapi/skedular/payment/v1/fetch --name SkedularBillingClient --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/billing_v1.yaml --output ./src/clients/openapi/skedular/billing/v1/fetch --name SkedularPaymentClient --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/msteams_v1.yaml --output ./src/clients/openapi/skedular/msteams/v1/fetch --name SkedularMSTeamsClient --client fetch --exportSchemas true &
wait
