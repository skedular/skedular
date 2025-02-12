'use client';

import { GoogleAnalytics, GoogleTagManager } from '@/libs/analytics';
import { MuiXLicense } from '@/libs/mui';
import {
  DatePickerLocalizationProvider,
  GlobalReloadIdProvider,
  GoogleAnalyticsProvider,
  InMsTeamsContext,
  InMsTeamsProvider,
  LogRocketProvider,
  PaletteModeContext,
  PaletteModeProvider,
  RelayProvider,
  SelectedOrganizationProvider,
  ThemeProvider,
} from '@/libs/providers';
import { TeamsUserCredential } from '@microsoft/teamsfx';
import { AppRouterCacheProvider } from '@mui/material-nextjs/v15-appRouter';
import CssBaseline from '@mui/material/CssBaseline';
import { Analytics } from '@vercel/analytics/react';
import { SpeedInsights } from '@vercel/speed-insights/react';
import { AuthKitProvider } from '@workos-inc/authkit-nextjs/components';
import { Barlow, Inter } from 'next/font/google';
import Script from 'next/script';
import type { PropsWithChildren } from 'react';
import { useContext, useEffect, useState } from 'react';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const inter = Inter({ subsets: ['latin'], weight: ['100', '200', '300', '400', '500', '600', '700', '800', '900'] });
const barlow = Barlow({ subsets: ['latin'], weight: ['100', '200', '300', '400', '500', '600', '700', '800', '900'] });

const RootLayout = ({ children }: PropsWithChildren) => {
  const paletteMode = useContext(PaletteModeContext);
  const inMsTeams = useContext(InMsTeamsContext);
  const [token, setToken] = useState<string | null>(null);

  useEffect(() => {
    if (!inMsTeams) {
      return;
    }

    const appInitialize = async () => {
      const scope: string[] = [];
      const credential = new TeamsUserCredential({
        initiateLoginEndpoint: new URL('auth-start.html', process.env.NEXT_PUBLIC_SITE_URL).href,
        clientId: process.env.NEXT_PUBLIC_APPLICATION_REGISTRATION_ID!,
      });

      try {
        try {
          const accessTokenResult = await credential.getToken(scope);
          if (!accessTokenResult) {
            throw new Error('accessTokenResult is null');
          }

          setToken(accessTokenResult.token);
        } catch (error) {
          await credential.login(scope);

          const accessTokenResult = await credential.getToken(scope);
          if (!accessTokenResult) {
            throw new Error('accessTokenResult is null');
          }

          setToken(accessTokenResult.token);
        }
      } catch (err) {
        alert('Login failed: ' + err);

        return;
      }
    };

    appInitialize();
  }, [inMsTeams]);

  return (
    <GlobalReloadIdProvider>
      <SelectedOrganizationProvider>
        <ThemeProvider mode={paletteMode}>
          <CssBaseline />
          <DatePickerLocalizationProvider>
            <AuthKitProvider>
              <RelayProvider token={token}>{children}</RelayProvider>
            </AuthKitProvider>
          </DatePickerLocalizationProvider>
        </ThemeProvider>
      </SelectedOrganizationProvider>
    </GlobalReloadIdProvider>
  );
};

const ThemedRootLayout = ({ children }: PropsWithChildren) => {
  return (
    <html lang="en">
      <title>Skedular</title>
      <meta name="viewport" content="width=device-width, initial-scale=1" />
      <meta name="description" content="The premier solution for modern workspace management " />
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
            <InMsTeamsProvider>
              <RootLayout>{children}</RootLayout>
            </InMsTeamsProvider>
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
      <GoogleAnalyticsProvider ignoreOptOutCookie={true} forceOverride={false} googleTagManagerContainerId={process.env.NEXT_PUBLIC_GOOGLE_TAG_MANAGER_CONTAINER_ID} />
    </html>
  );
};

export default ThemedRootLayout;
