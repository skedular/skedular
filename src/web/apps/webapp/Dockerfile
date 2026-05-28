# syntax = docker/dockerfile:1.12.0
ARG FINAL_IMAGE=node:22-alpine
ARG BUILD_IMAGE=node:22-alpine

FROM $BUILD_IMAGE AS build
ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"
RUN corepack enable

RUN mkdir -p /src/web
COPY src/web /src/web
WORKDIR /src/web

# This is to only allow build process pass, so we can validate the compilation of the project when PR raised
ENV GATEWAY_ENDPOINT="https://localhost/"
ENV WORKOS_API_KEY="sk_test_Sz3IQjepeSWaI4cMS4ms4sMuU"
ENV WORKOS_CLIENT_ID="client_Sz3IQjepeSWaI4cMS4ms4sMuU"

RUN --mount=type=cache,id=pnpm,target=/pnpm/store pnpm install --recursive --frozen-lockfile
RUN pnpm webapp#lint
RUN pnpm webapp#build

FROM build AS test
RUN mkdir -p /coverage/reports

FROM $FINAL_IMAGE AS final
LABEL maintainer="morteza.alizadeh@gmail.com"

RUN apk update && \
    apk add --upgrade apk-tools && \
    apk upgrade --available

WORKDIR /web/apps/webapp
COPY --from=build /src/web/apps/webapp /web/apps/webapp

CMD ["pnpm", "start"]
