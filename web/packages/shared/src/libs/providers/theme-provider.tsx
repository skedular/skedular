import { ThemeProvider as MuiThemeProvider } from '@mui/material';
import { PaletteMode } from '@mui/material/styles';
import { PropsWithChildren, useMemo } from 'react';
import { createTheme } from '../theme';

type Props = {
  mode: PaletteMode;
};

const ThemeProvider = ({ children, mode }: PropsWithChildren<Props>) => {
  const theme = useMemo(() => createTheme(mode), [mode]);

  return <MuiThemeProvider theme={theme}>{children}</MuiThemeProvider>;
};

export default ThemeProvider;
