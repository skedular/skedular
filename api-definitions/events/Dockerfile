# syntax = docker/dockerfile:1.12.0
ARG BUILD_IMAGE=mcr.microsoft.com/dotnet/sdk:10.0

FROM $BUILD_IMAGE AS build
LABEL maintainer="morteza.alizadeh@gmail.com"

RUN apt-get update -y && \
    apt-get install unzip curl -y && \
    apt-get clean

ENV GIT_TAG="33.0"
ENV PROTOC_ZIP=protoc-$GIT_TAG-linux-x86_64.zip
RUN curl -OL https://github.com/google/protobuf/releases/download/v$GIT_TAG/$PROTOC_ZIP
RUN unzip -o $PROTOC_ZIP -d /usr/local bin/protoc
RUN unzip -o $PROTOC_ZIP -d /usr/local include/*
RUN rm -f $PROTOC_ZIP

COPY [".git", "shared/.git"]
COPY ["shared/Enterprise.Shared", "shared/Enterprise.Shared"]
COPY ["shared/Skedularctl", "shared/Skedularctl"]
WORKDIR shared/Skedularctl
RUN --mount=type=cache,target=~/.nuget/packages dotnet restore "Skedularctl.csproj"
RUN --mount=type=cache,target=~/.nuget/packages dotnet build "Skedularctl.csproj" --no-restore -c Release -o /app/build
RUN --mount=type=cache,target=~/.nuget/packages dotnet publish "Skedularctl.csproj" -c Release -o /app/publish

COPY ["api-definitions/events", "/events"]

RUN mkdir -p /output/Skedular/Booking/V1
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/Booking/V1 --csharp_opt=file_extension=.g.cs booking_v1_key.proto
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/Booking/V1 --csharp_opt=file_extension=.g.cs booking_v1_value.proto
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.Booking.V1.Key \
    --event-type Key \
    --topic-name booking.v1.event \
    --retry-topic-name-prefix booking.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name booking.v1.event.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/skedular/booking_v1_key.proto \
    --output-file-path /output/Skedular/Booking/BookingKeyMetadata.g.cs
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.Booking.V1.Value \
    --event-type Event \
    --topic-name booking.v1.event \
    --retry-topic-name-prefix booking.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name booking.v1.event.deadletter \
    --protobuf-schema-file-path /events/skedular/booking_v1_value.proto \
    --output-file-path /output/Skedular/Booking/BookingValueMetadata.g.cs

RUN mkdir -p /output/Skedular/BookingInternal/V1
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/BookingInternal/V1 --csharp_opt=file_extension=.g.cs booking_internal_v1_key.proto
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/BookingInternal/V1 --csharp_opt=file_extension=.g.cs booking_internal_v1_value.proto
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Key \
    --event-type Key \
    --topic-name booking.v1.internal \
    --retry-topic-name-prefix booking.v1.internal.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name booking.v1.internal.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/skedular/booking_internal_v1_key.proto \
    --output-file-path /output/Skedular/BookingInternal/V1/BookingInternalKeyMetadata.g.cs
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value \
    --event-type Event \
    --topic-name booking.v1.internal \
    --retry-topic-name-prefix booking.v1.internal.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name booking.v1.internal.deadletter \
    --protobuf-schema-file-path /events/skedular/booking_internal_v1_value.proto \
    --output-file-path /output/Skedular/BookingInternal/V1/BookingInternalValueMetadata.g.cs

RUN mkdir -p /output/Skedular/Customer/V1
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/Customer/V1 --csharp_opt=file_extension=.g.cs customer_v1_key.proto
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/Customer/V1 --csharp_opt=file_extension=.g.cs customer_v1_value.proto
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.Customer.V1.Key \
    --event-type Key \
    --topic-name customer.v1.event \
    --retry-topic-name-prefix customer.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name customer.v1.event.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/skedular/customer_v1_key.proto \
    --output-file-path /output/Skedular/Customer/V1/CustomerKeyMetadata.g.cs
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.Customer.V1.Value \
    --event-type Event \
    --topic-name customer.v1.event \
    --retry-topic-name-prefix customer.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name customer.v1.event.deadletter \
    --protobuf-schema-file-path /events/skedular/customer_v1_value.proto \
    --output-file-path /output/Skedular/Customer/V1/CustomerValueMetadata.g.cs

RUN mkdir -p /output/Skedular/Location/V1
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/Location/V1 --csharp_opt=file_extension=.g.cs location_v1_key.proto
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/Location/V1 --csharp_opt=file_extension=.g.cs location_v1_value.proto
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.Location.V1.Key \
    --event-type Key \
    --topic-name location.v1.event \
    --retry-topic-name-prefix location.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name location.v1.event.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/skedular/location_v1_key.proto \
    --output-file-path /output/Skedular/Location/V1/LocationKeyMetadata.g.cs
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.Location.V1.Value \
    --event-type Event \
    --topic-name location.v1.event \
    --retry-topic-name-prefix location.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name location.v1.event.deadletter \
    --protobuf-schema-file-path /events/skedular/location_v1_value.proto \
    --output-file-path /output/Skedular/Location/V1/LocationValueMetadata.g.cs

RUN mkdir -p /output/Skedular/Marketplace/V1
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/Marketplace/V1 --csharp_opt=file_extension=.g.cs marketplace_v1_key.proto
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/Marketplace/V1 --csharp_opt=file_extension=.g.cs marketplace_v1_value.proto
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.Marketplace.V1.Key \
    --event-type Key \
    --topic-name marketplace.v1.event \
    --retry-topic-name-prefix marketplace.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name marketplace.v1.event.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/skedular/marketplace_v1_key.proto \
    --output-file-path /output/Skedular/Marketplace/V1/MarketplaceKeyMetadata.g.cs
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value \
    --event-type Event \
    --topic-name marketplace.v1.event \
    --retry-topic-name-prefix marketplace.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name marketplace.v1.event.deadletter \
    --protobuf-schema-file-path /events/skedular/marketplace_v1_value.proto \
    --output-file-path /output/Skedular/Marketplace/V1/MarketplaceValueMetadata.g.cs

RUN mkdir -p /output/Skedular/OrganizationInternal/V1
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/OrganizationInternal/V1 --csharp_opt=file_extension=.g.cs organization_internal_v1_value.proto
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/OrganizationInternal/V1 --csharp_opt=file_extension=.g.cs organization_internal_v1_key.proto
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Key \
    --event-type Key \
    --topic-name organization.v1.internal \
    --retry-topic-name-prefix organization.v1.internal.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name organization.v1.internal.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/skedular/organization_internal_v1_key.proto \
    --output-file-path /output/Skedular/OrganizationInternal/V1/OrganizationInternalKeyMetadata.g.cs
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Value \
    --event-type Event \
    --topic-name organization.v1.internal \
    --retry-topic-name-prefix organization.v1.internal.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name organization.v1.internal.deadletter \
    --protobuf-schema-file-path /events/skedular/organization_internal_v1_value.proto \
    --output-file-path /output/Skedular/OrganizationInternal/V1/OrganizationInternalValueMetadata.g.cs

RUN mkdir -p /output/Skedular/OrganizationMember/V1
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/OrganizationMember/V1 --csharp_opt=file_extension=.g.cs organization_member_v1_key.proto
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/OrganizationMember/V1 --csharp_opt=file_extension=.g.cs organization_member_v1_value.proto
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.OrganizationMember.V1.Key \
    --event-type Key \
    --topic-name organization.member.v1.event \
    --retry-topic-name-prefix organization.member.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name organization.member.v1.event.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/skedular/organization_member_v1_key.proto \
    --output-file-path /output/Skedular/OrganizationMember/V1/OrganizationMemberKeyMetadata.g.cs
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.OrganizationMember.V1.Value \
    --event-type Event \
    --topic-name organization.member.v1.event \
    --retry-topic-name-prefix organization.member.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name organization.member.v1.event.deadletter \
    --protobuf-schema-file-path /events/skedular/organization_member_v1_value.proto \
    --output-file-path /output/Skedular/OrganizationMember/V1/OrganizationMemberValueMetadata.g.cs

RUN mkdir -p /output/Skedular/Organization/V1
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/Organization/V1 --csharp_opt=file_extension=.g.cs organization_v1_key.proto
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/Organization/V1 --csharp_opt=file_extension=.g.cs organization_v1_value.proto
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.Organization.V1.Key \
    --event-type Key \
    --topic-name organization.v1.event \
    --retry-topic-name-prefix organization.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name organization.v1.event.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/skedular/organization_v1_key.proto \
    --output-file-path /output/Skedular/Organization/V1/OrganizationKeyMetadata.g.cs
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.Organization.V1.Value \
    --event-type Event \
    --topic-name organization.v1.event \
    --retry-topic-name-prefix organization.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name organization.v1.event.deadletter \
    --protobuf-schema-file-path /events/skedular/organization_v1_value.proto \
    --output-file-path /output/Skedular/Organization/V1/OrganizationValueMetadata.g.cs

RUN mkdir -p /output/Skedular/Team/V1
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/Team/V1 --csharp_opt=file_extension=.g.cs team_v1_key.proto
RUN protoc -I=/events/skedular --csharp_out=/output/Skedular/Team/V1 --csharp_opt=file_extension=.g.cs team_v1_value.proto
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.Team.V1.Key \
    --event-type Key \
    --topic-name team.v1.event \
    --retry-topic-name-prefix team.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name team.v1.event.deadletter \
    --generate-metadata-function-helper False \
    --protobuf-schema-file-path /events/skedular/team_v1_key.proto \
    --output-file-path /output/Skedular/Team/V1/TeamKeyMetadata.g.cs
RUN /app/publish/Skedularctl protobuf-event-metadata-generate \
    --namespace Api.Shared.Clients.Events.Skedular.Team.V1.Value \
    --event-type Event \
    --topic-name team.v1.event \
    --retry-topic-name-prefix team.v1.event.retry \
    --retry-topic-count 1 \
    --dead-letter-topic-name team.v1.event.deadletter \
    --protobuf-schema-file-path /events/skedular/team_v1_value.proto \
    --output-file-path /output/Skedular/Team/V1/TeamValueMetadata.g.cs 
