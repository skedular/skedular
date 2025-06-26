#!/usr/bin/env bash

set -e
set -x

BASE_DIR="$(cd "$(dirname "${0}")/.." && pwd)"

dotnet tool restore

cd "${BASE_DIR}/booking/apis/Booking.Api"
dotnet run -- schema export --output schema.graphql
dotnet fusion subgraph pack

cd "${BASE_DIR}/customer/apis/Customer.Api"
dotnet run -- schema export --output schema.graphql
dotnet fusion subgraph pack

cd "${BASE_DIR}/location/apis/Location.Api"
dotnet run -- schema export --output schema.graphql
dotnet fusion subgraph pack

cd "${BASE_DIR}/msteams/apis/MsTeams.Api"
dotnet run -- schema export --output schema.graphql
dotnet fusion subgraph pack

cd "${BASE_DIR}/marketplace/apis/Marketplace.Api"
dotnet run -- schema export --output schema.graphql
dotnet fusion subgraph pack

cd "${BASE_DIR}/notification/apis/Notification.Api"
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

cd "${BASE_DIR}/core/apis/Core.Api"
dotnet run -- schema export --output schema.graphql
dotnet fusion subgraph pack

cd "${BASE_DIR}/gateway/apis/Gateway"
dotnet fusion compose -p gateway.fgp -s ../../../booking/apis/Booking.Api
dotnet fusion compose -p gateway.fgp -s ../../../customer/apis/Customer.Api
dotnet fusion compose -p gateway.fgp -s ../../../location/apis/Location.Api
dotnet fusion compose -p gateway.fgp -s ../../../marketplace/apis/Marketplace.Api
dotnet fusion compose -p gateway.fgp -s ../../../msteams/apis/MsTeams.Api
dotnet fusion compose -p gateway.fgp -s ../../../notification/apis/Notification.Api
dotnet fusion compose -p gateway.fgp -s ../../../organization/apis/Organization.Api
dotnet fusion compose -p gateway.fgp -s ../../../slack/apis/Slack.Api
dotnet fusion compose -p gateway.fgp -s ../../../team/apis/Team.Api
dotnet fusion compose -p gateway.fgp -s ../../../core/apis/Core.Api
mkdir -p ../../../api-definitions/graphql/skedular/v1
dotnet run -- schema export --output ../../../api-definitions/graphql/skedular/v1/schema.graphql
