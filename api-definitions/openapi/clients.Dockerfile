# syntax = docker/dockerfile:1.12.0
ARG BUILD_IMAGE=mcr.microsoft.com/dotnet/sdk:10.0
ARG NSWAG_VERSION=latest

# Provides the .NET 8 shared runtimes
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet8

FROM $BUILD_IMAGE AS build
LABEL maintainer="morteza.alizadeh@gmail.com"

# Keep .NET 10 and add .NET 8 alongside it
COPY --from=dotnet8 /usr/share/dotnet/shared/Microsoft.NETCore.App/ \
  /usr/share/dotnet/shared/Microsoft.NETCore.App/
COPY --from=dotnet8 /usr/share/dotnet/shared/Microsoft.AspNetCore.App/ \
  /usr/share/dotnet/shared/Microsoft.AspNetCore.App/

RUN apt-get update -y && \
  apt-get install npm -y && \
  apt-get clean
RUN npm install -y -g "nswag@${NSWAG_VERSION}"

COPY ["api-definitions/openapi", "/openapi"]

RUN chmod +x /openapi/scripts/generate-clients.sh && \
  /openapi/scripts/generate-clients.sh