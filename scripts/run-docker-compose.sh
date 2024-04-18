#!/usr/bin/env bash

set -e
set -x

command=${@:-up -d --build}

cd "$(dirname "${0}")/.."

docker compose -p "unityhub" \
    --profile all \
    -f docker-compose.yml \
    --env-file .env \
    $command
