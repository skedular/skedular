# Contract: Web Application Project Structure

**Date**: 2026-04-18  
**Related**: [data-model.md](../data-model.md), [terraform-structure.md](terraform-structure.md)

## Overview

This contract defines the directory structure, configuration files, and application entry points for each web application project (`webapp`, `webapp-teams`, `webapp-spaces`).

---

## Project Root Directory Structure

```text
web/apps/{project_id}/
├── infrastructure/                  # Terraform infrastructure-as-code
│   ├── modules/                     # Terraform modules (shared across workspaces)
│   │   ├── app/                     # Application infrastructure
│   │   ├── database/                # Database infrastructure
│   │   └── [other modules]
│   ├── workspaces/                  # Environment-specific configurations
│   │   ├── staging/
│   │   ├── common_resources/
│   │   └── production/
│   └── README.md
├── src/                             # Application source code
│   ├── pages/                       # Next.js pages or application routes
│   │   ├── api/                     # API routes (if applicable)
│   │   ├── index.tsx                # Home page
│   │   ├── _app.tsx                 # App wrapper (Next.js)
│   │   └── _document.tsx            # Document wrapper (Next.js)
│   ├── components/                  # React components
│   │   ├── common/                  # Shared components
│   │   ├── layout/                  # Layout components (Header, Footer, etc.)
│   │   ├── design-system/           # Design system component wrappers
│   │   └── [feature-specific]/      # Feature components
│   ├── styles/                      # Global styles and CSS modules
│   │   ├── globals.css              # Global styles
│   │   └── layout.module.css        # Layout styles
│   ├── lib/                         # Utility functions and helpers
│   │   ├── api.ts                   # API client functions
│   │   ├── constants.ts             # Constants and configuration
│   │   └── [other utilities]
│   ├── types/                       # TypeScript type definitions
│   │   └── index.ts                 # Shared types
│   └── [feature-dirs]/              # Feature-specific subdirectories
├── public/                          # Static assets (images, fonts, etc.)
│   ├── favicon.ico
│   └── [other static files]
├── docs/                            # Project documentation
│   ├── README.md                    # Project overview
│   ├── SETUP.md                     # Setup and local development guide
│   ├── DEPLOYMENT.md                # Deployment procedures
│   └── [other documentation]
├── .github/                         # GitHub configuration
│   └── workflows/                   # GitHub Actions workflows
│       ├── lint-validate-infrastructure.yml
│       ├── build-deploy.yml
│       └── health-check.yml
├── .env.example                     # Example environment variables
├── .env.local                       # Local environment variables (git-ignored)
├── .gitignore                       # Git ignore patterns
├── package.json                     # NPM package configuration
├── package-lock.json                # NPM lock file (committed)
├── tsconfig.json                    # TypeScript configuration
├── next.config.js                   # Next.js configuration (if using Next.js)
├── jest.config.js                   # Jest test configuration (if using Jest)
├── eslint.config.js                 # ESLint configuration
├── README.md                        # Root project README
└── [other config files]
```

---

## File Specifications

### package.json

**Requirements**:

- Must list shared design system as a dependency
- All three products MUST use the same design system version
- Must include build, test, lint, and dev scripts
- Must specify Node.js version requirement in `engines` field

**Example**:

```json
{
  "name": "@skedular/webapp-teams",
  "version": "1.0.0",
  "description": "Skedular Teams Web Application",
  "private": true,
  "engines": {
    "node": ">=18.0.0"
  },
  "scripts": {
    "dev": "next dev",
    "build": "next build",
    "start": "next start",
    "lint": "eslint src --max-warnings 0",
    "test": "jest",
    "test:watch": "jest --watch"
  },
  "dependencies": {
    "react": "^18.2.0",
    "react-dom": "^18.2.0",
    "next": "^14.0.0",
    "@skedular/design-system": "1.0.0",
    "axios": "^1.6.0"
  },
  "devDependencies": {
    "@types/react": "^18.2.0",
    "@types/node": "^20.0.0",
    "typescript": "^5.0.0",
    "eslint": "^8.0.0",
    "jest": "^29.0.0",
    "@testing-library/react": "^14.0.0"
  }
}
```

