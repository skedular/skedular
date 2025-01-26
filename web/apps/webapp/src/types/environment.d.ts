export {};

declare global {
  namespace NodeJS {
    interface ProcessEnv {
      NEXT_PUBLIC_SITE_URL: string;
      NEXT_PUBLIC_MICROANALYTICS_APP_ID: string;
      NEXT_PUBLIC_LOGROCKET_APP_ID: string;
      NEXT_PUBLIC_SLACK_CLIENT_ID: string;
      NEXT_PUBLIC_GOOGLE_ANALYTICS_MEASUREMENT_ID: string;
      NEXT_PUBLIC_GOOGLE_TAG_MANAGER_CONTAINER_ID: string;
      NEXT_PUBLIC_PAYMENT_ENDPOINT: string;
      COGNITO_DOMAIN: string;
      COGNITO_CLIENT_ID: string;
      COGNITO_CLIENT_SECRET: string;
      COGNITO_ISSUER: string;
      GOOGLE_CLIENT_ID: string;
      GOOGLE_CLIENT_SECRET: string;
      AZURE_AD_CLIENT_ID: string;
      AZURE_AD_CLIENT_SECRET: string;
      SLACK_CLIENT_ID: string;
      SLACK_CLIENT_SECRET: string;
      GATEWAY_ENDPOINT: string;
      CUSTOMER_ENDPOINT: string;
      LOCATION_ENDPOINT: string;
      NOTIFICATION_ENDPOINT: string;
      ORGANIZATION_ENDPOINT: string;
      SLACK_ENDPOINT: string;
      TEAM_ENDPOINT: string;
      BOOKING_ENDPOINT: string;
      PAYMENT_ENDPOINT: string;
      BILLING_ENDPOINT: string;
      MSTEAMS_ENDPOINT: string;
    }
  }
}
