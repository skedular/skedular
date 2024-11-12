'use client';

import { AppRouterCacheProvider } from '@mui/material-nextjs/v14-appRouter';
import { DatePickerLocalizationProvider, PaletteModeContext, PaletteModeProvider, ThemeProvider } from '@repo/shared/libs/providers';
import { Analytics } from '@vercel/analytics/react';
import { SpeedInsights } from '@vercel/speed-insights/react';
import Script from 'next/script';
import { useContext } from 'react';

const RootLayout = ({ children }: { children: React.ReactNode }) => {
  const paletteMode = useContext(PaletteModeContext);

  return (
    <AppRouterCacheProvider>
      <ThemeProvider mode={paletteMode}>
        <DatePickerLocalizationProvider>
          <>{children}</>
        </DatePickerLocalizationProvider>
      </ThemeProvider>
    </AppRouterCacheProvider>
  );
};

const ThemedRootLayout = ({ children }: Readonly<{ children: React.ReactNode }>) => (
  <html lang="en">
    <title>Skedular</title>
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
    </body>
  </html>
);

export default ThemedRootLayout;
