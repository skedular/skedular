'use client';

import { useMediaQuery } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { SnackbarProvider as Provider } from 'notistack';

type Props = {
  children?: React.ReactNode;
};

const SnackbarProvider = ({ children }: Props) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

  return <Provider maxSnack={isMobile ? 3 : 10}>{children}</Provider>;
};

export default SnackbarProvider;