**Validation**:

- [ ] Design system version matches other products (use exact version, not range)
- [ ] Node.js version >= 18.0.0
- [ ] All required scripts present (dev, build, lint, test)
- [ ] No conflicting dependency versions

---

### tsconfig.json

**Requirements**:

- TypeScript strict mode enabled
- Target: ES2020 or later
- Module: ESNext
- Lib includes DOM and ES2020

**Example**:

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "jsx": "react-jsx",
    "module": "ESNext",
    "moduleResolution": "node",
    "strict": true,
    "esModuleInterop": true,
    "skipLibCheck": true,
    "forceConsistentCasingInFileNames": true,
    "resolveJsonModule": true,
    "declaration": true,
    "declarationMap": true,
    "sourceMap": true,
    "baseUrl": ".",
    "paths": {
      "@/*": ["./src/*"]
    }
  },
  "include": ["src/**/*"],
  "exclude": ["node_modules", "dist", ".next"]
}
```

---

### .env.example

**Requirements**:

- Template of all environment variables needed for the application
- No actual secrets (example values only)
- Clear documentation of each variable

**Example**:

```env
# Application Configuration
NEXT_PUBLIC_APP_ENV=development
NEXT_PUBLIC_APP_NAME=webapp-teams
NEXT_PUBLIC_API_URL=http://localhost:3000/api

# Design System
NEXT_PUBLIC_DESIGN_SYSTEM_VERSION=1.0.0

# Backend Services
NEXT_PUBLIC_BACKEND_URL=https://api.skedular.io
BACKEND_API_TOKEN=your-api-token-here

# Third-party Services
SENTRY_DSN=https://example@sentry.io/12345
ANALYTICS_ID=UA-12345678-1

# Feature Flags
NEXT_PUBLIC_FEATURE_PRIVATE_DASHBOARD=true
NEXT_PUBLIC_FEATURE_ANALYTICS=true
```

---

### next.config.js (if using Next.js)

**Requirements**:

- Configure for production optimization
- Enable SWR (Stale-While-Revalidate) caching
- Configure security headers
- Enable strict mode

**Example**:

```javascript
/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  swcMinify: true,

  async headers() {
    return [
      {
        source: "/:path*",
        headers: [
          {
            key: "X-Frame-Options",
            value: "SAMEORIGIN",
          },
          {
            key: "X-Content-Type-Options",
            value: "nosniff",
          },
        ],
      },
    ];
  },

  env: {
    APP_VERSION: process.env.npm_package_version,
  },
};

module.exports = nextConfig;
```

---

### src/pages/\_app.tsx (Application Entry Point)

**Requirements**:

- Import shared design system styles and components
- Set up global logging/observability
- Configure error boundaries
- Load environment configuration

**Example**:

```typescript
import React from 'react';
import type { AppProps } from 'next/app';
import { ThemeProvider } from '@skedular/design-system';
import '@skedular/design-system/styles/globals.css';
import { initializeLogger } from '@/lib/logger';
import GlobalStyles from '@/styles/globals.css';

// Initialize logging at app startup
initializeLogger({
  environment: process.env.NEXT_PUBLIC_APP_ENV,
  appName: process.env.NEXT_PUBLIC_APP_NAME,
  version: process.env.APP_VERSION
});

export default function App({ Component, pageProps }: AppProps) {
  return (
    <ThemeProvider>
      <Component {...pageProps} />
    </ThemeProvider>
  );
}
```

---

### src/lib/logger.ts (Logging Configuration)

**Requirements**:

- Use Enterprise.Shared logging framework
- Log application initialization
- Log API calls and responses
- Log errors and exceptions
- Include correlation context (request ID, user ID, etc.)

**Example**:

```typescript
import { createLogger } from "@skedular/logging";

export const logger = createLogger({
  service: "webapp-teams",
  environment: process.env.NEXT_PUBLIC_APP_ENV,
  version: process.env.APP_VERSION,
});

