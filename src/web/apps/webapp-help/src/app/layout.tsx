import { Barlow, Inter } from 'next/font/google';
import { Footer, Layout, Navbar } from 'nextra-theme-docs';
import 'nextra-theme-docs/style.css';
import { getPageMap } from 'nextra/page-map';
import type { PropsWithChildren } from 'react';

const inter = Inter({ subsets: ['latin'], weight: ['100', '200', '300', '400', '500', '600', '700', '800', '900'] });
const barlow = Barlow({ subsets: ['latin'], weight: ['100', '200', '300', '400', '500', '600', '700', '800', '900'] });

export const metadata = {
  title: 'Customer Help - Skedular',
  description: 'Help for browsing spaces, booking marketplace products, and managing customer bookings in Skedular.',
};

const RootLayout = async ({ children }: PropsWithChildren) => {
  return (
    <html lang="en" dir="ltr" suppressHydrationWarning>
      <title>Customer Help - Skedular</title>
      <meta name="viewport" content="width=device-width, initial-scale=1" />
      <meta name="description" content="Help for Skedular customers." />
      <link rel="icon" href="/images/skedular-icon-primary.svg" />
      <body className={`${inter.className} ${barlow.className}`}>
        <Layout navbar={<Navbar logo={<b>Customer Help</b>} />} pageMap={await getPageMap()} footer={<Footer>{new Date().getFullYear()} © Skedular.</Footer>}>
          {children}
        </Layout>
      </body>
    </html>
  );
};

export default RootLayout;
