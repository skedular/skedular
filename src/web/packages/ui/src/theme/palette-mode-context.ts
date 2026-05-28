'use client';

import type { PaletteMode } from '@mui/material/styles';
import { createContext } from 'react';

export type ExtendedPaletteMode = PaletteMode | 'system';

export const PaletteModeContext = createContext<PaletteMode>('light');
export const SelectedPaletteModeContext = createContext<ExtendedPaletteMode>('system');
export const UpdatePaletteModeContext = createContext<(mode: ExtendedPaletteMode) => void>(() => {});
