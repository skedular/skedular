import pino from 'pino';

type BrowserTelemetryGlobal = typeof globalThis & { __skedularBrowserLog?: (log: Record<string, unknown>) => void };

export default pino({
  name: 'webapp',
  level: process.env.LOG_LEVEL ?? 'info',
  redact: [
    'password',
    '*.password',
    'secret',
    '*.secret',
    'token',
    '*.token',
    'accessToken',
    '*.accessToken',
    'refreshToken',
    '*.refreshToken',
    'authorization',
    '*.authorization',
    'cookie',
    '*.cookie',
    'email',
    '*.email',
    'emailAddress',
    '*.emailAddress',
    'phone',
    '*.phone',
    'phoneNumber',
    '*.phoneNumber',
    'address',
    '*.address',
  ],
  base: {
    app: 'webapp',
  },
  browser: {
    write: (log: object) => (globalThis as BrowserTelemetryGlobal).__skedularBrowserLog?.(log as Record<string, unknown>),
  },
});