export function initializeLogger(config: { environment: string; appName: string; version: string }) {
  logger.info("Application startup", {
    environment: config.environment,
    appName: config.appName,
    version: config.version,
    timestamp: new Date().toISOString(),
  });
}

export function logApiCall(method: string, url: string, statusCode?: number) {
  logger.info("API call", {
    method,
    url,
    statusCode,
    timestamp: new Date().toISOString(),
  });
}
```

---

### src/components/layout/index.tsx (Layout Component)

**Requirements**:

- Use typography wrappers from shared design system (not direct Material-UI Typography)
- Consistent layout across all three products
- Header and footer components
- Navigation structure

**Example**:

```typescript
import React from 'react';
import {
  BodyTypography,
  HeadingTypography
} from '@skedular/design-system';

export default function Layout({ children }: { children: React.ReactNode }) {
  return (
    <div>
      <header className="header">
        <HeadingTypography level={1}>
          Skedular - teams application
        </HeadingTypography>
      </header>

      <main className="main-content">
        {children}
      </main>

      <footer className="footer">
        <BodyTypography>
          © 2026 Skedular. All rights reserved.
        </BodyTypography>
      </footer>
    </div>
  );
}
```

---

### .github/workflows/build-deploy.yml

**Location**: `.github/workflows/build-deploy.yml`

**Requirements**:

- Follows contract specified in [github-actions-workflows.md](github-actions-workflows.md)
- Triggers on push to main and PR
- Builds, tests, lints, and deploys to Vercel

---

## Application Startup Checklist

When running the application locally:

1. **Environment Setup**:
   - [ ] Node.js 18+ installed: `node --version`
   - [ ] Dependencies installed: `npm ci` or `pnpm install`
   - [ ] Environment file exists: `.env.local`
   - [ ] All required env vars set (check against .env.example)

2. **Build Verification**:
   - [ ] Build succeeds without errors: `npm run build`
   - [ ] TypeScript compilation passes
   - [ ] ESLint passes without errors: `npm run lint`
   - [ ] Unit tests pass: `npm run test`

3. **Design System Integration**:
   - [ ] Design system package installed
   - [ ] Design system styles loaded (no CSS errors)
   - [ ] Design system components render without errors

4. **Local Development**:
   - [ ] Dev server starts: `npm run dev`
   - [ ] Application accessible at http://localhost:3000
   - [ ] Home page renders with design system components
   - [ ] API calls to backend succeed (check console logs)
   - [ ] Health check endpoint responds: http://localhost:3000/health

---

## Deployment Checklist

Before deploying to staging/production:

1. **Code Quality**:
   - [ ] All tests pass
   - [ ] Linting passes (0 warnings)
   - [ ] TypeScript strict mode passes
   - [ ] No console errors in dev build

2. **Configuration**:
   - [ ] Environment variables set correctly in Vercel
   - [ ] Secrets configured (API tokens, etc.)
   - [ ] Design system version matches other products

3. **Terraform**:
   - [ ] Infrastructure validates: `terraform validate`
   - [ ] No drift detected: `terraform plan` shows no unexpected changes
   - [ ] All three workspaces validate

4. **GitHub Actions**:
   - [ ] lint-validate-infrastructure workflow passes
   - [ ] build-deploy workflow passes all jobs
   - [ ] Deployment to Vercel succeeds

5. **Post-Deployment**:
   - [ ] Application responds to health check
   - [ ] Design system components render correctly
   - [ ] No errors in application logs
   - [ ] API connectivity verified

---

## Contract Compliance

All three web applications MUST:

- [ ] Follow this directory structure exactly
- [ ] Use the shared design system with same version as other products
- [ ] Include all required configuration files (package.json, tsconfig.json, next.config.js, etc.)
- [ ] Import and use design system typography wrappers (not direct Material-UI)
- [ ] Implement structured logging with correlation IDs
- [ ] Include GitHub Actions workflows matching [github-actions-workflows.md](github-actions-workflows.md)
- [ ] Have health check endpoint at `/health`
- [ ] Support environment variables from .env.local and Vercel configuration
- [ ] Build successfully without errors
- [ ] Pass linting and testing
- [ ] Deploy successfully to Vercel
