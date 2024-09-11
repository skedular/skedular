import { ThemeProvider as MuiThemeProvider } from '@mui/material';
import { PaletteMode } from '@mui/material/styles';
import { createTheme } from '@repo/shared/libs/theme';
import { useMemo } from 'react';

type Props = {
  children?: React.ReactNode;
  mode: PaletteMode;
};

const ThemeProvider = ({ children, mode }: Props) => {
  const theme = useMemo(() => createTheme(mode), [mode]);

  return <MuiThemeProvider theme={theme}>{children}</MuiThemeProvider>;
};

export default ThemeProvider;
