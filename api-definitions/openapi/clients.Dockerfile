# syntax = docker/dockerfile:1.7.0
ARG BUILD_IMAGE=mcr.microsoft.com/dotnet/sdk:8.0

FROM $BUILD_IMAGE AS build
LABEL maintainer="morteza.alizadeh@gmail.com"

RUN apt-get update -y && \
  apt-get install npm -y && \
  apt-get clean
RUN npm install -y -g nswag@latest

RUN mkdir -p /output/V1
COPY ["api-definitions/openapi", "/openapi"]

RUN nswag \
  openapi2csclient \
  /Input:/openapi/unityhub/gateway_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.UnityHub.Gateway.V1 \
  /Classname:GatewayClient \
  /Output:/output/UnityHub/V1/Gateway/Gateway.g.cs \
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
  /GenerateNullableReferenceTypes:true
    
RUN nswag \
  openapi2csclient \
  /Input:/openapi/unityhub/billing_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.UnityHub.Billing.V1 \
  /Classname:BillingClient \
  /Output:/output/UnityHub/Billing/V1/Billing.g.cs \
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
  /GenerateNullableReferenceTypes:true 

RUN nswag \
  openapi2csclient \
  /Input:/openapi/unityhub/booking_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.UnityHub.Booking.V1 \
  /Classname:BookingClient \
  /Output:/output/UnityHub/Booking/V1/Booking.g.cs \
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
  /GenerateNullableReferenceTypes:true 

RUN nswag \
  openapi2csclient \
  /Input:/openapi/unityhub/customer_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.UnityHub.Customer.V1 \
  /Classname:CustomerClient \
  /Output:/output/UnityHub/Customer/V1/Customer.g.cs \
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
  /GenerateNullableReferenceTypes:true

RUN nswag \
  openapi2csclient \
  /Input:/openapi/unityhub/location_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.UnityHub.Location.V1 \
  /Classname:LocationClient \
  /Output:/output/UnityHub/Location/V1/Location.g.cs \
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
  /GenerateNullableReferenceTypes:true

RUN nswag \
  openapi2csclient \
  /Input:/openapi/unityhub/notification_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.UnityHub.Notification.V1 \
  /Classname:NotificationClient \
  /Output:/output/UnityHub/Notification/V1/Notification.g.cs \
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
  /GenerateNullableReferenceTypes:true

RUN nswag \
  openapi2csclient \
  /Input:/openapi/unityhub/organization_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.UnityHub.Organization.V1 \
  /Classname:OrganizationClient \
  /Output:/output/UnityHub/Organization/V1/Organization.g.cs \
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
  /GenerateNullableReferenceTypes:true

RUN nswag \
  openapi2csclient \
  /Input:/openapi/unityhub/payment_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.UnityHub.Payment.V1 \
  /Classname:PaymentClient \
  /Output:/output/UnityHub/Payment/V1/Payment.g.cs \
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
  /GenerateNullableReferenceTypes:true

RUN nswag \
  openapi2csclient \
  /Input:/openapi/unityhub/slack_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.UnityHub.Slack.V1 \
  /Classname:SlackClient \
  /Output:/output/UnityHub/Slack/V1/Slack.g.cs \
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
  /GenerateNullableReferenceTypes:true

RUN nswag \
  openapi2csclient \
  /Input:/openapi/unityhub/team_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.UnityHub.Team.V1 \
  /Classname:TeamClient \
  /Output:/output/UnityHub/Team/V1/Team.g.cs \
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
  /GenerateNullableReferenceTypes:true
