import { createTheme as createMuiTheme, PaletteMode, Theme } from '@mui/material/styles';
import type { CSSProperties } from '@mui/material/styles/createTypography';
import type { ResponsiveStyleValue, SxProps } from '@mui/system';
import { gridClasses } from '@mui/x-data-grid';
import getDesignTokens, { coal } from './theme-primitives';

export const defaultButtonStyle: SxProps<Theme> = {
  backgroundColor: 'white',
  borderColor: coal,
  borderWidth: 1,
  borderStyle: 'solid',
  textTransform: 'none',
};
export const defaultPadding: ResponsiveStyleValue<CSSProperties['paddingTop']> = { xs: 1, sm: 1, md: 3 };
export const defaultGridActionPadding: ResponsiveStyleValue<CSSProperties['paddingTop']> = { xs: 1, sm: 1, md: 2 };
export const maxScreenWidth = 1700;

export const defaultGridStyle: SxProps<Theme> = {
  border: 'none',
  [`& .${gridClasses.cell}`]: {
    border: 'none',
  },
  [`& .${gridClasses.row}`]: {
    borderRadius: 2,
    backgroundColor: (theme) => theme.palette.background.paper,
    border: 'none',
  },
};

export const defaultOldGridStyle: SxProps<Theme> = {
  border: 'none',
  [`& .${gridClasses.cell}`]: {
    border: 'none',
  },
  [`& .${gridClasses.row}`]: {
    borderRadius: 2,
    backgroundColor: (theme) => theme.palette.background.paper,
    border: 'none',
  },
};

export const selectedListItemPaddings = { paddingRight: 5 };
export const getSelectedListItemBorderRadius = (selected: boolean): number => (selected ? 4 : 0);

const createTheme = (mode: PaletteMode): Theme => createMuiTheme(getDesignTokens(mode));

export const secondDrawerExpandedDrawerWidth = 210;
export const secondDrawerCollapsedDrawerWidth = 80;
export const secondDrawerExpandedDrawerWidthPx = '230px';
export const secondDrawerCollapsedDrawerWidthPx = '100px';

export default createTheme;
