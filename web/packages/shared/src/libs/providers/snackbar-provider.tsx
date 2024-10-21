import { useMediaQuery } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { SnackbarProvider as Provider } from 'notistack';

type Props = {
  children?: React.ReactNode;
};

const SnackbarProvider = ({ children }: Props) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

  return (
    <Provider maxSnack={isMobile ? 2 : 5} autoHideDuration={3000}>
      {children}
    </Provider>
  );
};

export default SnackbarProvider;
