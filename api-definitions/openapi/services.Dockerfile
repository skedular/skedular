# syntax = docker/dockerfile:1.12.0
ARG BUILD_IMAGE=mcr.microsoft.com/dotnet/sdk:10.0

FROM $BUILD_IMAGE AS build
LABEL maintainer="morteza.alizadeh@gmail.com"

RUN apt-get update -y && \
  apt-get install npm -y && \
  apt-get clean
RUN npm install -y -g nswag@latest

COPY [".git", "shared/.git"]
COPY ["shared/Enterprise.Shared", "shared/Enterprise.Shared"]
COPY ["shared/Skedularctl", "shared/Skedularctl"]
WORKDIR shared/Skedularctl
RUN --mount=type=cache,target=~/.nuget/packages dotnet restore "Skedularctl.csproj"
RUN --mount=type=cache,target=~/.nuget/packages dotnet build "Skedularctl.csproj" --no-restore -c Release -o /app/build
RUN --mount=type=cache,target=~/.nuget/packages dotnet publish "Skedularctl.csproj" -c Release -o /app/publish

RUN mkdir -p /output/V1
COPY ["api-definitions/openapi", "/openapi"]

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/skedular/gateway_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.Skedular.Gateway.V1 \
  /Classname:Gateway \
  /Output:/output/Skedular/Gateway/V1/Gateway.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson \
  /ExcludedTypeNames:FileParameter

RUN sed -i '1iusing FileParameter = Microsoft.AspNetCore.Http.IFormFile;' /output/Skedular/Gateway/V1/Gateway.g.cs

#########################################################################################################################
RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/skedular/booking/booking_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.Skedular.Booking.Core.V1 \
  /Classname:BookingCore \
  /Output:/output/Skedular/Booking/V1/BookingCore.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson \
  /ExcludedTypeNames:FileParameter

RUN sed -i '1iusing FileParameter = Microsoft.AspNetCore.Http.IFormFile;' /output/Skedular/Booking/V1/BookingCore.g.cs
RUN /app/publish/Skedularctl mcp-tool-generate \
  --input-file /output/Skedular/Booking/V1/BookingCore.g.cs \
  --output-file /output/Skedular/Booking/V1/BookingCore.g.cs

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/skedular/booking/booking_graphql_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.Skedular.Booking.Graphql.V1 \
  /Classname:BookingGraphql \
  /Output:/output/Skedular/Booking/V1/BookingGraphql.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson \
  /ExcludedTypeNames:FileParameter

RUN sed -i '1iusing FileParameter = Microsoft.AspNetCore.Http.IFormFile;' /output/Skedular/Booking/V1/BookingGraphql.g.cs
RUN /app/publish/Skedularctl mcp-tool-generate \
  --input-file /output/Skedular/Booking/V1/BookingGraphql.g.cs \
  --output-file /output/Skedular/Booking/V1/BookingGraphql.g.cs

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/skedular/booking/booking_stripe_webhook_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.Skedular.Booking.StripeWebhook.V1 \
  /Classname:BookingStripeWebhook \
  /Output:/output/Skedular/Booking/V1/BookingStripeWebhook.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson \
  /ExcludedTypeNames:FileParameter

RUN sed -i '1iusing FileParameter = Microsoft.AspNetCore.Http.IFormFile;' /output/Skedular/Booking/V1/BookingStripeWebhook.g.cs
RUN /app/publish/Skedularctl mcp-tool-generate \
  --input-file /output/Skedular/Booking/V1/BookingStripeWebhook.g.cs \
  --output-file /output/Skedular/Booking/V1/BookingStripeWebhook.g.cs

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/skedular/booking/booking_xero_webhook_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.Skedular.Booking.XeroWebhook.V1 \
  /Classname:BookingXeroWebhook \
  /Output:/output/Skedular/Booking/V1/BookingXeroWebhook.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson \
  /ExcludedTypeNames:FileParameter

RUN sed -i '1iusing FileParameter = Microsoft.AspNetCore.Http.IFormFile;' /output/Skedular/Booking/V1/BookingXeroWebhook.g.cs
RUN /app/publish/Skedularctl mcp-tool-generate \
  --input-file /output/Skedular/Booking/V1/BookingXeroWebhook.g.cs \
  --output-file /output/Skedular/Booking/V1/BookingXeroWebhook.g.cs

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/skedular/booking/booking_workaround_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.Skedular.BookingWorkaround.V1 \
  /Classname:BookingWorkaround \
  /Output:/output/Skedular/Booking/V1/BookingWorkaround.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson \
  /ExcludedTypeNames:FileParameter

RUN sed -i '1iusing FileParameter = Microsoft.AspNetCore.Http.IFormFile;' /output/Skedular/Booking/V1/BookingWorkaround.g.cs
RUN /app/publish/Skedularctl mcp-tool-generate \
  --input-file /output/Skedular/Booking/V1/BookingWorkaround.g.cs \
  --output-file /output/Skedular/Booking/V1/BookingWorkaround.g.cs
