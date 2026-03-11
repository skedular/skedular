import { useMediaQuery } from '@mui/material';
import { PaletteMode } from '@mui/material/styles';
import type { PropsWithChildren } from 'react';
import { createContext, useMemo } from 'react';
import { useLocalStorage } from 'usehooks-ts';

export type ExtendedPaletteMode = PaletteMode | 'system';

export const PaletteModeContext = createContext<PaletteMode>('light');
export const SelectedPaletteModeContext = createContext<ExtendedPaletteMode>('system');
export const UpdatePaletteModeContext = createContext<(mode: ExtendedPaletteMode) => void>(() => {});

const PaletteModeProvider = ({ children }: PropsWithChildren) => {
  const prefersDarkMode = useMediaQuery('(prefers-color-scheme: dark)');
  const [persistedPaletteMode, setPersistedPaletteMode] = useLocalStorage<ExtendedPaletteMode | undefined>('paletteMode', undefined);
  const selectedPaletteMode: ExtendedPaletteMode = persistedPaletteMode ?? 'system';
  const paletteMode: PaletteMode = useMemo(() => {
    if (selectedPaletteMode === 'dark') {
      return 'dark';
    }

    if (selectedPaletteMode === 'light') {
      return 'light';
    }

    return prefersDarkMode ? 'dark' : 'light';
  }, [selectedPaletteMode, prefersDarkMode]);

  const updatePaletteMode = (mode: ExtendedPaletteMode) => {
    setPersistedPaletteMode(mode);
  };

  return (
    <SelectedPaletteModeContext.Provider value={selectedPaletteMode}>
      <PaletteModeContext.Provider value={paletteMode}>
        <UpdatePaletteModeContext.Provider value={updatePaletteMode}>{children}</UpdatePaletteModeContext.Provider>
      </PaletteModeContext.Provider>
    </SelectedPaletteModeContext.Provider>
  );
};

export default PaletteModeProvider;
