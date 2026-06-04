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

RUN --mount=type=cache,id=pnpm,target=/pnpm/store pnpm install --recursive --frozen-lockfile
RUN pnpm webapp-spaces-help#lint
RUN pnpm webapp-spaces-help#test
RUN pnpm webapp-spaces-help#build

FROM build AS test
RUN mkdir -p /coverage/reports

FROM $FINAL_IMAGE AS final
LABEL maintainer="morteza.alizadeh@gmail.com"

RUN apk update && \
    apk add --upgrade apk-tools && \
    apk upgrade --available

WORKDIR /web/apps/webapp-spaces-help
COPY --from=build /src/web/apps/webapp-spaces-help /web/apps/webapp-spaces-help

CMD ["pnpm", "start"]
