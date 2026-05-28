'use client';

import { useMediaQuery } from '@mui/material';
import { PaletteMode } from '@mui/material/styles';
import type { ExtendedPaletteMode } from '@skedular/ui';
import { PaletteModeContext, SelectedPaletteModeContext, UpdatePaletteModeContext } from '@skedular/ui';
import type { PropsWithChildren } from 'react';
import { useEffect, useMemo, useState } from 'react';
import { useLocalStorage } from 'usehooks-ts';

export type { ExtendedPaletteMode };

const PaletteModeProvider = ({ children }: PropsWithChildren) => {
  const [isMounted, setIsMounted] = useState(false);
  const prefersDarkMode = useMediaQuery('(prefers-color-scheme: dark)');
  const [persistedPaletteMode, setPersistedPaletteMode] = useLocalStorage<ExtendedPaletteMode | undefined>('paletteMode', undefined);
  const selectedPaletteMode: ExtendedPaletteMode = persistedPaletteMode ?? 'system';

  useEffect(() => {
    setIsMounted(true);
  }, []);

  const paletteMode: PaletteMode = useMemo(() => {
    // Return 'light' until mounted so client hydration matches server render.
    // useLocalStorage reads eagerly from localStorage on the client but the
    // server always sees undefined — deferring until mounted prevents the
    // class-name mismatch that causes React hydration errors.
    if (!isMounted) {
      return 'light';
    }

    if (selectedPaletteMode === 'dark') {
      return 'dark';
    }

    if (selectedPaletteMode === 'light') {
      return 'light';
    }

    return prefersDarkMode ? 'dark' : 'light';
  }, [isMounted, selectedPaletteMode, prefersDarkMode]);

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

export { PaletteModeContext, SelectedPaletteModeContext, UpdatePaletteModeContext };
export default PaletteModeProvider;
