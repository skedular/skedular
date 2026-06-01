import { Barlow, Inter } from 'next/font/google';
import { Footer, Layout, Navbar } from 'nextra-theme-docs';
import 'nextra-theme-docs/style.css';
import { getPageMap } from 'nextra/page-map';
import type { PropsWithChildren } from 'react';

const inter = Inter({ subsets: ['latin'], weight: ['100', '200', '300', '400', '500', '600', '700', '800', '900'] });
const barlow = Barlow({ subsets: ['latin'], weight: ['100', '200', '300', '400', '500', '600', '700', '800', '900'] });

export const metadata = {
  title: 'Spaces Help - Skedular',
  description: 'Help for marketplace and co-working operators using Skedular Spaces.',
};

const RootLayout = async ({ children }: PropsWithChildren) => {
  return (
    <html lang="en" dir="ltr" suppressHydrationWarning>
      <title>Spaces Help - Skedular</title>
      <meta name="viewport" content="width=device-width, initial-scale=1" />
      <meta name="description" content="Help for Skedular Spaces." />
      <link rel="icon" href="/images/skedular-icon-primary.svg" />
      <body className={`${inter.className} ${barlow.className}`}>
        <Layout navbar={<Navbar logo={<b>Spaces Help</b>} />} pageMap={await getPageMap()} footer={<Footer>{new Date().getFullYear()} © Skedular.</Footer>}>
          {children}
        </Layout>
      </body>
    </html>
  );
};

export default RootLayout;
