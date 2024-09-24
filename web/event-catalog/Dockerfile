# syntax = docker/dockerfile:1.7.0
ARG FINAL_IMAGE=node:20-alpine
ARG BUILD_IMAGE=node:20-alpine

FROM $BUILD_IMAGE AS build

RUN mkdir -p /src/web
COPY web/event-catalog /src/web/event-catalog
WORKDIR /src/web/event-catalog

RUN npm install --frozen-lockfile
RUN npm run build

FROM build AS test
RUN mkdir -p /coverage/reports

FROM $FINAL_IMAGE AS final
LABEL maintainer="morteza.alizadeh@gmail.com"

RUN apk update && \
    apk add --upgrade apk-tools && \
    apk upgrade --available

RUN mkdir -p /web/event-catalog
WORKDIR /web/event-catalog
COPY --from=build /src/web/event-catalog /web/event-catalog

CMD ["npm", "run", "start"]
