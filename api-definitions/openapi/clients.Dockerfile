# syntax = docker/dockerfile:1.12.0
ARG BUILD_IMAGE=mcr.microsoft.com/dotnet/sdk:9.0

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

RUN nswag \
  openapi2csclient \
  /Input:/openapi/skedular/booking_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Booking.V1 \
  /Classname:BookingClient \
  /Output:/output/Skedular/Booking/V1/Booking.g.cs \
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
  /Input:/openapi/skedular/customer_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Customer.V1 \
  /Classname:CustomerClient \
  /Output:/output/Skedular/Customer/V1/Customer.g.cs \
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
  /Input:/openapi/skedular/location_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Location.V1 \
  /Classname:LocationClient \
  /Output:/output/Skedular/Location/V1/Location.g.cs \
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
  /Input:/openapi/skedular/marketplace_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Marketplace.V1 \
  /Classname:MarketplaceClient \
  /Output:/output/Skedular/Marketplace/V1/Marketplace.g.cs \
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
  /Input:/openapi/skedular/msteams_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.MsTeams.V1 \
  /Classname:MsTeamsClient \
  /Output:/output/Skedular/MsTeams/V1/MsTeams.g.cs \
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
  /Input:/openapi/skedular/slack_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Slack.V1 \
  /Classname:SlackClient \
  /Output:/output/Skedular/Slack/V1/Slack.g.cs \
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
  /Input:/openapi/skedular/team_v1.yaml \
  /Namespace:Api.Shared.Clients.OpenApi.Skedular.Team.V1 \
  /Classname:TeamClient \
  /Output:/output/Skedular/Team/V1/Team.g.cs \
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

RUN nswag \
  openapi2csclient \
  /Input:/openapi/nominatim/nominatim.openapi_v4.json \
  /Namespace:Api.Shared.Clients.OpenApi.Nominatim.V4 \
  /Classname:NominatimClient \
  /Output:/output/Nominatim/V4/Nominatim.g.cs \
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

RUN sed -i '1i#pragma warning disable CS8981' /output/Nominatim/V4/Nominatim.g.cs
RUN sed -i '/public Geometry Geometry { get; set; } = default!;/s/^/\/\/ /' /output/Nominatim/V4/Nominatim.g.cs && \
  sed -i '/\[System.Text.Json.Serialization.JsonPropertyName("geometry")\]/s/^/\/\/ /' /output/Nominatim/V4/Nominatim.g.cs


