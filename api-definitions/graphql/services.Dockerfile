# syntax = docker/dockerfile:1.7.0
ARG BUILD_IMAGE=mcr.microsoft.com/dotnet/sdk:8.0

FROM $BUILD_IMAGE AS build
LABEL maintainer="morteza.alizadeh@gmail.com"

COPY ["shared/Unityhubctl", "shared/Unityhubctl"]
WORKDIR shared/Unityhubctl
RUN --mount=type=cache,target=~/.nuget/packages dotnet restore "Unityhubctl.csproj"
RUN --mount=type=cache,target=~/.nuget/packages dotnet build "Unityhubctl.csproj" --no-restore -c Release -o /app/build
RUN --mount=type=cache,target=~/.nuget/packages dotnet publish "Unityhubctl.csproj" -c Release -o /app/publish

COPY ["api-definitions/graphql", "/graphql"]

RUN mkdir -p /output/UnityHub/V1/Billing
RUN /app/publish/Unityhubctl graphql-services-generate \
    --namespace Api.Shared.Services.GraphQL.UnityHub.V1.Billing \
    --input-schema-files-path /graphql/unityhub/billing_v1.graphql \
    --visitors AbstractClass,Argument,Class,Enum,Interface,Union \
    --output-file-path /output/UnityHub/V1/Billing/Billing.g.cs \
    --output-metadata-file-path /output/UnityHub/V1/Billing/BillingMetadata.g.cs

RUN mkdir -p /output/UnityHub/V1/Booking
RUN /app/publish/Unityhubctl graphql-services-generate \
    --namespace Api.Shared.Services.GraphQL.UnityHub.V1.Booking \
    --input-schema-files-path /graphql/unityhub/booking_v1.graphql \
    --visitors AbstractClass,Argument,Class,Enum,Interface,Union \
    --output-file-path /output/UnityHub/V1/Booking/Booking.g.cs \
    --output-metadata-file-path /output/UnityHub/V1/Booking/BookingMetadata.g.cs

RUN mkdir -p /output/UnityHub/V1/Customer
RUN /app/publish/Unityhubctl graphql-services-generate \
    --namespace Api.Shared.Services.GraphQL.UnityHub.V1.Customer \
    --input-schema-files-path /graphql/unityhub/customer_v1.graphql \
    --visitors AbstractClass,Argument,Class,Enum,Interface,Union \
    --output-file-path /output/UnityHub/V1/Customer/Customer.g.cs \
    --output-metadata-file-path /output/UnityHub/V1/Customer/CustomerMetadata.g.cs

RUN mkdir -p /output/UnityHub/V1/Location
RUN /app/publish/Unityhubctl graphql-services-generate \
    --namespace Api.Shared.Services.GraphQL.UnityHub.V1.Location \
    --input-schema-files-path /graphql/unityhub/location_v1.graphql \
    --visitors AbstractClass,Argument,Class,Enum,Interface,Union \
    --output-file-path /output/UnityHub/V1/Location/Location.g.cs \
    --output-metadata-file-path /output/UnityHub/V1/Location/LocationMetadata.g.cs

RUN mkdir -p /output/UnityHub/V1/MsTeams
RUN /app/publish/Unityhubctl graphql-services-generate \
    --namespace Api.Shared.Services.GraphQL.UnityHub.V1.MsTeams \
    --input-schema-files-path /graphql/unityhub/msteams_v1.graphql \
    --visitors AbstractClass,Argument,Class,Enum,Interface,Union \
    --output-file-path /output/UnityHub/V1/MsTeams/MsTeams.g.cs \
    --output-metadata-file-path /output/UnityHub/V1/MsTeams/MsTeamsMetadata.g.cs

RUN mkdir -p /output/UnityHub/V1/Notification
RUN /app/publish/Unityhubctl graphql-services-generate \
    --namespace Api.Shared.Services.GraphQL.UnityHub.V1.Notification \
    --input-schema-files-path /graphql/unityhub/notification_v1.graphql \
    --visitors AbstractClass,Argument,Class,Enum,Interface,Union \
    --output-file-path /output/UnityHub/V1/Notification/Notification.g.cs \
    --output-metadata-file-path /output/UnityHub/V1/Notification/NotificationMetadata.g.cs

RUN mkdir -p /output/UnityHub/V1/Organization
RUN /app/publish/Unityhubctl graphql-services-generate \
    --namespace Api.Shared.Services.GraphQL.UnityHub.V1.Organization \
    --input-schema-files-path /graphql/unityhub/organization_v1.graphql \
    --visitors AbstractClass,Argument,Class,Enum,Interface,Union \
    --output-file-path /output/UnityHub/V1/Organization/Organization.g.cs \
    --output-metadata-file-path /output/UnityHub/V1/Organization/OrganizationMetadata.g.cs

RUN mkdir -p /output/UnityHub/V1/Payment
RUN /app/publish/Unityhubctl graphql-services-generate \
    --namespace Api.Shared.Services.GraphQL.UnityHub.V1.Payment \
    --input-schema-files-path /graphql/unityhub/payment_v1.graphql \
    --visitors AbstractClass,Argument,Class,Enum,Interface,Union \
    --output-file-path /output/UnityHub/V1/Payment/Payment.g.cs \
    --output-metadata-file-path /output/UnityHub/V1/Payment/PaymentMetadata.g.cs

RUN mkdir -p /output/UnityHub/V1/Slack
RUN /app/publish/Unityhubctl graphql-services-generate \
    --namespace Api.Shared.Services.GraphQL.UnityHub.V1.Slack \
    --input-schema-files-path /graphql/unityhub/slack_v1.graphql \
    --visitors AbstractClass,Argument,Class,Enum,Interface,Union \
    --output-file-path /output/UnityHub/V1/Slack/Slack.g.cs \
    --output-metadata-file-path /output/UnityHub/V1/Slack/SlackMetadata.g.cs

RUN mkdir -p /output/UnityHub/V1/Team
RUN /app/publish/Unityhubctl graphql-services-generate \
    --namespace Api.Shared.Services.GraphQL.UnityHub.V1.Team \
    --input-schema-files-path /graphql/unityhub/team_v1.graphql \
    --visitors AbstractClass,Argument,Class,Enum,Interface,Union \
    --output-file-path /output/UnityHub/V1/Team/Team.g.cs \
    --output-metadata-file-path /output/UnityHub/V1/Team/TeamMetadata.g.cs
