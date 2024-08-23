# syntax = docker/dockerfile:1.7.0
ARG BUILD_IMAGE=mcr.microsoft.com/dotnet/sdk:8.0

FROM $BUILD_IMAGE AS build
LABEL maintainer="morteza.alizadeh@gmail.com"

RUN apt-get update -y && \
    apt-get install unzip curl -y && \
    apt-get clean

ENV GIT_TAG="27.3"
ENV PROTOC_ZIP=protoc-$GIT_TAG-linux-x86_64.zip
RUN curl -OL https://github.com/google/protobuf/releases/download/v$GIT_TAG/$PROTOC_ZIP
RUN unzip -o $PROTOC_ZIP -d /usr/local bin/protoc
RUN unzip -o $PROTOC_ZIP -d /usr/local include/*
RUN rm -f $PROTOC_ZIP

COPY ["shared/Unityhubctl", "shared/Unityhubctl"]
WORKDIR shared/Unityhubctl
RUN --mount=type=cache,target=~/.nuget/packages dotnet restore "Unityhubctl.csproj"
RUN --mount=type=cache,target=~/.nuget/packages dotnet build "Unityhubctl.csproj" --no-restore -c Release -o /app/build
RUN --mount=type=cache,target=~/.nuget/packages dotnet publish "Unityhubctl.csproj" -c Release -o /app/publish

COPY ["api-definitions/events", "/events"]

RUN mkdir -p /output/UnityHub/BillingInternal/V1
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/BillingInternal/V1 --csharp_opt=file_extension=.g.cs billing_internal_v1_key.proto
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/BillingInternal/V1 --csharp_opt=file_extension=.g.cs billing_internal_v1_value.proto
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.BillingInternal.V1.Key \
    --event-type Key \
    --topic-name billing.v1.internal \
    --retry-topic-name-prefix billing.v1.internal.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name billing.v1.internal.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/unityhub/billing_internal_v1_key.proto \
    --output-file-path /output/UnityHub/BillingInternal/V1/BillingInternalKeyMetadata.g.cs
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.BillingInternal.V1.Value \
    --event-type Event \
    --topic-name billing.v1.internal \
    --retry-topic-name-prefix billing.v1.internal.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name billing.v1.internal.deadletter \
    --protobuf-schema-file-path /events/unityhub/billing_internal_v1_value.proto \
    --output-file-path /output/UnityHub/BillingInternal/V1/BillingInternalValueMetadata.g.cs

RUN mkdir -p /output/UnityHub/Billing/V1
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/Billing/V1 --csharp_opt=file_extension=.g.cs billing_v1_key.proto
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/Billing/V1 --csharp_opt=file_extension=.g.cs billing_v1_value.proto
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.Billing.V1.Key \
    --event-type Key \
    --topic-name billing.v1.event \
    --retry-topic-name-prefix billing.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name billing.v1.event.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/unityhub/billing_v1_key.proto \
    --output-file-path /output/UnityHub/Billing/V1/BillingKeyMetadata.g.cs
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.Billing.V1.Value \
    --event-type Event \
    --topic-name billing.v1.event \
    --retry-topic-name-prefix billing.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name billing.v1.event.deadletter \
    --protobuf-schema-file-path /events/unityhub/billing_v1_value.proto \
    --output-file-path /output/UnityHub/Billing/V1/BillingValueMetadata.g.cs

RUN mkdir -p /output/UnityHub/Booking/V1
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/Booking/V1 --csharp_opt=file_extension=.g.cs booking_v1_key.proto
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/Booking/V1 --csharp_opt=file_extension=.g.cs booking_v1_value.proto
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.Booking.V1.Key \
    --event-type Key \
    --topic-name booking.v1.event \
    --retry-topic-name-prefix booking.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name booking.v1.event.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/unityhub/booking_v1_key.proto \
    --output-file-path /output/UnityHub/Booking/BookingKeyMetadata.g.cs
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.Booking.V1.Value \
    --event-type Event \
    --topic-name booking.v1.event \
    --retry-topic-name-prefix booking.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name booking.v1.event.deadletter \
    --protobuf-schema-file-path /events/unityhub/booking_v1_value.proto \
    --output-file-path /output/UnityHub/Booking/BookingValueMetadata.g.cs

RUN mkdir -p /output/UnityHub/Customer/V1
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/Customer/V1 --csharp_opt=file_extension=.g.cs customer_v1_key.proto
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/Customer/V1 --csharp_opt=file_extension=.g.cs customer_v1_value.proto
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.Customer.V1.Key \
    --event-type Key \
    --topic-name customer.v1.event \
    --retry-topic-name-prefix customer.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name customer.v1.event.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/unityhub/customer_v1_key.proto \
    --output-file-path /output/UnityHub/Customer/V1/CustomerKeyMetadata.g.cs
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.Customer.V1.Value \
    --event-type Event \
    --topic-name customer.v1.event \
    --retry-topic-name-prefix customer.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name customer.v1.event.deadletter \
    --protobuf-schema-file-path /events/unityhub/customer_v1_value.proto \
    --output-file-path /output/UnityHub/Customer/V1/CustomerValueMetadata.g.cs

RUN mkdir -p /output/UnityHub/LocationInternal/V1
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/LocationInternal/V1 --csharp_opt=file_extension=.g.cs location_internal_v1_key.proto
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/LocationInternal/V1 --csharp_opt=file_extension=.g.cs location_internal_v1_value.proto
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.LocationInternal.V1.Key \
    --event-type Key \
    --topic-name location.v1.internal \
    --retry-topic-name-prefix location.v1.internal.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name location.v1.internal.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/unityhub/location_internal_v1_key.proto \
    --output-file-path /output/UnityHub/LocationInternal/V1/LocationInternalKeyMetadata.g.cs
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.LocationInternal.V1.Value \
    --event-type Event \
    --topic-name location.v1.internal \
    --retry-topic-name-prefix location.v1.internal.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name location.v1.internal.deadletter \
    --protobuf-schema-file-path /events/unityhub/location_internal_v1_value.proto \
    --output-file-path /output/UnityHub/LocationInternal/V1/LocationInternalValueMetadata.g.cs

RUN mkdir -p /output/UnityHub/Location/V1
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/Location/V1 --csharp_opt=file_extension=.g.cs location_v1_key.proto
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/Location/V1 --csharp_opt=file_extension=.g.cs location_v1_value.proto
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.Location.V1.Key \
    --event-type Key \
    --topic-name location.v1.event \
    --retry-topic-name-prefix location.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name location.v1.event.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/unityhub/location_v1_key.proto \
    --output-file-path /output/UnityHub/Location/V1/LocationKeyMetadata.g.cs
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.Location.V1.Value \
    --event-type Event \
    --topic-name location.v1.event \
    --retry-topic-name-prefix location.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name location.v1.event.deadletter \
    --protobuf-schema-file-path /events/unityhub/location_v1_value.proto \
    --output-file-path /output/UnityHub/Location/V1/LocationValueMetadata.g.cs

RUN mkdir -p /output/UnityHub/Notification/V1
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/Notification/V1 --csharp_opt=file_extension=.g.cs notification_v1_key.proto
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/Notification/V1 --csharp_opt=file_extension=.g.cs notification_v1_value.proto
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.Notification.V1.Key \
    --event-type Key \
    --topic-name notification.v1.event \
    --retry-topic-name-prefix notification.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name notification.v1.event.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/unityhub/notification_v1_key.proto \
    --output-file-path /output/UnityHub/Notification/V1/NotificationKeyMetadata.g.cs
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.Notification.V1.Value \
    --event-type Event \
    --topic-name notification.v1.event \
    --retry-topic-name-prefix notification.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name notification.v1.event.deadletter \
    --protobuf-schema-file-path /events/unityhub/notification_v1_value.proto \
    --output-file-path /output/UnityHub/Notification/V1/NotificationValueMetadata.g.cs

RUN mkdir -p /output/UnityHub/OrganizationInternal/V1
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/OrganizationInternal/V1 --csharp_opt=file_extension=.g.cs organization_internal_v1_value.proto
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/OrganizationInternal/V1 --csharp_opt=file_extension=.g.cs organization_internal_v1_key.proto
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.OrganizationInternal.V1.Key \
    --event-type Key \
    --topic-name organization.v1.internal \
    --retry-topic-name-prefix organization.v1.internal.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name organization.v1.internal.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/unityhub/organization_internal_v1_key.proto \
    --output-file-path /output/UnityHub/OrganizationInternal/V1/OrganizationInternalKeyMetadata.g.cs
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.OrganizationInternal.V1.Value \
    --event-type Event \
    --topic-name organization.v1.internal \
    --retry-topic-name-prefix organization.v1.internal.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name organization.v1.internal.deadletter \
    --protobuf-schema-file-path /events/unityhub/organization_internal_v1_value.proto \
    --output-file-path /output/UnityHub/OrganizationInternal/V1/OrganizationInternalValueMetadata.g.cs

RUN mkdir -p /output/UnityHub/OrganizationMember/V1
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/OrganizationMember/V1 --csharp_opt=file_extension=.g.cs organization_member_v1_key.proto
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/OrganizationMember/V1 --csharp_opt=file_extension=.g.cs organization_member_v1_value.proto
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.OrganizationMember.V1.Key \
    --event-type Key \
    --topic-name organization.member.v1.event \
    --retry-topic-name-prefix organization.member.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name organization.member.v1.event.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/unityhub/organization_member_v1_key.proto \
    --output-file-path /output/UnityHub/OrganizationMember/V1/OrganizationMemberKeyMetadata.g.cs
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.OrganizationMember.V1.Value \
    --event-type Event \
    --topic-name organization.member.v1.event \
    --retry-topic-name-prefix organization.member.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name organization.member.v1.event.deadletter \
    --protobuf-schema-file-path /events/unityhub/organization_member_v1_value.proto \
    --output-file-path /output/UnityHub/OrganizationMember/V1/OrganizationMemberValueMetadata.g.cs

RUN mkdir -p /output/UnityHub/Organization/V1
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/Organization/V1 --csharp_opt=file_extension=.g.cs organization_v1_key.proto
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/Organization/V1 --csharp_opt=file_extension=.g.cs organization_v1_value.proto
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.Organization.V1.Key \
    --event-type Key \
    --topic-name organization.v1.event \
    --retry-topic-name-prefix organization.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name organization.v1.event.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/unityhub/organization_v1_key.proto \
    --output-file-path /output/UnityHub/Organization/V1/OrganizationKeyMetadata.g.cs
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.Organization.V1.Value \
    --event-type Event \
    --topic-name organization.v1.event \
    --retry-topic-name-prefix organization.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name organization.v1.event.deadletter \
    --protobuf-schema-file-path /events/unityhub/organization_v1_value.proto \
    --output-file-path /output/UnityHub/Organization/V1/OrganizationValueMetadata.g.cs

RUN mkdir -p /output/UnityHub/Payment/V1
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/Payment/V1 --csharp_opt=file_extension=.g.cs payment_v1_key.proto
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/Payment/V1 --csharp_opt=file_extension=.g.cs payment_v1_value.proto
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.Payment.V1.Key \
    --event-type Key \
    --topic-name payment.v1.event \
    --retry-topic-name-prefix payment.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name payment.v1.event.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/unityhub/payment_v1_key.proto \
    --output-file-path /output/UnityHub/Payment/V1/PaymentKeyMetadata.g.cs
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.Payment.V1.Value \
    --event-type Event \
    --topic-name payment.v1.event \
    --retry-topic-name-prefix payment.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name payment.v1.event.deadletter \
    --protobuf-schema-file-path /events/unityhub/payment_v1_value.proto \
    --output-file-path /output/UnityHub/Payment/V1/PaymentValueMetadata.g.cs

RUN mkdir -p /output/UnityHub/SlackInternal/V1
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/SlackInternal/V1 --csharp_opt=file_extension=.g.cs slack_internal_v1_key.proto
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/SlackInternal/V1 --csharp_opt=file_extension=.g.cs slack_internal_v1_value.proto
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.SlackInternal.V1.Key \
    --event-type Key \
    --topic-name slack.v1.internal \
    --retry-topic-name-prefix slack.v1.internal.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name slack.v1.internal.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/unityhub/slack_internal_v1_key.proto \
    --output-file-path /output/UnityHub/SlackInternal/V1/SlackInternalKeyMetadata.g.cs
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.SlackInternal.V1.Value \
    --event-type Event \
    --topic-name slack.v1.internal \
    --retry-topic-name-prefix slack.v1.internal.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name slack.v1.internal.deadletter \
    --protobuf-schema-file-path /events/unityhub/slack_internal_v1_value.proto \
    --output-file-path /output/UnityHub/SlackInternal/V1/SlackInternalValueMetadata.g.cs

RUN mkdir -p /output/UnityHub/Team/V1
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/Team/V1 --csharp_opt=file_extension=.g.cs team_v1_key.proto
RUN protoc -I=/events/unityhub --csharp_out=/output/UnityHub/Team/V1 --csharp_opt=file_extension=.g.cs team_v1_value.proto
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.Team.V1.Key \
    --event-type Key \
    --topic-name team.v1.event \
    --retry-topic-name-prefix team.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name team.v1.event.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/unityhub/team_v1_key.proto \
    --output-file-path /output/UnityHub/Team/V1/TeamKeyMetadata.g.cs
RUN /app/publish/Unityhubctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.UnityHub.Team.V1.Value \
    --event-type Event \
    --topic-name team.v1.event \
    --retry-topic-name-prefix team.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name team.v1.event.deadletter \
    --protobuf-schema-file-path /events/unityhub/team_v1_value.proto \
    --output-file-path /output/UnityHub/Team/V1/TeamValueMetadata.g.cs 
