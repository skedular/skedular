import Box from '@mui/material/Box';
import Link from 'next/link';
import { ReactNode, memo } from 'react';
import { Navigation } from './navigation.data';

interface Props {
  navigation: Navigation;
  children: ReactNode;
}

const NavigationRootLinkWithoutSubItems = ({ navigation: { path }, children }: Props) => {
  return (
    <Box
      component={Link}
      href={path}
      sx={{
        textDecoration: 'none',
        position: 'relative',
        color: 'text.primary',
        cursor: 'pointer',
        fontWeight: 600,
        display: 'inline-flex',
        alignItems: 'center',
        justifyContent: 'center',
        px: { xs: 0, md: 3 },
        mb: { xs: 3, md: 0 },
        fontSize: { xs: '1.2rem', md: 'inherit' },

        '& > div': { display: 'none' },

        '&.current>div': { display: 'block' },

        '&:hover': {
          color: 'primary.main',
          '&>div': {
            display: 'block',
          },
        },
      }}
    >
      {children}
    </Box>
  );
};

export default memo(NavigationRootLinkWithoutSubItems);
