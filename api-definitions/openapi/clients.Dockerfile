# syntax = docker/dockerfile:1.12.0
ARG BUILD_IMAGE=mcr.microsoft.com/dotnet/sdk:10.0

FROM $BUILD_IMAGE AS build
LABEL maintainer="morteza.alizadeh@gmail.com"

RUN apt-get update -y && \
  apt-get install npm -y && \
  apt-get clean
RUN npm install -y -g nswag@latest

RUN mkdir -p /output/V1
COPY ["api-definitions/openapi", "/openapi"]

ENV DOTNET_ROLL_FORWARD=LatestMajor

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/gateway_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Gateway.V1 \
  /Classname:GatewayClient \
  /Output:/output/Skedular/Gateway/V1/Gateway.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

#########################################################################################################################
RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/booking/booking_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Booking.Core.V1 \
  /Classname:BookingCoreClient \
  /Output:/output/Skedular/Booking/V1/BookingCore.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/booking/booking_graphql_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Booking.Graphql.V1 \
  /Classname:BookingGraphqlClient \
  /Output:/output/Skedular/Booking/V1/BookingGraphql.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/booking/booking_stripe_webhook_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Booking.StripeWebhook.V1 \
  /Classname:BookingStripeWebhookClient \
  /Output:/output/Skedular/Booking/V1/BookingStripeWebhook.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/booking/booking_xero_webhook_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Booking.XeroWebhook.V1 \
  /Classname:BookingXeroWebhookClient \
  /Output:/output/Skedular/Booking/V1/BookingXeroWebhook.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/booking/booking_workaround_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.BookingWorkaround.V1 \
  /Classname:BookingWorkaroundClient \
  /Output:/output/Skedular/Booking/V1/BookingWorkaround.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson
#########################################################################################################################

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/customer/customer_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Customer.Core.V1 \
  /Classname:CustomerCoreClient \
  /Output:/output/Skedular/Customer/V1/CustomerCore.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/customer/customer_graphql_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Customer.Graphql.V1 \
  /Classname:CustomerGraphqlClient \
  /Output:/output/Skedular/Customer/V1/CustomerGraphql.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/customer/customer_workaround_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Customer.Workaround.V1 \
  /Classname:CustomerWorkaroundClient \
  /Output:/output/Skedular/Customer/V1/CustomerWorkaround.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/customer/customer_stripe_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Customer.Stripe.V1 \
  /Classname:CustomerStripeClient \
  /Output:/output/Skedular/Customer/V1/CustomerStripe.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/location/location_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Location.Core.V1 \
  /Classname:LocationCoreClient \
  /Output:/output/Skedular/Location/V1/LocationCore.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/location/location_graphql_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Location.Graphql.V1 \
  /Classname:LocationGraphqlClient \
  /Output:/output/Skedular/Location/V1/LocationGraphql.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/location/location_workaround_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Location.Workaround.V1 \
  /Classname:LocationWorkaroundClient \
  /Output:/output/Skedular/Location/V1/LocationWorkaround.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/location/location_analytics_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Location.Analytics.V1 \
  /Classname:LocationAnalyticsClient \
  /Output:/output/Skedular/Location/V1/LocationAnalytics.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/marketplace/marketplace_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Marketplace.Core.V1 \
  /Classname:MarketplaceCoreClient \
  /Output:/output/Skedular/Marketplace/V1/MarketplaceCore.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/marketplace/marketplace_graphql_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Marketplace.Graphql.V1 \
  /Classname:MarketplaceGraphqlClient \
  /Output:/output/Skedular/Marketplace/V1/MarketplaceGraphql.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/marketplace/marketplace_workaround_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Marketplace.Workaround.V1 \
  /Classname:MarketplaceWorkaroundClient \
  /Output:/output/Skedular/Marketplace/V1/MarketplaceWorkaround.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/msteams/msteams_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.MsTeams.Core.V1 \
  /Classname:MsTeamsCoreClient \
  /Output:/output/Skedular/MsTeams/V1/MsTeamsCore.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/msteams/msteams_graphql_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.MsTeams.Graphql.V1 \
  /Classname:MsTeamsGraphqlClient \
  /Output:/output/Skedular/MsTeams/V1/MsTeamsGraphql.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/msteams/msteams_workaround_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.MsTeams.Workaround.V1 \
  /Classname:MsTeamsWorkaroundClient \
  /Output:/output/Skedular/MsTeams/V1/MsTeamsWorkaround.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/organization_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Organization.V1 \
  /Classname:OrganizationClient \
  /Output:/output/Skedular/Organization/V1/Organization.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/slack/slack_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Slack.Core.V1 \
  /Classname:SlackCoreClient \
  /Output:/output/Skedular/Slack/V1/SlackCore.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/slack/slack_graphql_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Slack.Graphql.V1 \
  /Classname:SlackGraphqlClient \
  /Output:/output/Skedular/Slack/V1/SlackGraphql.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/slack/slack_callback_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Slack.Callback.V1 \
  /Classname:SlackCallbackClient \
  /Output:/output/Skedular/Slack/V1/SlackCallback.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/slack/slack_workaround_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Slack.Workaround.V1 \
  /Classname:SlackWorkaroundClient \
  /Output:/output/Skedular/Slack/V1/SlackWorkaround.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/team/team_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Team.Core.V1 \
  /Classname:TeamCoreClient \
  /Output:/output/Skedular/Team/V1/TeamCore.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/team/team_graphql_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Team.Graphql.V1 \
  /Classname:TeamGraphqlClient \
  /Output:/output/Skedular/Team/V1/TeamGraphql.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/team/team_workaround_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Team.Workaround.V1 \
  /Classname:TeamWorkaroundClient \
  /Output:/output/Skedular/Team/V1/TeamWorkaround.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/core_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Core.V1 \
  /Classname:CoreClient \
  /Output:/output/Skedular/Core/V1/Core.g.cs \
  /GenerateClientClasses:true \
  /OperationGenerationMode:SingleClientFromOperationId \
  /GenerateClientInterfaces:true \
  /InjectHttpClient:true \
  /UseBaseUrl:false \
  /GenerateOptionalParameters:true \
  /GenerateJsonMethods:false \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
  /GenerateDtoTypes:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson
