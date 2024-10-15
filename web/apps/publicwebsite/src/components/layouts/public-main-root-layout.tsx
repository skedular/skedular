import { Footer } from '@/components/footer';
import { Header } from '@/components/header';
import Box from '@mui/material/Box';
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
    </Box>
  );
};

export default PublicMainRootLayout;
