#!/usr/bin/env bash

set -e
set -x

command=${@:-up -d --build}

cd "$(dirname "${0}")/.."

docker compose -p "skedular" \
    -f docker-compose-min.yml \
    $command
