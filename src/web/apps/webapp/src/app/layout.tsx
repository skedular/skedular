import { getProductAppDefinition } from '@skedular/shared';
import type { Metadata, Viewport } from 'next';
import localFont from 'next/font/local';
import { headers } from 'next/headers';
import type { PropsWithChildren } from 'react';
import { isOrganizationCustomDomainHost } from './auth/host-utils';
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

export async function generateMetadata(): Promise<Metadata> {
  const headersList = await headers();
  const host = headersList.get('host') ?? '';
  const isCustomDomain = isOrganizationCustomDomainHost(host);

  if (isCustomDomain) {
    // Omit title and icons entirely so the browser never flashes "Skedular"
    // or the Skedular favicon on custom domain pages. The client-side
    // StoreFrontBrowserMetadata component fills them in once org data loads.
    return {
      description: 'The premier solution for modern workspace management',
    };
  }

  return {
    title: 'Skedular',
    description: 'The premier solution for modern workspace management',
    icons: '/images/skedular-icon-primary.svg',
  };
}

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
