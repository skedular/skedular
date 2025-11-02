import type { Metadata, Viewport } from 'next';
import { Barlow, Inter } from 'next/font/google';
import type { PropsWithChildren } from 'react';
import ClientRootLayout from './client-root-layout';

const inter = Inter({ subsets: ['latin'], weight: ['100', '200', '300', '400', '500', '600', '700', '800', '900'] });
const barlow = Barlow({ subsets: ['latin'], weight: ['100', '200', '300', '400', '500', '600', '700', '800', '900'] });

export const metadata: Metadata = {
  title: 'Skedular',
  description: 'The premier solution for modern workspace management ',
  icons: '/images/skedular-icon-primary.svg',
};

export const viewport: Viewport = {
  width: 'device-width',
  initialScale: 1,
  viewportFit: 'cover',
};

const RootLayout = ({ children }: PropsWithChildren) => (
  <html lang="en" dir="ltr" suppressHydrationWarning>
    <body className={`${inter.className} ${barlow.className}`}>
      <ClientRootLayout>{children}</ClientRootLayout>
    </body>
  </html>
);

export default RootLayout;