#########################################################################################################################

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/skedular/customer_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.Skedular.Customer.V1 \
  /Classname:Customer \
  /Output:/output/Skedular/Customer/V1/Customer.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson \
  /ExcludedTypeNames:FileParameter

RUN sed -i '1iusing FileParameter = Microsoft.AspNetCore.Http.IFormFile;' /output/Skedular/Customer/V1/Customer.g.cs
RUN /app/publish/Skedularctl mcp-tool-generate \
  --input-file /output/Skedular/Customer/V1/Customer.g.cs \
  --output-file /output/Skedular/Customer/V1/Customer.g.cs

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/skedular/location_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.Skedular.Location.V1 \
  /Classname:Location \
  /Output:/output/Skedular/Location/V1/Location.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson \
  /ExcludedTypeNames:FileParameter

RUN sed -i '1iusing FileParameter = Microsoft.AspNetCore.Http.IFormFile;' /output/Skedular/Location/V1/Location.g.cs
RUN /app/publish/Skedularctl mcp-tool-generate \
  --input-file /output/Skedular/Location/V1/Location.g.cs \
  --output-file /output/Skedular/Location/V1/Location.g.cs

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/skedular/marketplace_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.Skedular.Marketplace.V1 \
  /Classname:Marketplace \
  /Output:/output/Skedular/Marketplace/V1/Marketplace.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson \
  /ExcludedTypeNames:FileParameter

RUN sed -i '1iusing FileParameter = Microsoft.AspNetCore.Http.IFormFile;' /output/Skedular/Marketplace/V1/Marketplace.g.cs
RUN /app/publish/Skedularctl mcp-tool-generate \
  --input-file /output/Skedular/Marketplace/V1/Marketplace.g.cs \
  --output-file /output/Skedular/Marketplace/V1/Marketplace.g.cs

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/skedular/msteams_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.Skedular.MsTeams.V1 \
  /Classname:MsTeams \
  /Output:/output/Skedular/MsTeams/V1/MsTeams.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson \
  /ExcludedTypeNames:FileParameter

RUN sed -i '1iusing FileParameter = Microsoft.AspNetCore.Http.IFormFile;' /output/Skedular/MsTeams/V1/MsTeams.g.cs
RUN /app/publish/Skedularctl mcp-tool-generate \
  --input-file /output/Skedular/MsTeams/V1/MsTeams.g.cs \
  --output-file /output/Skedular/MsTeams/V1/MsTeams.g.cs

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/skedular/organization_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.Skedular.Organization.V1 \
  /Classname:Organization \
  /Output:/output/Skedular/Organization/V1/Organization.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson \
  /ExcludedTypeNames:FileParameter

RUN sed -i '1iusing FileParameter = Microsoft.AspNetCore.Http.IFormFile;' /output/Skedular/Organization/V1/Organization.g.cs
RUN /app/publish/Skedularctl mcp-tool-generate \
  --input-file /output/Skedular/Organization/V1/Organization.g.cs \
  --output-file /output/Skedular/Organization/V1/Organization.g.cs

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/skedular/slack_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.Skedular.Slack.V1 \
  /Classname:Slack \
  /Output:/output/Skedular/Slack/V1/Slack.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson \
  /ExcludedTypeNames:FileParameter

RUN sed -i '1iusing FileParameter = Microsoft.AspNetCore.Http.IFormFile;' /output/Skedular/Slack/V1/Slack.g.cs
RUN /app/publish/Skedularctl mcp-tool-generate \
  --input-file /output/Skedular/Slack/V1/Slack.g.cs \
  --output-file /output/Skedular/Slack/V1/Slack.g.cs

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/skedular/team_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.Skedular.Team.V1 \
  /Classname:Team \
  /Output:/output/Skedular/Team/V1/Team.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson \
  /ExcludedTypeNames:FileParameter

RUN sed -i '1iusing FileParameter = Microsoft.AspNetCore.Http.IFormFile;' /output/Skedular/Team/V1/Team.g.cs
RUN /app/publish/Skedularctl mcp-tool-generate \
  --input-file /output/Skedular/Team/V1/Team.g.cs \
  --output-file /output/Skedular/Team/V1/Team.g.cs

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/skedular/core_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.Skedular.Core.V1 \
  /Classname:Core \
  /Output:/output/Skedular/Core/V1/Core.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true \
  /JsonLibrary:SystemTextJson \
  /ExcludedTypeNames:FileParameter

RUN sed -i '1iusing FileParameter = Microsoft.AspNetCore.Http.IFormFile;' /output/Skedular/Core/V1/Core.g.cs
RUN /app/publish/Skedularctl mcp-tool-generate \
  --input-file /output/Skedular/Core/V1/Core.g.cs \
  --output-file /output/Skedular/Core/V1/Core.g.cs

RUN find /output -type f -name "*.g.cs" -exec sed -i 's/Microsoft\.AspNetCore\.Mvc\.ActionResult<Microsoft\.AspNetCore\.Mvc\.FileResult>/Microsoft.AspNetCore.Mvc.IActionResult/g' {} +
