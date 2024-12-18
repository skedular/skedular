import { createTheme as createMuiTheme, PaletteMode, Theme } from '@mui/material/styles';
import type { CSSProperties } from '@mui/material/styles/createTypography';
import type { ResponsiveStyleValue, SxProps } from '@mui/system';
import { gridClasses } from '@mui/x-data-grid';
import getDesignTokens from './theme-primitives';

export const defaultPadding: ResponsiveStyleValue<CSSProperties['paddingTop']> = { xs: 1, sm: 1, md: 5 };
export const maxScreenWidth = 1800;
export const defaultGridStyle: SxProps<Theme> = {
  border: 'none',
  [`& .${gridClasses.cell}`]: {
    paddingTop: 1,
    paddingBottom: 1,
    border: 'none',
  },
  [`& .${gridClasses.row}`]: {
    paddingLeft: 1,
    paddingTop: 1,
    paddingBottom: 1,
    borderRadius: 2,
    backgroundColor: (theme) => theme.palette.background.paper,
    border: 'none',
  },
};

const createTheme = (mode: PaletteMode): Theme => createMuiTheme(getDesignTokens(mode));

export default createTheme;
