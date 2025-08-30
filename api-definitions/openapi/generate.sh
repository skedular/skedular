#!/usr/bin/env sh

set -e
set -x

cd "$(dirname "${0}")"

cleanup() {
   docker rm skedular-extract-openapi-clients-generator || true
   docker rm skedular-extract-openapi-services-generator || true
}
trap cleanup EXIT

docker build --progress=plain -f clients.Dockerfile -t skedular-openapi-clients-generator ../../ &
docker build --progress=plain -f services.Dockerfile -t skedular-openapi-services-generator ../../ &
wait

mkdir -p ../../shared/Api.Shared.Clients/OpenApi
docker create --name skedular-extract-openapi-clients-generator skedular-openapi-clients-generator
docker cp skedular-extract-openapi-clients-generator:/output/. "../../shared/Api.Shared.Clients/OpenApi"

mkdir -p ../../shared/Api.Shared.Services/OpenApi
docker create --name skedular-extract-openapi-services-generator skedular-openapi-services-generator
docker cp skedular-extract-openapi-services-generator:/output/. "../../shared/Api.Shared.Services/OpenApi"
