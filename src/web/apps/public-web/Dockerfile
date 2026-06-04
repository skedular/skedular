# syntax = docker/dockerfile:1.12.0
ARG FINAL_IMAGE=node:22-alpine
ARG BUILD_IMAGE=node:22-alpine

FROM $BUILD_IMAGE AS build
ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"
ENV PUBLIC_SKEDULAR_SIGNUP_URL="https://app.example.test/sign-up"
RUN corepack enable

RUN mkdir -p /src/web
COPY .editorconfig /src/.editorconfig
COPY src/web /src/web
WORKDIR /src/web

RUN --mount=type=cache,id=pnpm,target=/pnpm/store pnpm install --recursive --frozen-lockfile
RUN pnpm public-web#lint
RUN pnpm public-web#test
RUN pnpm public-web#build

FROM build AS test
RUN mkdir -p /coverage/reports

FROM $FINAL_IMAGE AS final
LABEL maintainer="morteza.alizadeh@gmail.com"

RUN apk update && \
    apk add --upgrade apk-tools && \
    apk upgrade --available

WORKDIR /web/apps/public-web
COPY --from=build /src/web/apps/public-web /web/apps/public-web

CMD ["pnpm", "start"]
