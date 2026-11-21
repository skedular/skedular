#!/usr/bin/env sh

set -e
set -x

cd "$(dirname "${0}")/.."

dotnet format src/Skedular.slnx
terraform fmt -recursive
