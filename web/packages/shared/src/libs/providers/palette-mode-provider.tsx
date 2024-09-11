'use client';

import { useMediaQuery } from '@mui/material';
import { PaletteMode } from '@mui/material/styles';
import { createContext, useEffect, useMemo } from 'react';

export const PaletteModeContext = createContext({ toggleMode: () => {} });

type Props = {
  children?: React.ReactNode;
  loadDefaultSystemMode: boolean;
  setMode?: React.Dispatch<React.SetStateAction<PaletteMode>>;
};

const PaletteModeProvider = ({ children, loadDefaultSystemMode, setMode }: Props) => {
  const prefersDarkMode = useMediaQuery('(prefers-color-scheme: dark)');
  const colorMode = useMemo(
    () => ({
      toggleMode: () => {
        if (setMode) {
          setMode((prevMode) => (prevMode === 'light' ? 'dark' : 'light'));
        }
      },
    }),
    [setMode],
  );

  useEffect(() => {
    if (!loadDefaultSystemMode) {
      return;
    }

    if (setMode) {
      setMode(prefersDarkMode ? 'dark' : 'light');
    }
  }, [prefersDarkMode, loadDefaultSystemMode, setMode]);

  return <PaletteModeContext.Provider value={colorMode}>{children}</PaletteModeContext.Provider>;
};

export default PaletteModeProvider;
