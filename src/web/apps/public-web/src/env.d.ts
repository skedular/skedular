/// <reference types="astro/client" />

interface ImportMetaEnv {
  readonly PUBLIC_SKEDULAR_APP_URL: string;
  readonly PUBLIC_SKEDULAR_SIGNUP_URL: string;
  readonly PUBLIC_SKEDULAR_DEMO_URL: string;
  readonly PUBLIC_SKEDULAR_BECOME_HOST_URL: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
