import { Footer } from '@/components/footer';
import { Header } from '@/components/header';
import Box from '@mui/material/Box';
import { PropsWithChildren } from 'react';

const PublicMainRootLayout = ({ children }: PropsWithChildren) => (
  <Box component="main">
    <Header />
    {children}
    <Footer />
  </Box>
);

export default PublicMainRootLayout;
