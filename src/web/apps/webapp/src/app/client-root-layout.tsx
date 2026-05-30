'use client';

import {
  AuthenticatedRelayProvider,
  DatePickerLocalizationProvider,
  GoogleAnalyticsProvider,
  InMsTeamsContext,
  InMsTeamsProvider,
  LogRocketProvider,
  MuiXLicense,
  PaletteModeContext,
  PaletteModeProvider,
  ThemeProvider,
} from '@skedular/shared';
import { GoogleAnalytics, GoogleTagManager } from '@/libs/analytics';

import { TeamsUserCredential } from '@microsoft/teamsfx';
import { AppRouterCacheProvider } from '@mui/material-nextjs/v15-appRouter';
import CssBaseline from '@mui/material/CssBaseline';
import { Analytics } from '@vercel/analytics/react';
import { SpeedInsights } from '@vercel/speed-insights/react';
import { AuthKitProvider, useAccessToken, useAuth } from '@workos-inc/authkit-nextjs/components';
import Script from 'next/script';
import type { PropsWithChildren } from 'react';
import { memo, useContext, useEffect, useState } from 'react';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

type AppAuthenticatedRelayProviderProps = PropsWithChildren<{
  teamsToken: string | null;
}>;

const AppAuthenticatedRelayProvider = ({ children, teamsToken }: AppAuthenticatedRelayProviderProps) => {
  const { user, loading: authLoading } = useAuth();
  const { accessToken, loading: accessTokenLoading } = useAccessToken();

  return (
    <AuthenticatedRelayProvider accessToken={accessToken} accessTokenLoading={accessTokenLoading} authLoading={authLoading} teamsToken={teamsToken} userSignedIn={!!user}>
      {children}
    </AuthenticatedRelayProvider>
  );
};

const InnerRootLayout = ({ children }: PropsWithChildren) => {
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
        } catch {
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
    <ThemeProvider mode={paletteMode}>
      <CssBaseline />
      <DatePickerLocalizationProvider>
        <AuthKitProvider>
          <AppAuthenticatedRelayProvider teamsToken={token}>{children}</AppAuthenticatedRelayProvider>
        </AuthKitProvider>
      </DatePickerLocalizationProvider>
    </ThemeProvider>
  );
};

const ClientRootLayout = ({ children }: PropsWithChildren) => (
  <>
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
    <AppRouterCacheProvider>
      <PaletteModeProvider>
        <InMsTeamsProvider>
          <InnerRootLayout>{children}</InnerRootLayout>
        </InMsTeamsProvider>
      </PaletteModeProvider>
    </AppRouterCacheProvider>
    <Analytics />
    <SpeedInsights />
    <MuiXLicense />
    <ToastContainer position="top-right" pauseOnFocusLoss pauseOnHover hideProgressBar={false} draggable rtl={false} />
    <GoogleAnalytics ignoreOptOutCookie={true} forceOverride={false} />
    <GoogleTagManager ignoreOptOutCookie={true} forceOverride={false} />
    <LogRocketProvider ignoreOptOutCookie={true} forceOverride={false} logRocketAppId={process.env.NEXT_PUBLIC_LOGROCKET_APP_ID} />
    <GoogleAnalyticsProvider ignoreOptOutCookie={true} forceOverride={false} googleTagManagerContainerId={process.env.NEXT_PUBLIC_GOOGLE_TAG_MANAGER_CONTAINER_ID} />
  </>
);

export default memo(ClientRootLayout);
