'use client';

import { AppRouterCacheProvider } from '@mui/material-nextjs/v14-appRouter';
import { GoogleAnalytics, GoogleTagManager } from '@repo/shared/libs/analytics';
import {
    DatePickerLocalizationProvider,
    GoogleAnalyticsProvider,
    NextAuthProvider,
    PaletteModeContext,
    PaletteModeProvider,
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
        <SnackbarProvider>
          <DatePickerLocalizationProvider>
            <NextAuthProvider>
              <GoogleAnalyticsProvider
                ignoreOptOutCookie={false}
                forceOverride={true}
                googleTagManagerContainerId={process.env.NEXT_PUBLIC_GOOGLE_TAG_MANAGER_CONTAINER_ID}
              >
                {children}
              </GoogleAnalyticsProvider>
            </NextAuthProvider>
          </DatePickerLocalizationProvider>
        </SnackbarProvider>
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
      </body>
      <GoogleAnalytics ignoreOptOutCookie={true} forceOverride={false} />
      <GoogleTagManager ignoreOptOutCookie={true} forceOverride={false} />
    </html>
  );
};

export default ThemedRootLayout;
