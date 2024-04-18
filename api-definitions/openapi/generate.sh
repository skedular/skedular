#!/usr/bin/env sh

set -e
set -x

cd "$(dirname "${0}")"

cleanup() {
   docker rm extract-openapi-clients-generator || true
   docker rm extract-openapi-services-generator || true
}
trap cleanup EXIT

docker build --progress=plain -f clients.Dockerfile -t openapi-clients-generator ../../ &
docker build --progress=plain -f services.Dockerfile -t openapi-services-generator ../../ &
wait

mkdir -p ../../shared/Api.Shared.Clients/OpenApi
docker create --name extract-openapi-clients-generator openapi-clients-generator
docker cp extract-openapi-clients-generator:/output/. "../../shared/Api.Shared.Clients/OpenApi"

mkdir -p ../../shared/Api.Shared.Services/OpenApi
docker create --name extract-openapi-services-generator openapi-services-generator
docker cp extract-openapi-services-generator:/output/. "../../shared/Api.Shared.Services/OpenApi"
