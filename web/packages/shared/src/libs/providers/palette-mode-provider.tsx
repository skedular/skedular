import { useMediaQuery } from '@mui/material';
import { PaletteMode } from '@mui/material/styles';
import { createContext, PropsWithChildren, useEffect, useState } from 'react';
import { useLocalStorage } from 'usehooks-ts';

export type ExtendedPaletteMode = PaletteMode | 'system';

export const PaletteModeContext = createContext<PaletteMode>('light');
export const UpdatePaletteModeContext = createContext<(mode: ExtendedPaletteMode) => void>(() => {});

const PaletteModeProvider = ({ children }: PropsWithChildren) => {
  const prefersDarkMode = useMediaQuery('(prefers-color-scheme: dark)');
  const [persistedPaletteMode, setPersistedPaletteMode] = useLocalStorage<ExtendedPaletteMode | undefined>('paletteMode', undefined);
  const [paletteMode, setPaletteMode] = useState<PaletteMode>('light');

  useEffect(() => {
    let finalPaletteMode: PaletteMode;
    if (persistedPaletteMode) {
      if (persistedPaletteMode === 'system') {
        finalPaletteMode = prefersDarkMode ? 'dark' : 'light';
      } else if (persistedPaletteMode === 'dark') {
        finalPaletteMode = 'dark';
      } else {
        finalPaletteMode = 'light';
      }
    } else {
      finalPaletteMode = prefersDarkMode ? 'dark' : 'light';
    }

    setPaletteMode(finalPaletteMode);
  }, [persistedPaletteMode, prefersDarkMode]);

  const updatePaletteMode = (paletteMode: ExtendedPaletteMode) => {
    switch (paletteMode) {
      case 'system':
        setPaletteMode(prefersDarkMode ? 'dark' : 'light');
        break;
      case 'dark':
        setPaletteMode('dark');
        break;
      case 'light':
        setPaletteMode('light');
        break;
      default:
        setPaletteMode('light');
        break;
    }

    setPersistedPaletteMode(paletteMode);
  };

  return (
    <PaletteModeContext.Provider value={paletteMode}>
      <UpdatePaletteModeContext.Provider value={updatePaletteMode}>{children}</UpdatePaletteModeContext.Provider>
    </PaletteModeContext.Provider>
  );
};

export default PaletteModeProvider;
