#!/usr/bin/env bash

set -e
set -x

BASE_DIR="$(cd "$(dirname "${0}")/.." && pwd)"

dotnet tool restore

# Always start clean: remove stale Fusion pack/compose artifacts before regeneration.
find "${BASE_DIR}" -type f \( -name "*.fsp" -o -name "*.fgp" \) -delete

cd "${BASE_DIR}/booking/apis/Booking.Api"
dotnet run -- schema export --output schema.graphql
dotnet fusion subgraph pack

cd "${BASE_DIR}/core/apis/Core.Api"
dotnet run -- schema export --output schema.graphql
dotnet fusion subgraph pack

cd "${BASE_DIR}/customer/apis/Customer.Api"
dotnet run -- schema export --output schema.graphql
dotnet fusion subgraph pack

cd "${BASE_DIR}/location/apis/Location.Api"
dotnet run -- schema export --output schema.graphql
dotnet fusion subgraph pack

cd "${BASE_DIR}/marketplace/apis/Marketplace.Api"
dotnet run -- schema export --output schema.graphql
dotnet fusion subgraph pack

cd "${BASE_DIR}/msteams/apis/MsTeams.Api"
dotnet run -- schema export --output schema.graphql
dotnet fusion subgraph pack

cd "${BASE_DIR}/organization/apis/Organization.Api"
dotnet run -- schema export --output schema.graphql
dotnet fusion subgraph pack

cd "${BASE_DIR}/slack/apis/Slack.Api"
dotnet run -- schema export --output schema.graphql
dotnet fusion subgraph pack

cd "${BASE_DIR}/team/apis/Team.Api"
dotnet run -- schema export --output schema.graphql
dotnet fusion subgraph pack

cd "${BASE_DIR}/gateway/apis/Gateway"
dotnet fusion compose --enable-nodes -p gateway.fgp -s ../../../core/apis/Core.Api
dotnet fusion compose --enable-nodes -p gateway.fgp -s ../../../booking/apis/Booking.Api
dotnet fusion compose --enable-nodes -p gateway.fgp -s ../../../customer/apis/Customer.Api
dotnet fusion compose --enable-nodes -p gateway.fgp -s ../../../location/apis/Location.Api
dotnet fusion compose --enable-nodes -p gateway.fgp -s ../../../marketplace/apis/Marketplace.Api
dotnet fusion compose --enable-nodes -p gateway.fgp -s ../../../msteams/apis/MsTeams.Api
dotnet fusion compose --enable-nodes -p gateway.fgp -s ../../../organization/apis/Organization.Api
dotnet fusion compose --enable-nodes -p gateway.fgp -s ../../../slack/apis/Slack.Api
dotnet fusion compose --enable-nodes -p gateway.fgp -s ../../../team/apis/Team.Api
mkdir -p ../../../api-definitions/graphql/skedular/v1
dotnet run -- schema export --output ../../../api-definitions/graphql/skedular/v1/schema.graphql

################################################################################################################
cd "${BASE_DIR}/booking/domain/Booking.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/Booking.Api/schema.graphql
git checkout ./.graphqlrc.json

cd "${BASE_DIR}/core/domain/Core.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/Core.Api/schema.graphql
git checkout ./.graphqlrc.json

cd "${BASE_DIR}/customer/domain/Customer.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/Customer.Api/schema.graphql
git checkout ./.graphqlrc.json

cd "${BASE_DIR}/location/domain/Location.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/Location.Api/schema.graphql
git checkout ./.graphqlrc.json

cd "${BASE_DIR}/marketplace/domain/Marketplace.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/Marketplace.Api/schema.graphql
git checkout ./.graphqlrc.json

cd "${BASE_DIR}/msteams/domain/MsTeams.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/MsTeams.Api/schema.graphql
git checkout ./.graphqlrc.json

cd "${BASE_DIR}/organization/domain/Organization.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/Organization.Api/schema.graphql
git checkout ./.graphqlrc.json

cd "${BASE_DIR}/slack/domain/Slack.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/Slack.Api/schema.graphql
git checkout ./.graphqlrc.json

cd "${BASE_DIR}/team/domain/Team.Domain.IntegrationTests"
rm -f schema.graphql
dotnet graphql init -f ../../apis/Team.Api/schema.graphql
git checkout ./.graphqlrc.json

cd "${BASE_DIR}/system/Skedular.SystemTests"
rm -f schema.graphql
dotnet graphql init -f ../../api-definitions/graphql/skedular/v1/schema.graphql
git checkout ./.graphqlrc.json
