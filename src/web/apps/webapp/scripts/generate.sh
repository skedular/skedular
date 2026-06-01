#!/usr/bin/env sh

set -e
set -x

cd "$(dirname "${0}")/.."

npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/gateway/gateway_core_v1.yaml --output ./src/clients/openapi/skedular/v1/gateway/core/fetch --name SkedularGatewayCoreV1Client --client fetch --exportSchemas true &

npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/booking/booking_core_v1.yaml --output ./src/clients/openapi/skedular/v1/booking/core/fetch --name SkedularBookingCoreV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/booking/booking_graphql_v1.yaml --output ./src/clients/openapi/skedular/v1/booking/graphql/fetch --name SkedularBookingGraphqlV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/booking/booking_stripe_webhook_v1.yaml --output ./src/clients/openapi/skedular/v1/booking/stripe-webhook/fetch --name SkedularBookingStripeWebhookV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/booking/booking_xero_webhook_v1.yaml --output ./src/clients/openapi/skedular/v1/booking/xero-webhook/fetch --name SkedularBookingXeroWebhookV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/booking/booking_workaround_v1.yaml --output ./src/clients/openapi/skedular/v1/booking/workaround/fetch --name SkedularBookingWorkaroundV1Client --client fetch --exportSchemas true &

npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/customer/customer_core_v1.yaml --output ./src/clients/openapi/skedular/v1/customer/core/fetch --name SkedularCustomerCoreV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/customer/customer_graphql_v1.yaml --output ./src/clients/openapi/skedular/v1/customer/graphql/fetch --name SkedularCustomerGraphqlV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/customer/customer_workaround_v1.yaml --output ./src/clients/openapi/skedular/v1/customer/workaround/fetch --name SkedularCustomerWorkaroundV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/customer/customer_stripe_v1.yaml --output ./src/clients/openapi/skedular/v1/customer/stripe/fetch --name SkedularCustomerStripeV1Client --client fetch --exportSchemas true &

npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/core/core_core_v1.yaml --output ./src/clients/openapi/skedular/v1/core/core/fetch --name SkedularCoreCoreV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/core/core_graphql_v1.yaml --output ./src/clients/openapi/skedular/v1/core/graphql/fetch --name SkedularCoreGraphqlV1Client --client fetch --exportSchemas true &

npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/location/location_core_v1.yaml --output ./src/clients/openapi/skedular/v1/location/core/fetch --name SkedularLocationCoreV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/location/location_graphql_v1.yaml --output ./src/clients/openapi/skedular/v1/location/graphql/fetch --name SkedularLocationGraphqlV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/location/location_workaround_v1.yaml --output ./src/clients/openapi/skedular/v1/location/workaround/fetch --name SkedularLocationWorkaroundV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/location/location_analytics_v1.yaml --output ./src/clients/openapi/skedular/v1/location/analytics/fetch --name SkedularLocationAnalyticsV1Client --client fetch --exportSchemas true &

npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/marketplace/marketplace_core_v1.yaml --output ./src/clients/openapi/skedular/v1/marketplace/core/fetch --name SkedularMarketplaceCoreV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/marketplace/marketplace_graphql_v1.yaml --output ./src/clients/openapi/skedular/v1/marketplace/graphql/fetch --name SkedularMarketplaceGraphqlV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/marketplace/marketplace_workaround_v1.yaml --output ./src/clients/openapi/skedular/v1/marketplace/workaround/fetch --name SkedularMarketplaceWorkaroundV1Client --client fetch --exportSchemas true &

npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/organization/organization_core_v1.yaml --output ./src/clients/openapi/skedular/v1/organization/core/fetch --name SkedularOrganizationCoreV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/organization/organization_graphql_v1.yaml --output ./src/clients/openapi/skedular/v1/organization/graphql/fetch --name SkedularOrganizationGraphqlV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/organization/organization_workaround_v1.yaml --output ./src/clients/openapi/skedular/v1/organization/workaround/fetch --name SkedularOrganizationWorkaroundV1Client --client fetch --exportSchemas true &

npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/slack/slack_core_v1.yaml --output ./src/clients/openapi/skedular/v1/slack/core/fetch --name SkedularSlackCoreV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/slack/slack_graphql_v1.yaml --output ./src/clients/openapi/skedular/v1/slack/graphql/fetch --name SkedularSlackGraphqlV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/slack/slack_callback_v1.yaml --output ./src/clients/openapi/skedular/v1/slack/callback/fetch --name SkedularSlackCallbackV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/slack/slack_workaround_v1.yaml --output ./src/clients/openapi/skedular/v1/slack/workaround/fetch --name SkedularSlackWorkaroundV1Client --client fetch --exportSchemas true &

npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/team/team_core_v1.yaml --output ./src/clients/openapi/skedular/v1/team/core/fetch --name SkedularTeamCoreV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/team/team_graphql_v1.yaml --output ./src/clients/openapi/skedular/v1/team/graphql/fetch --name SkedularTeamGraphqlV1Client --client fetch --exportSchemas true &
npx openapi-typescript-codegen --input ../../../../api-definitions/openapi/skedular/team/team_workaround_v1.yaml --output ./src/clients/openapi/skedular/v1/team/workaround/fetch --name SkedularTeamWorkaroundV1Client --client fetch --exportSchemas true &

wait
