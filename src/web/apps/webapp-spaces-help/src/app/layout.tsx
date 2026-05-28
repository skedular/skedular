import { Barlow, Inter } from 'next/font/google';
import { Footer, Layout, Navbar } from 'nextra-theme-docs';
import 'nextra-theme-docs/style.css';
import { Banner } from 'nextra/components';
import { getPageMap } from 'nextra/page-map';
import type { PropsWithChildren } from 'react';

const inter = Inter({ subsets: ['latin'], weight: ['100', '200', '300', '400', '500', '600', '700', '800', '900'] });
const barlow = Barlow({ subsets: ['latin'], weight: ['100', '200', '300', '400', '500', '600', '700', '800', '900'] });

export const metadata = {
  // Define your metadata here
  // For more information on metadata API, see: https://nextjs.org/docs/app/building-your-application/optimizing/metadata
};

const RootLayout = async ({ children }: PropsWithChildren) => {
  return (
    <html lang="en" dir="ltr" suppressHydrationWarning>
      <title>Skedular</title>
      <meta name="viewport" content="width=device-width, initial-scale=1" />
      <meta name="description" content="The premier solution for modern workspace management " />
      <link rel="icon" href="/images/skedular-icon-primary.svg" />
      <body className={`${inter.className} ${barlow.className}`}>
        <Layout
          banner={<Banner storageKey="some-key">Nextra 4.0 is released 🎉</Banner>}
          navbar={<Navbar logo={<b>Skedular</b>} />}
          pageMap={await getPageMap()}
          docsRepositoryBase="https://github.com/shuding/nextra/tree/main/docs"
          footer={<Footer>{new Date().getFullYear()} © Skedular.</Footer>}
          // ... Your additional layout options
        >
          {children}
        </Layout>
      </body>
    </html>
  );
};

export default RootLayout;
