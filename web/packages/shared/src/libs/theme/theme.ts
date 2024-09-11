import { createTheme as createMuiTheme, PaletteMode, Theme } from '@mui/material/styles';
import getDesignTokens from './theme-primitives';

const createTheme = (mode: PaletteMode): Theme => {
  return createMuiTheme(getDesignTokens(mode));
};

export default createTheme;
