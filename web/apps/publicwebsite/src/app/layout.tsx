'use client';

import { GoogleAnalytics, GoogleTagManager } from '@repo/shared/libs/analytics';
import {
  ColorModeProvider,
  DatePickerLocalizationProvider,
  GoogleAnalyticsProvider,
  NextAuthProvider,
  SnackbarProvider,
  ThemeProvider,
} from '@repo/shared/libs/providers';
import type { ColorMode } from '@repo/shared/libs/theme';
import { Analytics } from '@vercel/analytics/react';
import { Inter } from 'next/font/google';
import Script from 'next/script';
import { useState } from 'react';

const inter = Inter({ subsets: ['latin'] });

const RootLayout = ({ children }: { children: React.ReactNode }) => {
  const [mode, setMode] = useState<ColorMode>('light');

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
      <body>
        <ColorModeProvider setMode={setMode} loadDefaultSystemMode={false}>
          <ThemeProvider mode={mode}>
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
        </ColorModeProvider>
        <Analytics />
      </body>
      <GoogleAnalytics ignoreOptOutCookie={false} forceOverride={true} />
      <GoogleTagManager ignoreOptOutCookie={false} forceOverride={true} />
    </html>
  );
};

export default RootLayout;
