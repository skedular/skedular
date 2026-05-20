import type { Metadata, Viewport } from 'next';
import localFont from 'next/font/local';
import { getProductAppDefinition } from '@skedular/shared';
import type { PropsWithChildren } from 'react';
import ClientRootLayout from './client-root-layout';
import './fonts.css';

const inter = localFont({
  src: './fonts/InterVariable.woff2',
  variable: '--font-inter',
  display: 'swap',
  adjustFontFallback: false,
});

const barlow = localFont({
  src: [
    { path: './fonts/Barlow-Regular.ttf', weight: '400', style: 'normal' },
    { path: './fonts/Barlow-Medium.ttf', weight: '500', style: 'normal' },
    { path: './fonts/Barlow-SemiBold.ttf', weight: '600', style: 'normal' },
    { path: './fonts/Barlow-Bold.ttf', weight: '700', style: 'normal' },
  ],
  variable: '--font-barlow',
  display: 'swap',
  adjustFontFallback: false,
});

const appDefinition = getProductAppDefinition('webapp');

export const metadata: Metadata = {
  title: 'Skedular',
  description: 'The premier solution for modern workspace management',
  icons: '/images/skedular-icon-primary.svg',
};

export const viewport: Viewport = {
  width: 'device-width',
  initialScale: 1,
  viewportFit: 'cover',
};

const RootLayout = ({ children }: PropsWithChildren) => (
  <html lang="en" dir="ltr" suppressHydrationWarning className={`${inter.variable} ${barlow.variable}`} data-product-app={appDefinition.id}>
    <body>
      <ClientRootLayout>{children}</ClientRootLayout>
    </body>
  </html>
);

export default RootLayout;
