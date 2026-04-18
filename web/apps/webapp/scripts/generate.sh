#!/usr/bin/env sh

set -e
set -x

cd "$(dirname "${0}")/.."

npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/gateway_v1.yaml --output ./src/clients/openapi/skedular/v1/gateway/fetch --name SkedularGatewayV1Client --client fetch --exportSchemas true &

npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/booking/booking_v1.yaml --output ./src/clients/openapi/skedular/v1/booking/core/fetch --name SkedularBookingCoreV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/booking/booking_graphql_v1.yaml --output ./src/clients/openapi/skedular/v1/booking/graphql/fetch --name SkedularBookingGraphqlV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/booking/booking_stripe_webhook_v1.yaml --output ./src/clients/openapi/skedular/v1/booking/stripe-webhook/fetch --name SkedularBookingStripeWebhookV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/booking/booking_xero_webhook_v1.yaml --output ./src/clients/openapi/skedular/v1/booking/xero-webhook/fetch --name SkedularBookingXeroWebhookV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/booking/booking_workaround_v1.yaml --output ./src/clients/openapi/skedular/v1/booking/workaround/fetch --name SkedularBookingWorkaroundV1Client --client fetch --exportSchemas true &

npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/customer_v1.yaml --output ./src/clients/openapi/skedular/v1/customer/fetch --name SkedularCustomerV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/organization_v1.yaml --output ./src/clients/openapi/skedular/v1/organization/fetch --name SkedularOrganizationV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/team_v1.yaml --output ./src/clients/openapi/skedular/v1/team/fetch --name SkedularTeamClient --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/location_v1.yaml --output ./src/clients/openapi/skedular/v1/location/fetch --name SkedularLocationV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/slack_v1.yaml --output ./src/clients/openapi/skedular/v1/slack/fetch --name SkedularSlackClient --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/msteams_v1.yaml --output ./src/clients/openapi/skedular/v1/msteams/fetch --name SkedularMSTeamsV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/marketplace_v1.yaml --output ./src/clients/openapi/skedular/v1/marketplace/fetch --name SkedularMarketplaceV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/core_v1.yaml --output ./src/clients/openapi/skedular/v1/core/fetch --name SkedularCoreV1Client --client fetch --exportSchemas true &

wait
