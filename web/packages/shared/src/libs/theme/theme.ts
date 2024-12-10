import { createTheme as createMuiTheme, PaletteMode, Theme } from '@mui/material/styles';
import getDesignTokens from './theme-primitives';

export const defaultPadding = { xs: 1, sm: 1, md: 5 };
export const defaultSpacing = { xs: 1, sm: 1, md: 1, lg: 15 };
export const maxScreenWidth = 1800;

const createTheme = (mode: PaletteMode): Theme => createMuiTheme(getDesignTokens(mode));

export default createTheme;
