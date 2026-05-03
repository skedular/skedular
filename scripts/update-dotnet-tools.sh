#!/usr/bin/env bash

set -euo pipefail
set -x

BASE_DIR="$(cd "$(dirname "${0}")/.." && pwd)"

UNIT_TEST_TOOL_DIRS=(
  "booking/apis/Booking.Api.UnitTests"
  "booking/jobs/Booking.Jobs.UnitTests"
  "booking/processors/Booking.Processors.UnitTests"
  "booking/shared/Booking.Shared.UnitTests"
  "core/apis/Core.Api.UnitTests"
  "core/jobs/Core.Jobs.UnitTests"
  "core/processors/Core.Processors.UnitTests"
  "core/shared/Core.Shared.UnitTests"
  "customer/apis/Customer.Api.UnitTests"
  "customer/jobs/Customer.Jobs.UnitTests"
  "customer/processors/Customer.Processors.UnitTests"
  "customer/shared/Customer.Shared.UnitTests"
  "location/apis/Location.Api.UnitTests"
  "location/jobs/Location.Jobs.UnitTests"
  "location/processors/Location.Processors.UnitTests"
  "location/shared/Location.Shared.UnitTests"
  "marketplace/apis/Marketplace.Api.UnitTests"
  "marketplace/jobs/Marketplace.Jobs.UnitTests"
  "marketplace/processors/Marketplace.Processors.UnitTests"
  "marketplace/shared/Marketplace.Shared.UnitTests"
  "msteams/apis/MsTeams.Api.UnitTests"
  "msteams/jobs/MsTeams.Jobs.UnitTests"
  "msteams/processors/MsTeams.Processors.UnitTests"
  "msteams/shared/MsTeams.Shared.UnitTests"
  "organization/apis/Organization.Api.UnitTests"
  "organization/jobs/Organization.Jobs.UnitTests"
  "organization/processors/Organization.Processors.UnitTests"
  "organization/shared/Organization.Shared.UnitTests"
  "shared/Api.Shared.Clients.UnitTests"
  "shared/Api.Shared.Services.UnitTests"
  "shared/Enterprise.Shared.UnitTests"
  "slack/apis/Slack.Api.UnitTests"
  "slack/jobs/Slack.Jobs.UnitTests"
  "slack/processors/Slack.Processors.UnitTests"
  "slack/shared/Slack.Shared.UnitTests"
  "team/apis/Team.Api.UnitTests"
  "team/jobs/Team.Jobs.UnitTests"
  "team/processors/Team.Processors.UnitTests"
  "team/shared/Team.Shared.UnitTests"
)

cd "$BASE_DIR"

dotnet tool update jetbrains.resharper.globaltools
dotnet tool update dotnet-ef
dotnet tool update dotnet-outdated-tool
dotnet tool update chillicream.nitro.commandline
dotnet tool update strawberryshake.tools

for unit_test_tool_dir in "${UNIT_TEST_TOOL_DIRS[@]}"; do
  pushd "$BASE_DIR/$unit_test_tool_dir" >/dev/null
  dotnet tool update dotnet-stryker
  popd >/dev/null
done
