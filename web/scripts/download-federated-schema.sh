#!/usr/bin/env sh

set -e
set -x

cd "$(dirname "${0}")/.."

mkdir -p schema/unityhub/v1
cd schema/unityhub/v1

if ! [ -x "$(command -v rover)" ]; then
  curl -sSL https://rover.apollo.dev/nix/latest | sh
fi

rover graph introspect http://localhost:9000/gateway/api/v1/graphql >schema.graphql
