# syntax = docker/dockerfile:1.7.0
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
ENV BILLING_ENDPOINT="/temp"
ENV BOOKING_ENDPOINT="/temp"
ENV CUSTOMER_ENDPOINT="/temp"
ENV GATEWAY_ENDPOINT="/temp"
ENV LOCATION_ENDPOINT="/temp"
ENV NOTIFICATION_ENDPOINT="/temp"
ENV ORGANIZATION_ENDPOINT="/temp"
ENV PAYMENT_ENDPOINT="/temp"
ENV SLACK_ENDPOINT="/temp"
ENV TEAM_ENDPOINT="/temp"

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

WORKDIR /web/
WORKDIR /web/apps/publicwebsite
COPY --from=build /src/web/apps/publicwebsite /web/apps/publicwebsite

CMD ["pnpm", "start"]
