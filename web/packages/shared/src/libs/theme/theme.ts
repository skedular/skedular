import { createTheme as createMuiTheme, PaletteMode, Theme } from '@mui/material/styles';
import type { CSSProperties } from '@mui/material/styles/createTypography';
import type { ResponsiveStyleValue } from '@mui/system';
import getDesignTokens from './theme-primitives';

export const defaultPadding: ResponsiveStyleValue<CSSProperties['paddingTop']> = { xs: 1, sm: 1, md: 5 };
export const maxScreenWidth = 1800;

const createTheme = (mode: PaletteMode): Theme => createMuiTheme(getDesignTokens(mode));

export default createTheme;
