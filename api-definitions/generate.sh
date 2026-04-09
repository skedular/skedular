#!/usr/bin/env sh

set -e
set -x

cd "$(dirname "${0}")"

./openapi/generate.sh
