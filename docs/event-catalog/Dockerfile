# syntax = docker/dockerfile:1.12.0
ARG FINAL_IMAGE=node:22-alpine
ARG BUILD_IMAGE=node:22-alpine

FROM $BUILD_IMAGE AS build

RUN mkdir -p /src/docs
COPY docs/event-catalog /src/docs/event-catalog
WORKDIR /src/docs/event-catalog

RUN npm install --frozen-lockfile
RUN npm run build

FROM build AS test
RUN mkdir -p /coverage/reports

FROM $FINAL_IMAGE AS final
LABEL maintainer="morteza.alizadeh@gmail.com"

RUN apk update && \
    apk add --upgrade apk-tools && \
    apk upgrade --available

RUN mkdir -p /docs/event-catalog
WORKDIR /docs/event-catalog
COPY --from=build /src/docs/event-catalog /docs/event-catalog

CMD ["npm", "run", "start"]
