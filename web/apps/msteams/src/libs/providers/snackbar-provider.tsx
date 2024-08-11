import { useMediaQuery } from '@mui/material';
import { SnackbarProvider as Provider } from 'notistack';

type Props = {
  children?: React.ReactNode;
};

const SnackbarProvider = ({ children }: Props) => {
  const isDesktop = useMediaQuery('(min-width: 600px)');

  return <Provider maxSnack={isDesktop ? 10 : 3}>{children}</Provider>;
};

export default SnackbarProvider;
