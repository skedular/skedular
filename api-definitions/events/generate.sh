#!/usr/bin/env sh

set -e
set -x

cd "$(dirname "${0}")"

cleanup() {
   docker rm extract-events-generator || true
}
trap cleanup EXIT

docker build --progress=plain -f Dockerfile -t events-generator ../../
docker create --name extract-events-generator events-generator

mkdir -p ../../shared/Api.Shared.Clients/Events
docker cp extract-events-generator:/output/. "../../shared/Api.Shared.Clients/Events"
