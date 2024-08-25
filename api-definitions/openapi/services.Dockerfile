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
  openapi2cscontroller \
  /Input:/openapi/unityhub/gateway_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.UnityHub.Gateway.V1 \
  /Classname:Gateway \
  /Output:/output/UnityHub/Gateway/V1/Gateway.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/unityhub/billing_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.UnityHub.Billing.V1 \
  /Classname:Billing \
  /Output:/output/UnityHub/Billing/V1/Billing.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/unityhub/booking_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.UnityHub.Booking.V1 \
  /Classname:Booking \
  /Output:/output/UnityHub/Booking/V1/Booking.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/unityhub/customer_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.UnityHub.Customer.V1 \
  /Classname:Customer \
  /Output:/output/UnityHub/Customer/V1/Customer.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/unityhub/location_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.UnityHub.Location.V1 \
  /Classname:Location \
  /Output:/output/UnityHub/Location/V1/Location.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/unityhub/msteams_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.UnityHub.MsTeams.V1 \
  /Classname:MsTeams \
  /Output:/output/UnityHub/MsTeams/V1/MsTeams.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/unityhub/notification_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.UnityHub.Notification.V1 \
  /Classname:Notification \
  /Output:/output/UnityHub/Notification/V1/Notification.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/unityhub/organization_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.UnityHub.Organization.V1 \
  /Classname:Organization \
  /Output:/output/UnityHub/Organization/V1/Organization.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/unityhub/payment_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.UnityHub.Payment.V1 \
  /Classname:Payment \
  /Output:/output/UnityHub/Payment/V1/Payment.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/unityhub/slack_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.UnityHub.Slack.V1 \
  /Classname:Slack \
  /Output:/output/UnityHub/Slack/V1/Slack.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true

RUN nswag \
  openapi2cscontroller \
  /Input:/openapi/unityhub/team_v1.yaml \
  /Namespace:Api.Shared.Services.OpenApi.UnityHub.Team.V1 \
  /Classname:Team \
  /Output:/output/UnityHub/Team/V1/Team.g.cs \
  /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
  /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
  /ControllerStyle:abstract \
  /HandleReferences:true \
  /ArrayType:System.Collections.Generic.IList \
  /DictionaryType:System.Collections.Generic.IDictionary \
  /UseActionResultType:true \
  /UseCancellationToken:true \
  /GenerateNullableReferenceTypes:true
