'use client';

import { ThemeProvider as MuiThemeProvider } from '@mui/material/styles';
import { PaletteMode } from '@mui/material/styles';
import { createTheme } from '@skedular/ui';
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
