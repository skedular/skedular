import { useMediaQuery } from '@mui/material';
import { PaletteMode } from '@mui/material/styles';
import type { PropsWithChildren } from 'react';
import { createContext, useMemo } from 'react';
import { useLocalStorage } from 'usehooks-ts';

export type ExtendedPaletteMode = PaletteMode | 'system';

export const PaletteModeContext = createContext<PaletteMode>('light');
export const UpdatePaletteModeContext = createContext<(mode: ExtendedPaletteMode) => void>(() => {});

const PaletteModeProvider = ({ children }: PropsWithChildren) => {
  const prefersDarkMode = useMediaQuery('(prefers-color-scheme: dark)');
  const [persistedPaletteMode, setPersistedPaletteMode] = useLocalStorage<ExtendedPaletteMode | undefined>('paletteMode', undefined);
  const paletteMode: PaletteMode = useMemo(() => {
    if (persistedPaletteMode === 'dark') {
      return 'dark';
    }

    if (persistedPaletteMode === 'light') {
      return 'light';
    }

    return prefersDarkMode ? 'dark' : 'light';
  }, [persistedPaletteMode, prefersDarkMode]);

  const updatePaletteMode = (mode: ExtendedPaletteMode) => {
    setPersistedPaletteMode(mode);
  };

  return (
    <PaletteModeContext.Provider value={paletteMode}>
      <UpdatePaletteModeContext.Provider value={updatePaletteMode}>{children}</UpdatePaletteModeContext.Provider>
    </PaletteModeContext.Provider>
  );
};

export default PaletteModeProvider;
