'use client';

import { GoogleAnalytics, GoogleTagManager } from '@/libs/analytics';
import { GoogleAnalyticsProvider, LogRocketProvider, NextAuthProvider, RelayProvider } from '@/libs/providers';
import { AppRouterCacheProvider } from '@mui/material-nextjs/v14-appRouter';
import CssBaseline from '@mui/material/CssBaseline';
import { MuiXLicense } from '@repo/shared/libs/mui';
import {
  BreadcrumpsProvider,
  DatePickerLocalizationProvider,
  GlobalReloadIdProvider,
  PaletteModeContext,
  PaletteModeProvider,
  ThemeProvider,
} from '@repo/shared/libs/providers';
import { Analytics } from '@vercel/analytics/react';
import { SpeedInsights } from '@vercel/speed-insights/react';
import Script from 'next/script';
import { useContext } from 'react';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const RootLayout = ({ children }: { children: React.ReactNode }) => {
  const paletteMode = useContext(PaletteModeContext);

  return (
    <GlobalReloadIdProvider>
      <BreadcrumpsProvider>
        <AppRouterCacheProvider>
          <ThemeProvider mode={paletteMode}>
            <CssBaseline />
            <DatePickerLocalizationProvider>
              <NextAuthProvider>
                <RelayProvider>{children}</RelayProvider>
              </NextAuthProvider>
            </DatePickerLocalizationProvider>
          </ThemeProvider>
        </AppRouterCacheProvider>
      </BreadcrumpsProvider>
    </GlobalReloadIdProvider>
  );
};

const ThemedRootLayout = ({ children }: { children: React.ReactNode }) => (
  <html lang="en">
    <title>UnityHub</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="Always know who will be in the office" />
    <link rel="icon" href="/favicon.ico" />
    {process.env.NEXT_PUBLIC_MICROANALYTICS_APP_ID && (
      <Script
        data-host="https://app.microanalytics.io"
        data-dnt="false"
        src="https://app.microanalytics.io/js/script.js"
        id={process.env.NEXT_PUBLIC_MICROANALYTICS_APP_ID}
        async
        defer
      />
    )}
    <body>
      <PaletteModeProvider>
        <RootLayout>{children}</RootLayout>
      </PaletteModeProvider>
      <Analytics />
      <SpeedInsights />
      <MuiXLicense />
      <ToastContainer position="top-right" pauseOnFocusLoss pauseOnHover hideProgressBar={false} draggable rtl={false} />
    </body>
    <GoogleAnalytics ignoreOptOutCookie={true} forceOverride={false} />
    <GoogleTagManager ignoreOptOutCookie={true} forceOverride={false} />
    <LogRocketProvider ignoreOptOutCookie={true} forceOverride={false} logRocketAppId={process.env.NEXT_PUBLIC_LOGROCKET_APP_ID} />
    <GoogleAnalyticsProvider
      ignoreOptOutCookie={true}
      forceOverride={false}
      googleTagManagerContainerId={process.env.NEXT_PUBLIC_GOOGLE_TAG_MANAGER_CONTAINER_ID}
    />
  </html>
);

export default ThemedRootLayout;
