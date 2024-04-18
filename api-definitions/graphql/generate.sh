#!/usr/bin/env sh

set -e
set -x

cd "$(dirname "${0}")"

cleanup() {
   docker rm extract-graphql-services-generator || true
}
trap cleanup EXIT

docker build --progress=plain -f services.Dockerfile -t graphql-services-generator ../../
docker create --name extract-graphql-services-generator graphql-services-generator

mkdir -p ../../shared/Api.Shared.Services/GraphQL
docker cp extract-graphql-services-generator:/output/. "../../shared/Api.Shared.Services/GraphQL"
