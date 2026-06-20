/// <reference types="astro/client" />

interface ImportMetaEnv {
  readonly PUBLIC_WEB_SITE_URL: string;
  readonly PUBLIC_SKEDULAR_APP_URL: string;
  readonly PUBLIC_SKEDULAR_SIGNUP_URL: string;
  readonly PUBLIC_SKEDULAR_TEAMS_APP_URL: string;
  readonly PUBLIC_SKEDULAR_SPACES_APP_URL: string;
  readonly PUBLIC_SKEDULAR_HOST_APP_URL: string;
  readonly PUBLIC_SKEDULAR_DEMO_URL: string;
  readonly PUBLIC_SKEDULAR_BECOME_HOST_URL: string;
  readonly PUBLIC_SKEDULAR_SLACK_INSTALL_URL: string;
  readonly PUBLIC_GOOGLE_ANALYTICS_MEASUREMENT_ID?: string;
  readonly PUBLIC_LOGROCKET_APP_ID?: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
