import { createTheme as createMuiTheme, PaletteMode, Theme } from '@mui/material/styles';
import paletteBase from './palette-base';
import paletteDark from './palette-dark';
import paletteLight from './palette-light';
import shadows from './shadows';
import typography from './typography';

export { paletteBase, paletteDark, paletteLight, shadows, typography };

const createTheme = (mode: PaletteMode): Theme => {
  const palette = mode === 'light' ? { ...paletteBase, ...paletteLight } : { ...paletteBase, ...paletteDark };
  return createMuiTheme({
    palette,
    typography,
    shadows,
  });
};

export default createTheme;
