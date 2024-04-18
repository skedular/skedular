import { Footer } from '@/components/footer';
import { Header } from '@/components/header';
import Box from '@mui/material/Box';
import { CookieConsent } from '@repo/shared/libs/cookie-consent';
import { ReactNode } from 'react';

interface Props {
  children: ReactNode;
}

const PublicMainRootLayout = ({ children }: Props) => {
  return (
    <Box component="main">
      <Header />
      {children}
      <Footer />
      <CookieConsent />
    </Box>
  );
};

export default PublicMainRootLayout;
