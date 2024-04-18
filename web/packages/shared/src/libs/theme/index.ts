import { Theme, createTheme as createMuiTheme } from '@mui/material/styles';
import paletteBase from './palette-base';
import paletteDark from './palette-dark';
import paletteLight from './palette-light';
import shadows from './shadows';
import typography from './typography';

export type ColorMode = 'light' | 'dark';

export { paletteBase, paletteDark, paletteLight, shadows, typography };

const createTheme = (colorMode: ColorMode): Theme => {
  const palette = colorMode === 'light' ? { ...paletteBase, ...paletteLight } : { ...paletteBase, ...paletteDark };
  return createMuiTheme({
    palette,
    typography,
    shadows,
  });
};

export default createTheme;
