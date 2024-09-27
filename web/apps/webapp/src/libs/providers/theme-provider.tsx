'use client';

import { ThemeProvider as MuiThemeProvider } from '@mui/material';
import { PaletteMode } from '@mui/material/styles';
import { createTheme } from '@repo/shared/libs/theme';

type Props = {
  children?: React.ReactNode;
  mode: PaletteMode;
};

const ThemeProvider = ({ children, mode }: Props) => {
  const theme = createTheme(mode);

  return <MuiThemeProvider theme={theme}>{children}</MuiThemeProvider>;
};

export default ThemeProvider;
