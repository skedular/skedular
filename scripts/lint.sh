#!/usr/bin/env sh

set -e
set -x

cd "$(dirname "${0}")/.."

dotnet tool restore
dotnet jb inspectcode src/Skedular.slnx -o=analysis_output.json
