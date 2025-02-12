import { createTheme } from '@/libs/theme';
import { ThemeProvider as MuiThemeProvider } from '@mui/material';
import { PaletteMode } from '@mui/material/styles';
import type { PropsWithChildren } from 'react';
import { useMemo } from 'react';

type Props = {
  mode: PaletteMode;
};

const ThemeProvider = ({ children, mode }: PropsWithChildren<Props>) => {
  const theme = useMemo(() => createTheme(mode), [mode]);

  return <MuiThemeProvider theme={theme}>{children}</MuiThemeProvider>;
};

export default ThemeProvider;
