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
clients_build_pid=$!
docker build --progress=plain -f services.Dockerfile -t skedular-openapi-services-generator ../../ &
services_build_pid=$!

build_status=0
wait "$clients_build_pid" || build_status=1
wait "$services_build_pid" || build_status=1
if [ "$build_status" -ne 0 ]; then
   echo "OpenAPI generator image build failed; generated outputs were not copied." >&2
   exit "$build_status"
fi

mkdir -p ../../src/shared/Api.Shared.Clients/OpenApi
docker create --name skedular-extract-openapi-clients-generator skedular-openapi-clients-generator
docker cp skedular-extract-openapi-clients-generator:/output/. "../../src/shared/Api.Shared.Clients/OpenApi"

mkdir -p ../../src/shared/Api.Shared.Services/OpenApi
docker create --name skedular-extract-openapi-services-generator skedular-openapi-services-generator
docker cp skedular-extract-openapi-services-generator:/output/. "../../src/shared/Api.Shared.Services/OpenApi"
