#!/usr/bin/env bash

set -euo pipefail
set -x

BASE_DIR="$(cd "$(dirname "${0}")/.." && pwd)"

UNIT_TEST_TOOL_DIRS=(
  "src/booking/apis/Booking.Api.UnitTests"
  "src/booking/jobs/Booking.Jobs.UnitTests"
  "src/booking/processors/Booking.Processors.UnitTests"
  "src/booking/shared/Booking.Shared.UnitTests"
  "src/core/apis/Core.Api.UnitTests"
  "src/core/jobs/Core.Jobs.UnitTests"
  "src/core/processors/Core.Processors.UnitTests"
  "src/core/shared/Core.Shared.UnitTests"
  "src/customer/apis/Customer.Api.UnitTests"
  "src/customer/jobs/Customer.Jobs.UnitTests"
  "src/customer/processors/Customer.Processors.UnitTests"
  "src/customer/shared/Customer.Shared.UnitTests"
  "src/location/apis/Location.Api.UnitTests"
  "src/location/jobs/Location.Jobs.UnitTests"
  "src/location/processors/Location.Processors.UnitTests"
  "src/location/shared/Location.Shared.UnitTests"
  "src/marketplace/apis/Marketplace.Api.UnitTests"
  "src/marketplace/jobs/Marketplace.Jobs.UnitTests"
  "src/marketplace/processors/Marketplace.Processors.UnitTests"
  "src/marketplace/shared/Marketplace.Shared.UnitTests"
  "src/msteams/apis/MsTeams.Api.UnitTests"
  "src/msteams/jobs/MsTeams.Jobs.UnitTests"
  "src/msteams/processors/MsTeams.Processors.UnitTests"
  "src/msteams/shared/MsTeams.Shared.UnitTests"
  "src/organization/apis/Organization.Api.UnitTests"
  "src/organization/jobs/Organization.Jobs.UnitTests"
  "src/organization/processors/Organization.Processors.UnitTests"
  "src/organization/shared/Organization.Shared.UnitTests"
  "src/shared/Api.Shared.Clients.UnitTests"
  "src/shared/Api.Shared.Services.UnitTests"
  "src/shared/Enterprise.Shared.UnitTests"
  "src/slack/apis/Slack.Api.UnitTests"
  "src/slack/jobs/Slack.Jobs.UnitTests"
  "src/slack/processors/Slack.Processors.UnitTests"
  "src/slack/shared/Slack.Shared.UnitTests"
  "src/team/apis/Team.Api.UnitTests"
  "src/team/jobs/Team.Jobs.UnitTests"
  "src/team/processors/Team.Processors.UnitTests"
  "src/team/shared/Team.Shared.UnitTests"
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
