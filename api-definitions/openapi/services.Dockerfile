# syntax = docker/dockerfile:1.12.0
ARG BUILD_IMAGE=mcr.microsoft.com/dotnet/sdk:10.0
ARG NSWAG_VERSION=latest

FROM $BUILD_IMAGE AS build
LABEL maintainer="morteza.alizadeh@gmail.com"

RUN apt-get update -y && \
  apt-get install npm -y && \
  apt-get clean
RUN npm install -y -g "nswag@${NSWAG_VERSION}"

COPY [".git", "shared/.git"]
COPY ["shared/Api.Shared", "shared/Api.Shared"]
COPY ["shared/Enterprise.Shared", "shared/Enterprise.Shared"]
COPY ["shared/Skedularctl", "shared/Skedularctl"]

WORKDIR /shared/Skedularctl

RUN --mount=type=cache,target=~/.nuget/packages dotnet restore "Skedularctl.csproj"
RUN --mount=type=cache,target=~/.nuget/packages dotnet build "Skedularctl.csproj" --no-restore -c Release -o /app/build
RUN --mount=type=cache,target=~/.nuget/packages dotnet publish "Skedularctl.csproj" -c Release -o /app/publish

COPY ["api-definitions/openapi", "/openapi"]

RUN chmod +x /openapi/scripts/generate-services.sh && \
  /openapi/scripts/generate-services.sh
