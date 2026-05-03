#!/usr/bin/env bash

set -e
set -x

BASE_DIR="$(cd "$(dirname "${0}")/.." && pwd)"

dotnet tool restore

# Always clean fusion package artifacts to ensure fresh generation.
# Note: schema-settings.json is intentionally preserved — it carries the
# subgraph name + clientName that the gateway uses for runtime URL routing.
find "${BASE_DIR}" -type f -name "*.fsp" -o -name "*.fgp" | xargs rm -f 2>/dev/null || true
find "${BASE_DIR}" -type f -name "schema.graphqls" \
    \( -path "*/apis/*" \) \
    -delete 2>/dev/null || true
rm -f "${BASE_DIR}/gateway/apis/Gateway/gateway.far"

# Export source schema (.graphqls + schema-settings.json) for Fusion v2 composition.
# `dotnet run -- schema export` overwrites schema-settings.json, so back it up
# to preserve the checked-in `clientName` used for runtime URL routing.
export_schema() {
    local dir="$1"
    local settings_path="${dir}/schema-settings.json"
    local settings_backup=""
    if [ -f "${settings_path}" ]; then
        settings_backup=$(cat "${settings_path}")
    fi

    cd "${dir}"
    dotnet run -- schema export

    if [ -n "${settings_backup}" ]; then
        echo "${settings_backup}" > "${settings_path}"
    fi
}

export_schema "${BASE_DIR}/booking/apis/Booking.Api"
export_schema "${BASE_DIR}/core/apis/Core.Api"
export_schema "${BASE_DIR}/customer/apis/Customer.Api"
export_schema "${BASE_DIR}/location/apis/Location.Api"
export_schema "${BASE_DIR}/marketplace/apis/Marketplace.Api"
export_schema "${BASE_DIR}/msteams/apis/MsTeams.Api"
export_schema "${BASE_DIR}/organization/apis/Organization.Api"
export_schema "${BASE_DIR}/slack/apis/Slack.Api"
export_schema "${BASE_DIR}/team/apis/Team.Api"

cd "${BASE_DIR}/gateway/apis/Gateway"
dotnet nitro fusion compose \
    -f ../../../booking/apis/Booking.Api/schema.graphqls \
    -f ../../../core/apis/Core.Api/schema.graphqls \
    -f ../../../customer/apis/Customer.Api/schema.graphqls \
    -f ../../../location/apis/Location.Api/schema.graphqls \
    -f ../../../marketplace/apis/Marketplace.Api/schema.graphqls \
    -f ../../../msteams/apis/MsTeams.Api/schema.graphqls \
    -f ../../../organization/apis/Organization.Api/schema.graphqls \
    -f ../../../slack/apis/Slack.Api/schema.graphqls \
    -f ../../../team/apis/Team.Api/schema.graphqls \
    -a gateway.far

mkdir -p "${BASE_DIR}/api-definitions/graphql/skedular/v1"
GATEWAY_FAR="${BASE_DIR}/gateway/apis/Gateway/gateway.far"
GATEWAY_SCHEMA_ENTRY=$(unzip -Z1 "${GATEWAY_FAR}" | grep "gateway/.*/gateway.graphqls")
unzip -p "${GATEWAY_FAR}" "${GATEWAY_SCHEMA_ENTRY}" > "${BASE_DIR}/api-definitions/graphql/skedular/v1/schema.graphql"

################################################################################################################
cd "${BASE_DIR}/booking/domain/Booking.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/Booking.Api/schema.graphqls
git checkout -- ./.graphqlrc.json

cd "${BASE_DIR}/core/domain/Core.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/Core.Api/schema.graphqls
git checkout -- ./.graphqlrc.json

cd "${BASE_DIR}/customer/domain/Customer.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/Customer.Api/schema.graphqls
git checkout -- ./.graphqlrc.json

cd "${BASE_DIR}/location/domain/Location.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/Location.Api/schema.graphqls
git checkout -- ./.graphqlrc.json

cd "${BASE_DIR}/marketplace/domain/Marketplace.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/Marketplace.Api/schema.graphqls
git checkout -- ./.graphqlrc.json

cd "${BASE_DIR}/msteams/domain/MsTeams.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/MsTeams.Api/schema.graphqls
git checkout -- ./.graphqlrc.json

cd "${BASE_DIR}/organization/domain/Organization.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/Organization.Api/schema.graphqls
git checkout -- ./.graphqlrc.json

cd "${BASE_DIR}/slack/domain/Slack.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/Slack.Api/schema.graphqls
git checkout -- ./.graphqlrc.json

cd "${BASE_DIR}/team/domain/Team.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/Team.Api/schema.graphqls
git checkout -- ./.graphqlrc.json

cd "${BASE_DIR}/system/Skedular.SystemTests"
rm -f schema.graphql
dotnet graphql init -f ../../api-definitions/graphql/skedular/v1/schema.graphql
git checkout -- ./.graphqlrc.json
