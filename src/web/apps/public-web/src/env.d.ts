/// <reference types="astro/client" />

interface ImportMetaEnv {
  readonly PUBLIC_SKEDULAR_SIGNUP_URL: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
