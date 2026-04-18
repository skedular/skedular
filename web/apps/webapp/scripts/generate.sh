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

npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/customer/customer_v1.yaml --output ./src/clients/openapi/skedular/v1/customer/core/fetch --name SkedularCustomerCoreV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/customer/customer_graphql_v1.yaml --output ./src/clients/openapi/skedular/v1/customer/graphql/fetch --name SkedularCustomerGraphqlV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/customer/customer_workaround_v1.yaml --output ./src/clients/openapi/skedular/v1/customer/workaround/fetch --name SkedularCustomerWorkaroundV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/customer/customer_stripe_v1.yaml --output ./src/clients/openapi/skedular/v1/customer/stripe/fetch --name SkedularCustomerStripeV1Client --client fetch --exportSchemas true &

npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/core_v1.yaml --output ./src/clients/openapi/skedular/v1/core/fetch --name SkedularCoreV1Client --client fetch --exportSchemas true &

npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/location_v1.yaml --output ./src/clients/openapi/skedular/v1/location/fetch --name SkedularLocationV1Client --client fetch --exportSchemas true &

npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/marketplace_v1.yaml --output ./src/clients/openapi/skedular/v1/marketplace/fetch --name SkedularMarketplaceV1Client --client fetch --exportSchemas true &

npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/msteams/msteams_v1.yaml --output ./src/clients/openapi/skedular/v1/msteams/core/fetch --name SkedularMsTeamsCoreV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/msteams/msteams_graphql_v1.yaml --output ./src/clients/openapi/skedular/v1/msteams/graphql/fetch --name SkedularMsTeamsGraphqlV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/msteams/msteams_workaround_v1.yaml --output ./src/clients/openapi/skedular/v1/msteams/workaround/fetch --name SkedularMsTeamsWorkaroundV1Client --client fetch --exportSchemas true &

npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/organization_v1.yaml --output ./src/clients/openapi/skedular/v1/organization/fetch --name SkedularOrganizationV1Client --client fetch --exportSchemas true &

npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/slack/slack_v1.yaml --output ./src/clients/openapi/skedular/v1/slack/core/fetch --name SkedularSlackCoreV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/slack/slack_graphql_v1.yaml --output ./src/clients/openapi/skedular/v1/slack/graphql/fetch --name SkedularSlackGraphqlV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/slack/slack_callback_v1.yaml --output ./src/clients/openapi/skedular/v1/slack/callback/fetch --name SkedularSlackCallbackV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/slack/slack_workaround_v1.yaml --output ./src/clients/openapi/skedular/v1/slack/workaround/fetch --name SkedularSlackWorkaroundV1Client --client fetch --exportSchemas true &

npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/team/team_v1.yaml --output ./src/clients/openapi/skedular/v1/team/core/fetch --name SkedularTeamCoreV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/team/team_graphql_v1.yaml --output ./src/clients/openapi/skedular/v1/team/graphql/fetch --name SkedularTeamGraphqlV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../api-definitions/openapi/skedular/team/team_workaround_v1.yaml --output ./src/clients/openapi/skedular/v1/team/workaround/fetch --name SkedularTeamWorkaroundV1Client --client fetch --exportSchemas true &

wait
