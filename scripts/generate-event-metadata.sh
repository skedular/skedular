#!/usr/bin/env bash

set -e
set -x

BASE_DIR="$(cd "$(dirname "${0}")/.." && pwd)"
SKEDULARCTL_PROJECT="${BASE_DIR}/shared/Skedularctl/Skedularctl.csproj"
SKEDULARCTL_OUTPUT_DIR="${BASE_DIR}/shared/Skedularctl/bin/Debug/net10.0"
SKEDULARCTL_DLL="${SKEDULARCTL_OUTPUT_DIR}/Skedularctl.dll"
EVENTS_DIR="${BASE_DIR}/api-definitions/events/skedular"
OUTPUT_DIR="${BASE_DIR}/shared/Api.Shared.Clients/Events/Skedular"

if [[ "${SKIP_BUILD:-false}" != "true" ]]; then
    dotnet build "${SKEDULARCTL_PROJECT}" --no-restore
fi

generate_metadata() {
    local namespace="$1"
    local event_type="$2"
    local topic_name="$3"
    local retry_topic_name_prefix="$4"
    local retry_topic_count="$5"
    local dead_letter_topic_name="$6"
    local output_file_path="$7"
    local generate_metadata_function_helper="${8:-True}"

    mkdir -p "$(dirname "${output_file_path}")"

    dotnet "${SKEDULARCTL_DLL}" protobuf-event-metadata-generate \
        --namespace "${namespace}" \
        --event-type "${event_type}" \
        --topic-name "${topic_name}" \
        --retry-topic-name-prefix "${retry_topic_name_prefix}" \
        --retry-topic-count "${retry_topic_count}" \
        --dead-letter-topic-name "${dead_letter_topic_name}" \
        --generate-metadata-function-helper "${generate_metadata_function_helper}" \
        --output-file-path "${output_file_path}"
}

generate_metadata \
    "Api.Shared.Clients.Events.Skedular.Booking.V1.Key" \
    "Key" \
    "booking.v1.event" \
    "booking.v1.event.retry" \
    "1" \
    "booking.v1.event.deadletter" \
    "${OUTPUT_DIR}/Booking/V1/BookingKeyMetadata.g.cs" \
    "False"
generate_metadata \
    "Api.Shared.Clients.Events.Skedular.Booking.V1.Value" \
    "Event" \
    "booking.v1.event" \
    "booking.v1.event.retry" \
    "1" \
    "booking.v1.event.deadletter" \
    "${OUTPUT_DIR}/Booking/V1/BookingValueMetadata.g.cs"

generate_metadata \
    "Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Key" \
    "Key" \
    "booking.v1.internal" \
    "booking.v1.internal.retry" \
    "1" \
    "booking.v1.internal.deadletter" \
    "${OUTPUT_DIR}/BookingInternal/V1/BookingInternalKeyMetadata.g.cs" \
    "False"
generate_metadata \
    "Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value" \
    "Event" \
    "booking.v1.internal" \
    "booking.v1.internal.retry" \
    "1" \
    "booking.v1.internal.deadletter" \
    "${OUTPUT_DIR}/BookingInternal/V1/BookingInternalValueMetadata.g.cs"

generate_metadata \
    "Api.Shared.Clients.Events.Skedular.Customer.V1.Key" \
    "Key" \
    "customer.v1.event" \
    "customer.v1.event.retry" \
    "1" \
    "customer.v1.event.deadletter" \
    "${OUTPUT_DIR}/Customer/V1/CustomerKeyMetadata.g.cs" \
    "False"
generate_metadata \
    "Api.Shared.Clients.Events.Skedular.Customer.V1.Value" \
    "Event" \
    "customer.v1.event" \
    "customer.v1.event.retry" \
    "1" \
    "customer.v1.event.deadletter" \
    "${OUTPUT_DIR}/Customer/V1/CustomerValueMetadata.g.cs"

