import { createAppSwitcherModel, logAppSwitcherConfiguration, type AppSwitcherLogger, type AppSwitcherModel } from '@skedular/shared';

const defaultAppSwitcherLogger: AppSwitcherLogger = {
  info: (event, message) => console.info(message, event),
  warn: (event, message) => console.warn(message, event),
};

type AppSwitcherEnvironment = {
  NEXT_PUBLIC_SITE_URL?: string;
  NEXT_PUBLIC_SKEDULAR_APP_URL?: string;
  NEXT_PUBLIC_SKEDULAR_TEAMS_APP_URL?: string;
  NEXT_PUBLIC_SKEDULAR_SPACES_APP_URL?: string;
};

export type CreateAppSwitcherModelOptions = {
  env?: AppSwitcherEnvironment;
  logger?: AppSwitcherLogger;
  logConfiguration?: boolean;
};

const getDefaultAppSwitcherEnvironment = (): AppSwitcherEnvironment => ({
  NEXT_PUBLIC_SITE_URL: process.env.NEXT_PUBLIC_SITE_URL,
  NEXT_PUBLIC_SKEDULAR_APP_URL: process.env.NEXT_PUBLIC_SKEDULAR_APP_URL,
  NEXT_PUBLIC_SKEDULAR_TEAMS_APP_URL: process.env.NEXT_PUBLIC_SKEDULAR_TEAMS_APP_URL,
  NEXT_PUBLIC_SKEDULAR_SPACES_APP_URL: process.env.NEXT_PUBLIC_SKEDULAR_SPACES_APP_URL,
});

export const createTeamsAppSwitcherModel = ({
  env = getDefaultAppSwitcherEnvironment(),
  logger: appLogger = defaultAppSwitcherLogger,
  logConfiguration = true,
}: CreateAppSwitcherModelOptions = {}): AppSwitcherModel => {
  const model = createAppSwitcherModel({
    currentAppId: 'webapp-teams',
    destinations: {
      webapp: env.NEXT_PUBLIC_SKEDULAR_APP_URL,
      'webapp-teams': env.NEXT_PUBLIC_SKEDULAR_TEAMS_APP_URL ?? env.NEXT_PUBLIC_SITE_URL,
      'webapp-spaces': env.NEXT_PUBLIC_SKEDULAR_SPACES_APP_URL,
    },
  });

  if (logConfiguration) {
    logAppSwitcherConfiguration(appLogger, model);
  }

  return model;
};
