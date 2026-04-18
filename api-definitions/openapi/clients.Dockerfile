# syntax = docker/dockerfile:1.12.0
ARG BUILD_IMAGE=mcr.microsoft.com/dotnet/sdk:10.0
ARG NSWAG_VERSION=latest

FROM $BUILD_IMAGE AS build
LABEL maintainer="morteza.alizadeh@gmail.com"

RUN apt-get update -y && \
  apt-get install npm -y && \
  apt-get clean
RUN npm install -y -g "nswag@${NSWAG_VERSION}"

COPY ["api-definitions/openapi", "/openapi"]

ENV DOTNET_ROLL_FORWARD=LatestMajor

RUN chmod +x /openapi/scripts/generate-clients.sh && \
  /openapi/scripts/generate-clients.sh
