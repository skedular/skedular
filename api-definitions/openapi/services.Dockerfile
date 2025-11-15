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

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/skedular/booking_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.Skedular.Booking.V1 \
  /Classname:Booking \
  /Output:/output/Skedular/Booking/V1/Booking.g.cs \
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

RUN sed -i '1iusing FileParameter = Microsoft.AspNetCore.Http.IFormFile;' /output/Skedular/Booking/V1/Booking.g.cs

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

RUN find /output -type f -name "*.g.cs" -exec sed -i 's/Microsoft\.AspNetCore\.Mvc\.ActionResult<Microsoft\.AspNetCore\.Mvc\.FileResult>/Microsoft.AspNetCore.Mvc.IActionResult/g' {} +
