export {};

declare global {
  namespace NodeJS {
    interface ProcessEnv {
      NEXT_PUBLIC_SITE_URL: string;
      NEXT_PUBLIC_LOGROCKET_APP_ID: string;
      NEXT_PUBLIC_SLACK_CLIENT_ID: string;
      NEXT_PUBLIC_SLACK_REDIRECT_URL: string;
      NEXT_PUBLIC_GOOGLE_ANALYTICS_MEASUREMENT_ID: string;
      NEXT_PUBLIC_GOOGLE_TAG_MANAGER_CONTAINER_ID: string;
      GOOGLE_MAPS_API_KEY: string;
      NEXT_PUBLIC_API_ENDPOINT: string;
      NEXT_PUBLIC_APPLICATION_REGISTRATION_ID: string;
      COGNITO_DOMAIN: string;
      COGNITO_CLIENT_ID: string;
      COGNITO_CLIENT_SECRET: string;
      COGNITO_ISSUER: string;
      GOOGLE_CLIENT_ID: string;
      GOOGLE_CLIENT_SECRET: string;
      AZURE_AD_CLIENT_ID: string;
      AZURE_AD_CLIENT_SECRET: string;
      SLACK_CLIENT_SECRET: string;
      GATEWAY_ENDPOINT: string;
      GATEWAY_FETCH_TIMEOUT_MS?: string;
      NEXT_PUBLIC_GRAPHQL_FETCH_TIMEOUT_MS?: string;
      LOG_LEVEL?: 'fatal' | 'error' | 'warn' | 'info' | 'debug' | 'trace' | 'silent';
    }
  }
}
