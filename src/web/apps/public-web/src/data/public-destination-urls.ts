export const requiredPublicDestinationUrlNames = [
  "PUBLIC_SKEDULAR_APP_URL",
  "PUBLIC_SKEDULAR_SIGNUP_URL",
  "PUBLIC_SKEDULAR_TEAMS_APP_URL",
  "PUBLIC_SKEDULAR_SPACES_APP_URL",
  "PUBLIC_SKEDULAR_HOST_APP_URL",
  "PUBLIC_SKEDULAR_DEMO_URL",
  "PUBLIC_SKEDULAR_BECOME_HOST_URL",
  "PUBLIC_SKEDULAR_SLACK_INSTALL_URL",
] as const;

export type PublicDestinationUrlName =
  (typeof requiredPublicDestinationUrlNames)[number];

const publicDestinationUrlPurpose: Record<PublicDestinationUrlName, string> = {
  PUBLIC_SKEDULAR_APP_URL: "app/search/booking destination",
  PUBLIC_SKEDULAR_SIGNUP_URL: "login/sign-up destination",
  PUBLIC_SKEDULAR_TEAMS_APP_URL: "Teams app destination",
  PUBLIC_SKEDULAR_SPACES_APP_URL: "Spaces app destination",
  PUBLIC_SKEDULAR_HOST_APP_URL: "Host app destination",
  PUBLIC_SKEDULAR_DEMO_URL: "demo/contact destination",
  PUBLIC_SKEDULAR_BECOME_HOST_URL: "become-a-host destination",
  PUBLIC_SKEDULAR_SLACK_INSTALL_URL: "Slack install destination",
};

function readRequiredPublicUrl(name: PublicDestinationUrlName): string {
  const value = import.meta.env[name]?.trim();

  if (!value) {
    throw new Error(
      `${name} is required for the ${publicDestinationUrlPurpose[name]}`,
    );
  }

  return value;
}

export const publicDestinationUrls = {
  app: readRequiredPublicUrl("PUBLIC_SKEDULAR_APP_URL"),
  signup: readRequiredPublicUrl("PUBLIC_SKEDULAR_SIGNUP_URL"),
  teamsApp: readRequiredPublicUrl("PUBLIC_SKEDULAR_TEAMS_APP_URL"),
  spacesApp: readRequiredPublicUrl("PUBLIC_SKEDULAR_SPACES_APP_URL"),
  hostApp: readRequiredPublicUrl("PUBLIC_SKEDULAR_HOST_APP_URL"),
  demo: readRequiredPublicUrl("PUBLIC_SKEDULAR_DEMO_URL"),
  becomeHost: readRequiredPublicUrl("PUBLIC_SKEDULAR_BECOME_HOST_URL"),
  slackInstall: readRequiredPublicUrl("PUBLIC_SKEDULAR_SLACK_INSTALL_URL"),
};

export function getPublicDestinationUrl(
  name: PublicDestinationUrlName,
): string {
  if (name === "PUBLIC_SKEDULAR_APP_URL") {
    return publicDestinationUrls.app;
  }

  if (name === "PUBLIC_SKEDULAR_BECOME_HOST_URL") {
    return publicDestinationUrls.becomeHost;
  }

  if (name === "PUBLIC_SKEDULAR_TEAMS_APP_URL") {
    return publicDestinationUrls.teamsApp;
  }

  if (name === "PUBLIC_SKEDULAR_SPACES_APP_URL") {
    return publicDestinationUrls.spacesApp;
  }

  if (name === "PUBLIC_SKEDULAR_HOST_APP_URL") {
    return publicDestinationUrls.hostApp;
  }

  if (name === "PUBLIC_SKEDULAR_DEMO_URL") {
    return publicDestinationUrls.demo;
  }

  if (name === "PUBLIC_SKEDULAR_SLACK_INSTALL_URL") {
    return publicDestinationUrls.slackInstall;
  }

  return publicDestinationUrls.signup;
}
