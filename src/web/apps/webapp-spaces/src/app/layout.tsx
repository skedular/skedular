import { getProductAppDefinition } from '@skedular/shared';
import type { Metadata, Viewport } from 'next';
import localFont from 'next/font/local';
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
    { path: './fonts/Barlow-Regular.woff2', weight: '400', style: 'normal' },
    { path: './fonts/Barlow-Medium.woff2', weight: '500', style: 'normal' },
    { path: './fonts/Barlow-SemiBold.woff2', weight: '600', style: 'normal' },
    { path: './fonts/Barlow-Bold.woff2', weight: '700', style: 'normal' },
  ],
  variable: '--font-barlow',
  display: 'swap',
  adjustFontFallback: false,
});

const appDefinition = getProductAppDefinition('webapp');

export const metadata: Metadata = {
  title: 'Skedular Spaces',
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
    <body suppressHydrationWarning>
      <ClientRootLayout>{children}</ClientRootLayout>
    </body>
  </html>
);

export default RootLayout;
