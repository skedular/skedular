'use client';

import { useMediaQuery } from '@mui/material';
import type { ColorMode } from '../theme';
import { createContext, useEffect, useMemo } from 'react';

export const ColorModeContext = createContext({ toggleColorMode: () => {} });

type Props = {
  children?: React.ReactNode;
  loadDefaultSystemMode: boolean;
  setMode: React.Dispatch<React.SetStateAction<ColorMode>>;
};

const ColorModeProvider = ({ children, loadDefaultSystemMode, setMode }: Props) => {
  const prefersDarkMode = useMediaQuery('(prefers-color-scheme: dark)');
  const colorMode = useMemo(
    () => ({
      toggleColorMode: () => {
        setMode((prevMode) => (prevMode === 'light' ? 'dark' : 'light'));
      },
    }),
    [setMode],
  );

  useEffect(() => {
    if (!loadDefaultSystemMode) {
      return;
    }

    setMode(prefersDarkMode ? 'dark' : 'light');
  }, [prefersDarkMode, loadDefaultSystemMode, setMode]);

  return <ColorModeContext.Provider value={colorMode}>{children}</ColorModeContext.Provider>;
};

export default ColorModeProvider;
