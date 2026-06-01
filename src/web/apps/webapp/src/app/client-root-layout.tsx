'use client';

import { GoogleAnalytics, GoogleTagManager } from '@/libs/analytics';
import { AppRouterCacheProvider } from '@mui/material-nextjs/v15-appRouter';
import CssBaseline from '@mui/material/CssBaseline';
import {
  AuthenticatedRelayProvider,
  DatePickerLocalizationProvider,
  GoogleAnalyticsProvider,
  LogRocketProvider,
  MuiXLicense,
  PaletteModeContext,
  PaletteModeProvider,
  ThemeProvider,
} from '@skedular/shared';
import { Analytics } from '@vercel/analytics/react';
import { SpeedInsights } from '@vercel/speed-insights/react';
import { AuthKitProvider, useAuth } from '@workos-inc/authkit-nextjs/components';
import Script from 'next/script';
import type { PropsWithChildren } from 'react';
import { memo, useContext } from 'react';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

type AppAuthenticatedRelayProviderProps = PropsWithChildren<{
  teamsToken: string | null;
}>;

const AppAuthenticatedRelayProvider = ({ children, teamsToken }: AppAuthenticatedRelayProviderProps) => {
  const { loading: authLoading } = useAuth();

  return (
    <AuthenticatedRelayProvider authLoading={authLoading} teamsToken={teamsToken}>
      {children}
    </AuthenticatedRelayProvider>
  );
};

const InnerRootLayout = ({ children }: PropsWithChildren) => {
  const paletteMode = useContext(PaletteModeContext);

  return (
    <ThemeProvider mode={paletteMode}>
      <CssBaseline />
      <DatePickerLocalizationProvider>
        <AuthKitProvider>
          <AppAuthenticatedRelayProvider teamsToken={null}>{children}</AppAuthenticatedRelayProvider>
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
        <InnerRootLayout>{children}</InnerRootLayout>
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
