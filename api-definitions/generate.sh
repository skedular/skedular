#!/usr/bin/env sh

set -e
set -x

cd "$(dirname "${0}")"

./events/generate.sh
./graphql/generate.sh
./openapi/generate.sh
