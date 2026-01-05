#!/usr/bin/env sh

set -e
set -x

cd "$(dirname "${0}")/.."

dotnet tool restore
dotnet jb cleanupcode Skedular.slnx
