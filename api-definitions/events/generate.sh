#!/usr/bin/env bash

set -e
set -x

BASE_DIR="$(cd "$(dirname "${0}")/../.." && pwd)"

dotnet build "${BASE_DIR}/shared/Api.Shared.Clients/Api.Shared.Clients.csproj"
"${BASE_DIR}/scripts/generate-event-metadata.sh"
