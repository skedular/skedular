import { createAppSwitcherModel, logAppSwitcherConfiguration, type AppSwitcherLogger, type AppSwitcherModel } from '@skedular/shared';

type AppSwitcherEnvironment = {
  NEXT_PUBLIC_SITE_URL?: string;
  NEXT_PUBLIC_SKEDULAR_APP_URL?: string;
  NEXT_PUBLIC_SKEDULAR_TEAMS_APP_URL?: string;
  NEXT_PUBLIC_SKEDULAR_SPACES_APP_URL?: string;
  NEXT_PUBLIC_SKEDULAR_HOST_APP_URL?: string;
};

type Options = {
  env?: AppSwitcherEnvironment;
  logger?: AppSwitcherLogger;
  logConfiguration?: boolean;
};

const defaultLogger: AppSwitcherLogger = {
  info: (event, message) => console.info(message, event),
  warn: (event, message) => console.warn(message, event),
};

const getEnvironment = (): AppSwitcherEnvironment => ({
  NEXT_PUBLIC_SITE_URL: process.env.NEXT_PUBLIC_SITE_URL,
  NEXT_PUBLIC_SKEDULAR_APP_URL: process.env.NEXT_PUBLIC_SKEDULAR_APP_URL,
  NEXT_PUBLIC_SKEDULAR_TEAMS_APP_URL: process.env.NEXT_PUBLIC_SKEDULAR_TEAMS_APP_URL,
  NEXT_PUBLIC_SKEDULAR_SPACES_APP_URL: process.env.NEXT_PUBLIC_SKEDULAR_SPACES_APP_URL,
  NEXT_PUBLIC_SKEDULAR_HOST_APP_URL: process.env.NEXT_PUBLIC_SKEDULAR_HOST_APP_URL,
});

export const createHostAppSwitcherModel = ({ env = getEnvironment(), logger = defaultLogger, logConfiguration = true }: Options = {}): AppSwitcherModel => {
  const model = createAppSwitcherModel({
    currentAppId: 'webapp-host',
    destinations: {
      webapp: env.NEXT_PUBLIC_SKEDULAR_APP_URL,
      'webapp-teams': env.NEXT_PUBLIC_SKEDULAR_TEAMS_APP_URL,
      'webapp-spaces': env.NEXT_PUBLIC_SKEDULAR_SPACES_APP_URL,
      'webapp-host': env.NEXT_PUBLIC_SKEDULAR_HOST_APP_URL ?? env.NEXT_PUBLIC_SITE_URL,
    },
  });

  if (logConfiguration) {
    logAppSwitcherConfiguration(logger, model);
  }

  return model;
};
