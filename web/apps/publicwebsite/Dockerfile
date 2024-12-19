# syntax = docker/dockerfile:1.12.0
ARG FINAL_IMAGE=node:20-alpine
ARG BUILD_IMAGE=node:20-alpine

FROM $BUILD_IMAGE AS build
ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"
RUN corepack enable

RUN mkdir -p /src/web
COPY web /src/web
WORKDIR /src/web

# This is to only allow build process pass, so we can validate the compilation of the project when PR raised
ENV BILLING_ENDPOINT="https://localhost""
ENV BOOKING_ENDPOINT="https://localhost""
ENV CUSTOMER_ENDPOINT="https://localhost""
ENV GATEWAY_ENDPOINT="https://localhost""
ENV LOCATION_ENDPOINT="https://localhost""
ENV NOTIFICATION_ENDPOINT="https://localhost""
ENV ORGANIZATION_ENDPOINT="https://localhost""
ENV PAYMENT_ENDPOINT="https://localhost""
ENV SLACK_ENDPOINT="https://localhost""
ENV TEAM_ENDPOINT="https://localhost""
ENV MSTEAMS_ENDPOINT="https://localhost""

RUN --mount=type=cache,id=pnpm,target=/pnpm/store pnpm install --recursive --frozen-lockfile
RUN pnpm publicwebsite#lint
RUN pnpm publicwebsite#build

FROM build AS test
RUN mkdir -p /coverage/reports

FROM $FINAL_IMAGE AS final
LABEL maintainer="morteza.alizadeh@gmail.com"

RUN apk update && \
    apk add --upgrade apk-tools && \
    apk upgrade --available

RUN mkdir -p /web/apps/msteams
WORKDIR /web/apps/publicwebsite
COPY --from=build /src/web/apps/publicwebsite /web/apps/publicwebsite

CMD ["pnpm", "start"]
