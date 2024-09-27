'use client';

import { AppRouterCacheProvider } from '@mui/material-nextjs/v14-appRouter';
import CssBaseline from '@mui/material/CssBaseline';
import { GoogleAnalytics, GoogleTagManager } from '@repo/shared/libs/analytics';
import { MuiXLicense } from '@repo/shared/libs/mui';
import {
  DatePickerLocalizationProvider,
  GoogleAnalyticsProvider,
  LogRocketProvider,
  NextAuthProvider,
  PaletteModeContext,
  PaletteModeProvider,
  RelayProvider,
  SelectedOrganizationProvider,
  SnackbarProvider,
  ThemeProvider,
} from '@repo/shared/libs/providers';
import { Analytics } from '@vercel/analytics/react';
import { SpeedInsights } from '@vercel/speed-insights/react';
import { Roboto } from 'next/font/google';
import Script from 'next/script';
import { useContext } from 'react';

const roboto = Roboto({
  weight: ['300', '400', '500', '700'],
  subsets: ['latin'],
  display: 'swap',
  variable: '--font-roboto',
});

const RootLayout = ({ children }: { children: React.ReactNode }) => {
  const paletteMode = useContext(PaletteModeContext);

  return (
    <AppRouterCacheProvider>
      <ThemeProvider mode={paletteMode}>
        <CssBaseline />
        <SelectedOrganizationProvider>
          <SnackbarProvider>
            <DatePickerLocalizationProvider>
              <NextAuthProvider>
                <LogRocketProvider ignoreOptOutCookie={true} forceOverride={false} logRocketAppId={process.env.NEXT_PUBLIC_LOGROCKET_APP_ID}>
                  <RelayProvider>
                    <GoogleAnalyticsProvider
                      ignoreOptOutCookie={true}
                      forceOverride={false}
                      googleTagManagerContainerId={process.env.NEXT_PUBLIC_GOOGLE_TAG_MANAGER_CONTAINER_ID}
                    >
                      {children}
                    </GoogleAnalyticsProvider>
                  </RelayProvider>
                </LogRocketProvider>
              </NextAuthProvider>
            </DatePickerLocalizationProvider>
          </SnackbarProvider>
        </SelectedOrganizationProvider>
      </ThemeProvider>
    </AppRouterCacheProvider>
  );
};

const ThemedRootLayout = ({ children }: { children: React.ReactNode }) => {
  return (
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
      <body className={roboto.variable}>
        <PaletteModeProvider>
          <RootLayout>{children}</RootLayout>
        </PaletteModeProvider>
        <Analytics />
        <SpeedInsights />
        <MuiXLicense />
      </body>
      <GoogleAnalytics ignoreOptOutCookie={true} forceOverride={false} />
      <GoogleTagManager ignoreOptOutCookie={true} forceOverride={false} />
    </html>
  );
};

export default ThemedRootLayout;
