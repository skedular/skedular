'use client';

import { useMediaQuery } from '@mui/material';
import { PaletteMode } from '@mui/material/styles';
import { createContext, useEffect, useMemo } from 'react';

export const PaletteModeContext = createContext({ toggleMode: () => {} });

type Props = {
  children?: React.ReactNode;
  loadDefaultSystemMode: boolean;
  setMode: React.Dispatch<React.SetStateAction<PaletteMode>>;
};

const localStorageThemeModeKey = 'themeMode';

const PaletteModeProvider = ({ children, loadDefaultSystemMode, setMode }: Props) => {
  const prefersDarkMode = useMediaQuery('(prefers-color-scheme: dark)');
  const colorMode = useMemo(
    () => ({
      toggleMode: () => {
        if (setMode) {
          setMode((prevMode) => {
            const newMode = prevMode === 'light' ? 'dark' : 'light';
            localStorage.setItem(localStorageThemeModeKey, newMode);

            return newMode;
          });
        }
      },
    }),
    [setMode],
  );

  useEffect(() => {
    const savedMode = localStorage.getItem(localStorageThemeModeKey) as PaletteMode | null;
    if (savedMode) {
      setMode(savedMode);
    } else {
      if (!loadDefaultSystemMode) {
        return;
      }

      setMode(prefersDarkMode ? 'dark' : 'light');
    }
  }, [prefersDarkMode, loadDefaultSystemMode, setMode]);

  return <PaletteModeContext.Provider value={colorMode}>{children}</PaletteModeContext.Provider>;
};

export default PaletteModeProvider;
