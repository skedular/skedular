# syntax = docker/dockerfile:1.12.0
ARG FINAL_IMAGE=node:22-alpine
ARG BUILD_IMAGE=node:22-alpine

FROM $BUILD_IMAGE AS build
ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"
ENV PUBLIC_WEB_SITE_URL="https://getskedular.com"
ENV PUBLIC_SKEDULAR_APP_URL="https://skedular.app"
ENV PUBLIC_SKEDULAR_SIGNUP_URL="https://skedular.app"
ENV PUBLIC_SKEDULAR_TEAMS_APP_URL="https://teams.skedular.app"
ENV PUBLIC_SKEDULAR_SPACES_APP_URL="https://spaces.skedular.app"
ENV PUBLIC_SKEDULAR_HOST_APP_URL="https://host.skedular.app"
ENV PUBLIC_SKEDULAR_DEMO_URL="https://skedular.app"
ENV PUBLIC_SKEDULAR_BECOME_HOST_URL="https://skedular.app"
ENV PUBLIC_SKEDULAR_SLACK_INSTALL_URL="https://slack.com/oauth/v2/authorize?scope=app_mentions%3Aread%2Cchannels%3Ajoin%2Cchannels%3Amanage%2Cchannels%3Aread%2Cchat%3Awrite%2Cteam%3Aread%2Cusers%3Aread%2Cusers%3Aread.email%2Cusers.profile%3Aread&user_scope=users.profile%3Awrite%2Cusers.profile%3Aread&redirect_uri=https%3A%2F%2Fslackapi.skedular.app%2Fv1%2Fslack%2Fcallback&client_id=118234978193.5578039519830"
ENV PUBLIC_GOOGLE_ANALYTICS_MEASUREMENT_ID=""
ENV PUBLIC_LOGROCKET_APP_ID=""
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