generate_metadata \
    "Api.Shared.Clients.Events.Skedular.Location.V1.Key" \
    "Key" \
    "location.v1.event" \
    "location.v1.event.retry" \
    "1" \
    "location.v1.event.deadletter" \
    "${OUTPUT_DIR}/Location/V1/LocationKeyMetadata.g.cs" \
    "False"
generate_metadata \
    "Api.Shared.Clients.Events.Skedular.Location.V1.Value" \
    "Event" \
    "location.v1.event" \
    "location.v1.event.retry" \
    "1" \
    "location.v1.event.deadletter" \
    "${OUTPUT_DIR}/Location/V1/LocationValueMetadata.g.cs"

generate_metadata \
    "Api.Shared.Clients.Events.Skedular.Marketplace.V1.Key" \
    "Key" \
    "marketplace.v1.event" \
    "marketplace.v1.event.retry" \
    "1" \
    "marketplace.v1.event.deadletter" \
    "${OUTPUT_DIR}/Marketplace/V1/MarketplaceKeyMetadata.g.cs" \
    "False"
generate_metadata \
    "Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value" \
    "Event" \
    "marketplace.v1.event" \
    "marketplace.v1.event.retry" \
    "1" \
    "marketplace.v1.event.deadletter" \
    "${OUTPUT_DIR}/Marketplace/V1/MarketplaceValueMetadata.g.cs"

generate_metadata \
    "Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Key" \
    "Key" \
    "organization.v1.internal" \
    "organization.v1.internal.retry" \
    "1" \
    "organization.v1.internal.deadletter" \
    "${OUTPUT_DIR}/OrganizationInternal/V1/OrganizationInternalKeyMetadata.g.cs" \
    "False"
generate_metadata \
    "Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Value" \
    "Event" \
    "organization.v1.internal" \
    "organization.v1.internal.retry" \
    "1" \
    "organization.v1.internal.deadletter" \
    "${OUTPUT_DIR}/OrganizationInternal/V1/OrganizationInternalValueMetadata.g.cs"

generate_metadata \
    "Api.Shared.Clients.Events.Skedular.OrganizationMember.V1.Key" \
    "Key" \
    "organization.member.v1.event" \
    "organization.member.v1.event.retry" \
    "1" \
    "organization.member.v1.event.deadletter" \
    "${OUTPUT_DIR}/OrganizationMember/V1/OrganizationMemberKeyMetadata.g.cs" \
    "False"
generate_metadata \
    "Api.Shared.Clients.Events.Skedular.OrganizationMember.V1.Value" \
    "Event" \
    "organization.member.v1.event" \
    "organization.member.v1.event.retry" \
    "1" \
    "organization.member.v1.event.deadletter" \
    "${OUTPUT_DIR}/OrganizationMember/V1/OrganizationMemberValueMetadata.g.cs"

generate_metadata \
    "Api.Shared.Clients.Events.Skedular.Organization.V1.Key" \
    "Key" \
    "organization.v1.event" \
    "organization.v1.event.retry" \
    "1" \
    "organization.v1.event.deadletter" \
    "${OUTPUT_DIR}/Organization/V1/OrganizationKeyMetadata.g.cs" \
    "False"
generate_metadata \
    "Api.Shared.Clients.Events.Skedular.Organization.V1.Value" \
    "Event" \
    "organization.v1.event" \
    "organization.v1.event.retry" \
    "1" \
    "organization.v1.event.deadletter" \
    "${OUTPUT_DIR}/Organization/V1/OrganizationValueMetadata.g.cs"

generate_metadata \
    "Api.Shared.Clients.Events.Skedular.Team.V1.Key" \
    "Key" \
    "team.v1.event" \
    "team.v1.event.retry" \
    "1" \
    "team.v1.event.deadletter" \
    "${OUTPUT_DIR}/Team/V1/TeamKeyMetadata.g.cs" \
    "False"
generate_metadata \
    "Api.Shared.Clients.Events.Skedular.Team.V1.Value" \
    "Event" \
    "team.v1.event" \
    "team.v1.event.retry" \
    "1" \
    "team.v1.event.deadletter" \
    "${OUTPUT_DIR}/Team/V1/TeamValueMetadata.g.cs"
