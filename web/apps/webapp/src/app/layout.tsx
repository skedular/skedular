'use client';

import { GoogleAnalytics, GoogleTagManager } from '@/libs/analytics';
import { GoogleAnalyticsProvider, LogRocketProvider, NextAuthProvider, RelayProvider, SelectedOrganizationProvider } from '@/libs/providers';
import { AppRouterCacheProvider } from '@mui/material-nextjs/v15-appRouter';
import CssBaseline from '@mui/material/CssBaseline';
import { MuiXLicense } from '@repo/shared/libs/mui';
import {
  DatePickerLocalizationProvider,
  GlobalReloadIdProvider,
  PaletteModeContext,
  PaletteModeProvider,
  SwitchToModernUIProvider,
  ThemeProvider,
} from '@repo/shared/libs/providers';
import { Analytics } from '@vercel/analytics/react';
import { SpeedInsights } from '@vercel/speed-insights/react';
import { Barlow, Inter } from 'next/font/google';
import Script from 'next/script';
import { PropsWithChildren, useContext } from 'react';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const inter = Inter({ subsets: ['latin'], weight: ['100', '200', '300', '400', '500', '600', '700', '800', '900'] });
const barlow = Barlow({ subsets: ['latin'], weight: ['100', '200', '300', '400', '500', '600', '700', '800', '900'] });

const RootLayout = ({ children }: PropsWithChildren) => {
  const paletteMode = useContext(PaletteModeContext);

  return (
    <GlobalReloadIdProvider>
      <SelectedOrganizationProvider>
        <SwitchToModernUIProvider>
          <ThemeProvider mode={paletteMode}>
            <CssBaseline />
            <DatePickerLocalizationProvider>
              <NextAuthProvider>
                <RelayProvider>{children}</RelayProvider>
              </NextAuthProvider>
            </DatePickerLocalizationProvider>
          </ThemeProvider>
        </SwitchToModernUIProvider>
      </SelectedOrganizationProvider>
    </GlobalReloadIdProvider>
  );
};

const ThemedRootLayout = ({ children }: PropsWithChildren) => (
  <html lang="en">
    <title>Skedular</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="Always know who will be in the office" />
    <link rel="icon" href="/images/skedular-icon-primary.svg" />
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
    <body className={`${inter.className} ${barlow.className}`}>
      <AppRouterCacheProvider>
        <PaletteModeProvider>
          <RootLayout>{children}</RootLayout>
        </PaletteModeProvider>
      </AppRouterCacheProvider>
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
