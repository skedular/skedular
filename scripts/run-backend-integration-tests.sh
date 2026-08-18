#!/usr/bin/env bash

set -euo pipefail
set -x

BASE_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJECT_COUNT=0

while IFS= read -r -d '' project; do
    PROJECT_COUNT=$((PROJECT_COUNT + 1))
    relative_project="${project#"${BASE_DIR}/"}"
    echo "Testing ${relative_project}"

    dotnet test \
        --project "${project}" \
        -- \
        --filter-trait "Category=Integration"
done < <(find "${BASE_DIR}/src" -name '*IntegrationTests.csproj' -print0 | sort -z)

if [ "${PROJECT_COUNT}" -eq 0 ]; then
    echo "No backend integration-test projects found." >&2
    exit 1
fi

echo "Completed ${PROJECT_COUNT} backend integration-test projects."
